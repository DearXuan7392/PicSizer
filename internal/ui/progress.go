package ui

import (
	"fmt"
	"sync"

	"PicSizer/internal/core"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
)

// ProgressForm 进度窗口
//
// 设计要点:
//  1. 基于 walk.Dialog 实现, 作为 owner 的子窗口, 显示时会自动获得焦点并置于上层.
//  2. 窗体固定大小 (FixedSize + MinSize == MaxSize), 用户无法拖拽调整.
//  3. Show 时禁用 owner, 实现"父窗体无法操作"的模态效果; 关闭时恢复 owner 的可用状态.
//  4. 居中算法基于 owner 的位置和尺寸 (CenterWindowToOwner), 而非屏幕中心.
//  5. 点击"取消"按钮或窗体的关闭按钮 (叉叉) 都会触发 onCancel 回调,
//     通知 MainForm 停止压缩任务 (调用 pool.Stop).
//  6. 由于 compress.Compress 是同步阻塞调用, 无法从外部强制中断正在执行的图片压缩,
//     pool.Stop 采用"清空任务数组 + ExitFlag"的双保险策略:
//     正在压缩的那张图会自然完成, 后续未启动的任务不再执行, 全部线程在毫秒级内结束.
//  7. "等待当前任务结束" 状态: 用户中途停止压缩后, 不立即关闭窗口, 而是:
//     - 标签切换为"正在等待当前任务结束..."
//     - 取消按钮被禁用 (变灰)
//     - 窗体关闭按钮 (叉叉) 被拦截, 阻止用户关闭窗口
//     - 待所有子任务自然结束后, 由 pool.onComplete 回调统一调用 Close() 关闭窗体.
type ProgressForm struct {
	*walk.Dialog
	progressBar *walk.ProgressBar
	labelInfo   *walk.Label
	btnCancel   *walk.PushButton
	total       int

	mu       sync.Mutex
	visible  bool      // 当前是否处于显示状态
	stopping bool      // 是否已进入"等待结束"状态 (用户已请求停止, 但子任务尚未全部结束)
	owner    walk.Form // 父窗体, 用于在 Show/Close 时切换其可用状态
	onCancel func()    // 取消 (按钮或关闭按钮) 时的回调
	onClosed func()    // 窗体已关闭回调 (用于业务清理, 如释放 pool 引用)
}

// NewProgressForm 创建进度窗口
func NewProgressForm() *ProgressForm {
	return &ProgressForm{}
}

// SetOnCancel 设置取消回调
// 当用户点击"取消"按钮或窗体关闭按钮时, 该回调会被调用
func (pf *ProgressForm) SetOnCancel(fn func()) {
	pf.mu.Lock()
	pf.onCancel = fn
	pf.mu.Unlock()
}

// SetOnClosed 设置关闭完成回调
// 当进度窗体彻底关闭 (无论是否因取消) 后, 该回调会被调用一次
func (pf *ProgressForm) SetOnClosed(fn func()) {
	pf.mu.Lock()
	pf.onClosed = fn
	pf.mu.Unlock()
}

// IsVisible 返回当前是否处于显示状态
// MainForm 在点击"开始压缩"时, 通过该方法判断是否已有任务正在进行
func (pf *ProgressForm) IsVisible() bool {
	pf.mu.Lock()
	defer pf.mu.Unlock()
	return pf.visible
}

// IsStopping 返回是否已进入"等待结束"状态
//
// 处于该状态时, 进度窗体的关闭请求会被拦截, 等待所有子任务结束后再统一关闭.
func (pf *ProgressForm) IsStopping() bool {
	pf.mu.Lock()
	defer pf.mu.Unlock()
	return pf.stopping
}

