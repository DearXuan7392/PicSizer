package ui

import (
	"fmt"
	"sync"

	"PicSizer/internal/core/strings"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
)

// ProgressForm 表示压缩进度窗口.
// 支持显示进度条、取消压缩以及"等待当前任务结束"状态.
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

// NewProgressForm 创建进度窗口实例.
func NewProgressForm() *ProgressForm {
	return &ProgressForm{}
}

// SetOnCancel 设置取消回调, 当用户点击取消按钮或关闭窗口时触发.
func (pf *ProgressForm) SetOnCancel(fn func()) {
	pf.mu.Lock()
	pf.onCancel = fn
	pf.mu.Unlock()
}

// SetOnClosed 设置关闭完成回调, 当进度窗口彻底关闭后触发.
func (pf *ProgressForm) SetOnClosed(fn func()) {
	pf.mu.Lock()
	pf.onClosed = fn
	pf.mu.Unlock()
}

// IsVisible 返回当前是否处于显示状态, 用于判断压缩任务是否正在进行.
func (pf *ProgressForm) IsVisible() bool {
	pf.mu.Lock()
	defer pf.mu.Unlock()
	return pf.visible
}

// IsStopping 返回是否已进入"等待结束"状态.
func (pf *ProgressForm) IsStopping() bool {
	pf.mu.Lock()
	defer pf.mu.Unlock()
	return pf.stopping
}

// Show 显示进度窗口, 相对于 owner 居中并禁用 owner.
func (pf *ProgressForm) Show(owner walk.Form, total int, appIcon *walk.Icon) error {
	pf.mu.Lock()
	if pf.visible {
		pf.mu.Unlock()
		return fmt.Errorf(strs.ProgressAlreadyVisible)
	}
	pf.visible = true
	pf.owner = owner
	pf.total = total
	pf.stopping = false
	pf.mu.Unlock()

	if owner != nil {
		owner.SetEnabled(false)
	}

	err := declarative.Dialog{
		AssignTo:  &pf.Dialog,
		Title:     strs.TitleProgress,
		Icon:      appIcon,
		FixedSize: true,
		MinSize:   declarative.Size{Width: 400, Height: 150},
		MaxSize:   declarative.Size{Width: 400, Height: 150},
		Size:      declarative.Size{Width: 400, Height: 150},
		Layout:    declarative.VBox{},
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
				Text:     strs.TextCancel,
				OnClicked: func() {
					pf.handleCancel()
				},
			},
		},
	}.Create(owner)

	if err != nil {
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

	CenterWindowToOwner(pf.Dialog, owner)
	ApplyInheritedTopMost(pf.Dialog)

	pf.progressBar.SetMarqueeMode(true)
	pf.Dialog.Show()
	return nil
}

// UpdateProgress 更新进度显示. 可被任意线程调用, 内部通过 Synchronize 切换到 UI 线程.
func (pf *ProgressForm) UpdateProgress(current, errCount, total int) {
	if pf.progressBar != nil && pf.Dialog != nil {
		pf.Dialog.Synchronize(func() {
			pf.progressBar.SetMarqueeMode(false)
			pf.progressBar.SetValue(current + errCount)
			if pf.labelInfo != nil {
				pf.labelInfo.SetText(fmt.Sprintf("%s: %d / %d", strs.TextOK, current+errCount, total))
			}
		})
	}
}

// Close 关闭进度窗口, 恢复 owner 的可用状态.
func (pf *ProgressForm) Close() {
	if pf.Dialog != nil {
		pf.mu.Lock()
		pf.visible = false
		owner := pf.owner
		cb := pf.onClosed
		pf.mu.Unlock()

		pf.Dialog.Close(0)
		if owner != nil {
			owner.SetEnabled(true)
		}
		if cb != nil {
			cb()
		}
	}
}

// handleCancel 处理取消操作, 进入"等待当前任务结束"状态.
func (pf *ProgressForm) handleCancel() {
	pf.mu.Lock()
	if pf.stopping {
		pf.mu.Unlock()
		return
	}
	pf.stopping = true
	cb := pf.onCancel
	pf.mu.Unlock()

	if pf.Dialog != nil {
		pf.Dialog.Synchronize(func() {
			if pf.labelInfo != nil {
				pf.labelInfo.SetText(strs.StrWaitingForFinish)
			}
			if pf.btnCancel != nil {
				pf.btnCancel.SetEnabled(false)
			}
		})
	}

	if cb != nil {
		cb()
	}
}
