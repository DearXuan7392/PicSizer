package ui

import (
	"PicSizer/internal/core/setting"
	"PicSizer/internal/fileio"

	"github.com/lxn/walk"
	"github.com/lxn/win"
)

// CenterWindow 将任意窗口居中显示在屏幕中央（已增加高 DPI 分辨率适配）
func CenterWindow(w walk.Form) {
	// 1. 获取窗口当前的逻辑尺寸
	bounds := w.Bounds()

	// 2. 动态获取当前窗口的 DPI（Windows 默认 100% 缩放时为 96）
	dpi := 96 // 设置默认兜底值

	// 通过类型断言尝试将 walk.Form 转换为拥有 DPI() 方法的实现类（如 *walk.MainWindow 或 *walk.Dialog）
	if wnd, ok := w.(interface{ DPI() int }); ok {
		dpi = wnd.DPI()
	}

	// 计算缩放比例
	scale := float64(dpi) / 96.0

	// 3. 将 win API 获取的物理分辨率转换为 walk 所需的逻辑分辨率
	screenWidth := int(float64(win.GetSystemMetrics(win.SM_CXSCREEN)) / scale)
	screenHeight := int(float64(win.GetSystemMetrics(win.SM_CYSCREEN)) / scale)

	// 4. 精准计算居中坐标
	x := (screenWidth - bounds.Width) / 2
	y := (screenHeight - bounds.Height) / 2

	// 5. 设置窗口位置
	w.SetBounds(walk.Rectangle{X: x, Y: y, Width: bounds.Width, Height: bounds.Height})
}

// CenterWindowToOwner 将窗口居中显示在 owner 窗体的中央
// 当 owner 为空时, 回退到屏幕居中
func CenterWindowToOwner(w, owner walk.Form) {
	if owner == nil {
		CenterWindow(w)
		return
	}

	// 获取 owner 的位置和大小, 以及待居中窗口的尺寸
	ownerBounds := owner.Bounds()
	bounds := w.Bounds()

	// 计算相对于 owner 的居中坐标
	x := ownerBounds.X + (ownerBounds.Width-bounds.Width)/2
	y := ownerBounds.Y + (ownerBounds.Height-bounds.Height)/2

	w.SetBounds(walk.Rectangle{X: x, Y: y, Width: bounds.Width, Height: bounds.Height})
}

// ShowOpenImageDialog 显示打开图片文件对话框, 返回选中的文件路径列表
func ShowOpenImageDialog(owner walk.Form) []string {
	dlg := new(walk.FileDialog)
	dlg.Filter = "图片文件|" + fileio.GetSupportedExtensions() + "|所有文件|*.*"
	ok, _ := dlg.ShowOpenMultiple(owner)
	if ok && len(dlg.FilePaths) > 0 {
		return dlg.FilePaths
	}
	return nil
}

// ShowBrowseFolderDialog 显示浏览文件夹对话框, 返回选中的文件夹路径
func ShowBrowseFolderDialog(owner walk.Form, title string) string {
	dlg := new(walk.FileDialog)
	dlg.Title = title
	ok, _ := dlg.ShowBrowseFolder(owner)
	if ok && dlg.FilePath != "" {
		return dlg.FilePath
	}
	return ""
}

// BindEnabledToRadioChecked 将控件的启用状态绑定到单选按钮的选中状态
// 当 radio 被选中时, 控件启用; 否则禁用
func BindEnabledToRadioChecked(radio *walk.RadioButton, controls ...walk.Widget) {
	checked := radio.Checked()
	for _, ctrl := range controls {
		if w, ok := ctrl.(interface{ SetEnabled(bool) }); ok {
			w.SetEnabled(checked)
		}
	}
}

// ApplyTopMostToWindow 设置/取消指定窗口的置顶状态
// 通过 Windows API SetWindowPos 设置 Z-Order, 与主窗口使用同一套机制
// 子窗体 (关于, 设置, 进度) 通过该函数继承主窗体的 TopMost 属性
//
// 参数:
//   - w: 目标窗体, 为 nil 时直接返回 (避免空指针).
//   - topMost: true 表示置顶, false 表示取消置顶.
func ApplyTopMostToWindow(w walk.Form, topMost bool) {
	if w == nil {
		return
	}
	hwnd := w.Handle()
	insertAfter := win.HWND_NOTOPMOST
	if topMost {
		insertAfter = win.HWND_TOPMOST
	}
	win.SetWindowPos(hwnd, insertAfter, 0, 0, 0, 0,
		win.SWP_NOMOVE|win.SWP_NOSIZE|win.SWP_NOACTIVATE)
}

// ApplyInheritedTopMost 让指定窗体继承主窗体的 TopMost 状态
// 从 core.GetSetting().TopMost 读取当前值, 委托给 ApplyTopMostToWindow 执行
// 适用于子窗体的 Show 方法, 在窗体创建后立即调用一次
func ApplyInheritedTopMost(w walk.Form) {
	ApplyTopMostToWindow(w, setting.GetSetting().TopMost)
}