// Show 显示进度窗口
//
// 参数:
//   - owner: 父窗体 (通常为主窗口), 进度窗体会相对于它居中显示并禁用它.
//   - total: 总任务数.
//   - appIcon: 应用程序图标.
//
// 返回:
//   - 若当前已有一个进度窗体在显示, 返回错误而不创建新窗体.
//   - 否则创建并显示窗体, 返回 nil.
func (pf *ProgressForm) Show(owner walk.Form, total int, appIcon *walk.Icon) error {
	pf.mu.Lock()
	if pf.visible {
		pf.mu.Unlock()
		return fmt.Errorf("进度窗口已在显示中")
	}
	pf.visible = true
	pf.owner = owner
	pf.total = total
	pf.stopping = false
	pf.mu.Unlock()

	// 禁用父窗体, 实现模态效果 (父窗体无法操作, 类似模态对话框)
	if owner != nil {
		owner.SetEnabled(false)
	}

	err := declarative.Dialog{
		AssignTo:  &pf.Dialog,
		Title:     core.TitleProgress,
		Icon:      appIcon,
		FixedSize: true,
		// 三者尺寸一致, 既保证显示效果, 也彻底禁止用户调整窗口大小
		MinSize: declarative.Size{Width: 400, Height: 150},
		MaxSize: declarative.Size{Width: 400, Height: 150},
		Size:    declarative.Size{Width: 400, Height: 150},
		Layout:  declarative.VBox{},
		Children: []declarative.Widget{
			declarative.Label{
				AssignTo: &pf.labelInfo,
				Text:     fmt.Sprintf("0 / %d", total),
			},
			declarative.ProgressBar{
				AssignTo:    &pf.progressBar,
				MinValue:    0,
				MaxValue:    total,
				Value:       0,
				MarqueeMode: false,
			},
			declarative.PushButton{
				AssignTo: &pf.btnCancel,
				Text:     core.TextCancel,
				OnClicked: func() {
					pf.handleCancel()
				},
			},
		},
	}.Create(owner)

	if err != nil {
		// 创建失败, 回滚状态并恢复 owner
		pf.mu.Lock()
		pf.visible = false
		pf.owner = nil
		pf.mu.Unlock()
		if owner != nil {
			owner.SetEnabled(true)
		}
		return err
	}

	// 注册窗体关闭事件 (用户点击叉叉或系统关闭按钮时触发)
	//
	// 行为规则:
	//  1. 若窗体已经处于关闭流程中 (pf.visible == false), 说明是业务层主动调用 Close()
	//     (例如 pool.onComplete), 不应再做任何拦截.
	//  2. 若已处于"等待结束"状态, 拦截关闭 (canceled = true), 防止用户提前关闭窗口.
	//  3. 否则视为用户在压缩进行中点了关闭按钮, 调用 handleCancel 进入等待结束状态,
	//     并设置 canceled = true 让 walk 不立即关闭窗体, 等子任务完成后再统一关闭.
	pf.Dialog.Closing().Attach(func(canceled *bool, reason walk.CloseReason) {
		pf.mu.Lock()
		alreadyClosing := !pf.visible
		stopping := pf.stopping
		pf.mu.Unlock()

		if alreadyClosing {
			// 内部 Close() 触发的关闭, 不干预
			return
		}
		if stopping {
			// 已在等待结束, 禁止关闭
			*canceled = true
			return
		}
		// 进入等待结束状态, 并阻止 walk 直接关闭窗体
		pf.handleCancel()
		*canceled = true
	})

	// 相对于父窗体居中显示
	CenterWindowToOwner(pf.Dialog, owner)

	// 继承主窗体的 TopMost 属性
	// 主窗体置顶时, 进度窗口也置顶, 压缩过程中始终可见
	// 主窗体取消置顶时, 进度窗口同步取消
	ApplyInheritedTopMost(pf.Dialog)

	// 进入"不确定进度"动画模式, 表示任务进行中但尚未确定具体进度
	pf.progressBar.SetMarqueeMode(true)
	pf.Dialog.Show()
	return nil
}

// UpdateProgress 更新进度 (可被任意线程调用, 内部通过 Synchronize 切换到 UI 线程)
func (pf *ProgressForm) UpdateProgress(current, errCount, total int) {
	if pf.progressBar != nil && pf.Dialog != nil {
		pf.Dialog.Synchronize(func() {
			pf.progressBar.SetMarqueeMode(false)
			pf.progressBar.SetValue(current + errCount)
			pf.labelInfo.SetText(fmt.Sprintf("%d / %d (错误: %d)", current+errCount, total, errCount))
		})
	}
}

// Close 关闭窗口 (由 MainForm 在收到 pool 完成回调时调用)
//
// 该方法会被调用多次:
//   - 用户点击取消或叉叉时, handleCancel 内部已经关闭了窗体;
//   - 任务自然完成时, 由 pool 的 onComplete 回调调用.
//
// 因此这里需要做幂等保护.
func (pf *ProgressForm) Close() {
	pf.mu.Lock()
	if !pf.visible {
		pf.mu.Unlock()
		return
	}
	pf.visible = false
	owner := pf.owner
	pf.owner = nil
	onClosed := pf.onClosed
	pf.mu.Unlock()

	// 恢复父窗体的可用状态
	if owner != nil {
		owner.SetEnabled(true)
		// 将焦点切回主窗体, 避免用户看不见的窗体仍持有焦点
		owner.Show()
	}

	// 关闭窗体 (Dialog.Close 需要一个 CloseResult 参数, 这里传 DlgCmdNone 表示外部主动关闭)
	if pf.Dialog != nil {
		pf.Dialog.Synchronize(func() {
			pf.Dialog.Close(walk.DlgCmdNone)
		})
	}

	// 触发关闭完成回调
	if onClosed != nil {
		onClosed()
	}
}

// handleCancel 处理取消请求 (取消按钮或窗体关闭按钮)
//
// 行为:
//   - 第一次调用时, 进入"等待结束"状态: 修改标签文案 + 禁用取消按钮 + 调用 onCancel.
//   - 后续调用 (幂等保护): 直接返回, 不重复触发 onCancel.
//   - 注意: 此方法不再调用 Close(). 窗体的关闭由 pool.onComplete 回调在所有子任务
//     自然结束后统一调用, 以满足"等待当前任务结束"的需求.
//
// 调用方:
//   - 取消按钮 OnClicked
//   - 窗体 Closing 事件 (用户点击叉叉时)
func (pf *ProgressForm) handleCancel() {
	pf.mu.Lock()
	if !pf.visible {
		pf.mu.Unlock()
		return
	}
	if pf.stopping {
		// 已经在等待结束状态, 不重复处理
		pf.mu.Unlock()
		return
	}
	pf.stopping = true
	onCancel := pf.onCancel
	pf.mu.Unlock()

	// 切换 UI 到"等待结束"状态: 修改标签 + 禁用取消按钮
	if pf.Dialog != nil {
		pf.Dialog.Synchronize(func() {
			if pf.btnCancel != nil {
				pf.btnCancel.SetEnabled(false)
				pf.btnCancel.SetText(core.StrWaitingForFinish)
			}
		})
	}

	// 通知外部停止压缩任务 (调用 pool.Stop, 该函数是幂等的)
	if onCancel != nil {
		onCancel()
	}
}
