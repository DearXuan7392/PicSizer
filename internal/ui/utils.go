package ui

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/fileio"

	"github.com/lxn/walk"
	"github.com/lxn/win"
)

// CenterWindow 将窗口居中显示在屏幕中央.
// GetSystemMetrics 返回物理像素, 需除以 DPI 缩放比换算回逻辑坐标.
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

// CenterWindowToOwner 将窗口居中在 owner 中央, owner 为空时回退屏幕居中.
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

func ShowOpenImageDialog(owner walk.Form) []string {
	dlg := new(walk.FileDialog)
	dlg.Filter = "图片文件|" + fileio.GetSupportedExtensions() + "|所有文件|*.*"
	ok, _ := dlg.ShowOpenMultiple(owner)
	if ok && len(dlg.FilePaths) > 0 {
		return dlg.FilePaths
	}
	return nil
}

func ShowBrowseFolderDialog(owner walk.Form, title string) string {
	dlg := new(walk.FileDialog)
	dlg.Title = title
	ok, _ := dlg.ShowBrowseFolder(owner)
	if ok && dlg.FilePath != "" {
		return dlg.FilePath
	}
	return ""
}

// ApplyTopMostToWindow 通过 Windows API SetWindowPos 设置/取消窗口置顶.
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

// ApplyInheritedTopMost 让窗体继承主窗体的 TopMost 状态.
func ApplyInheritedTopMost(w walk.Form) {
	ApplyTopMostToWindow(w, settingLoader.GetSetting().TopMost)
}
