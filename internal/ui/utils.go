package ui

import (
	"PicSizer/internal/core/setting"
	"PicSizer/internal/fileio"

	"github.com/lxn/walk"
	"github.com/lxn/win"
)

// CenterWindow 将窗口居中显示在屏幕中央, 已适配高 DPI 分辨率.
func CenterWindow(w walk.Form) {
	bounds := w.Bounds()

	dpi := 96
	if wnd, ok := w.(interface{ DPI() int }); ok {
		dpi = wnd.DPI()
	}

	scale := float64(dpi) / 96.0

	screenWidth := int(float64(win.GetSystemMetrics(win.SM_CXSCREEN)) / scale)
	screenHeight := int(float64(win.GetSystemMetrics(win.SM_CYSCREEN)) / scale)

	x := (screenWidth - bounds.Width) / 2
	y := (screenHeight - bounds.Height) / 2

	w.SetBounds(walk.Rectangle{X: x, Y: y, Width: bounds.Width, Height: bounds.Height})
}

// CenterWindowToOwner 将窗口居中显示在 owner 窗体的中央.
// owner 为空时回退到屏幕居中.
func CenterWindowToOwner(w, owner walk.Form) {
	if owner == nil {
		CenterWindow(w)
		return
	}

	ownerBounds := owner.Bounds()
	bounds := w.Bounds()

	x := ownerBounds.X + (ownerBounds.Width-bounds.Width)/2
	y := ownerBounds.Y + (ownerBounds.Height-bounds.Height)/2

	w.SetBounds(walk.Rectangle{X: x, Y: y, Width: bounds.Width, Height: bounds.Height})
}

// ShowOpenImageDialog 显示打开图片文件对话框, 返回选中的文件路径列表.
func ShowOpenImageDialog(owner walk.Form) []string {
	dlg := new(walk.FileDialog)
	dlg.Filter = "图片文件|" + fileio.GetSupportedExtensions() + "|所有文件|*.*"
	ok, _ := dlg.ShowOpenMultiple(owner)
	if ok && len(dlg.FilePaths) > 0 {
		return dlg.FilePaths
	}
	return nil
}

// ShowBrowseFolderDialog 显示浏览文件夹对话框, 返回选中的文件夹路径.
func ShowBrowseFolderDialog(owner walk.Form, title string) string {
	dlg := new(walk.FileDialog)
	dlg.Title = title
	ok, _ := dlg.ShowBrowseFolder(owner)
	if ok && dlg.FilePath != "" {
		return dlg.FilePath
	}
	return ""
}

// BindEnabledToRadioChecked 将一组控件的启用状态绑定到单选按钮的选中状态.
func BindEnabledToRadioChecked(radio *walk.RadioButton, controls ...walk.Widget) {
	checked := radio.Checked()
	for _, ctrl := range controls {
		if w, ok := ctrl.(interface{ SetEnabled(bool) }); ok {
			w.SetEnabled(checked)
		}
	}
}

// ApplyTopMostToWindow 通过 Windows API SetWindowPos 设置/取消指定窗口的置顶状态.
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

// ApplyInheritedTopMost 让指定窗体继承主窗体的 TopMost 状态.
func ApplyInheritedTopMost(w walk.Form) {
	ApplyTopMostToWindow(w, setting.GetSetting().TopMost)
}
