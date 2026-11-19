package dialog

import (
	"fmt"
	"sync/atomic"
	"syscall"
	"unsafe"

	"PicSizer/internal/core/strings"
)

// Windows MessageBox 标志位常量
const (
	// 当未指定有效父窗口时, 阻塞当前线程 (任务) 内的所有顶层窗口
	MB_TASKMODAL uint = 0x00002000

	// 使弹窗始终位于最顶层
	MB_TOPMOST uint = 0x00040000

	// 强制将弹窗设置为前台窗口 (获取焦点)
	MB_SETFOREGROUND uint = 0x00010000
)

// dialogBaseFlags 基础标志位: 任务模态 + 置顶 + 抢占焦点,
// 保证弹窗必须被关闭后才能操作其他界面.
const dialogBaseFlags = MB_TASKMODAL | MB_TOPMOST | MB_SETFOREGROUND

// globalParentHwnd 默认弹窗父窗口句柄,
// 仅作未显式指定父窗口时的回退.
var globalParentHwnd uintptr

// SetParentHwnd 设置默认弹窗的父窗口句柄.
// 通常在主窗口创建完成后调用一次.
func SetParentHwnd(hwnd uintptr) {
	atomic.StoreUintptr(&globalParentHwnd, hwnd)
}

// ShowInfo 显示信息弹窗, 标题使用默认主窗口标题.
func ShowInfo(message string) {
	showMessageBox(message, strs.TitleMain, 0x40)
}

// ShowInfoWithTitle 显示信息弹窗, 可指定自定义标题.
func ShowInfoWithTitle(title, message string) {
	if title == "" {
		title = strs.TitleMain
	}
	showMessageBox(message, title, 0x40)
}

// ShowInfoWithTitleAndParent 显示信息弹窗, 可指定标题与父窗口.
// 父窗口为 0 时回退到 SetParentHwnd 设置的全局父窗口.
func ShowInfoWithTitleAndParent(parentHwnd uintptr, title, message string) {
	if title == "" {
		title = strs.TitleMain
	}
	if parentHwnd == 0 {
		parentHwnd = atomic.LoadUintptr(&globalParentHwnd)
	}
	showMessageBoxWithParent(parentHwnd, message, title, 0x40)
}

// ShowWarning 显示警告弹窗.
func ShowWarning(message string) {
	showMessageBox(message, strs.TitleMain, 0x30)
}

// ShowError 显示错误弹窗.
func ShowError(message string) {
	showMessageBox(message, strs.DialogTitleError, 0x10)
}

// ShowErrorWithParent 显示错误弹窗并指定父窗口, 父窗口为 0 时回退到全局父窗口.
func ShowErrorWithParent(parentHwnd uintptr, message string) {
	showMessageBoxWithParent(parentHwnd, message, strs.DialogTitleError, 0x10)
}

// ShowWarningWithParent 显示警告弹窗并指定父窗口, 父窗口为 0 时回退到全局父窗口.
func ShowWarningWithParent(parentHwnd uintptr, message string) {
	showMessageBoxWithParent(parentHwnd, message, strs.TitleMain, 0x30)
}

// ShowConfirm 显示确认弹窗, 返回用户是否点击"确定".
func ShowConfirm(message string) bool {
	ret := showMessageBox(message, strs.TitleMain, 0x1|0x20)
	return ret == 1
}

// ShowConfirmWithParent 显示确认弹窗并指定父窗口, 返回用户是否点击"确定".
func ShowConfirmWithParent(parentHwnd uintptr, message string) bool {
	ret := showMessageBoxWithParent(parentHwnd, message, strs.TitleMain, 0x1|0x20)
	return ret == 1
}

func ShowCompressResult(total, success int) {
	message := fmt.Sprintf(strs.MsgCompressResult, total, success, total-success)
	showMessageBox(message, strs.MsgCompressFinish, 0x40)
}

// showMessageBox 使用 SetParentHwnd 设置的默认父窗口句柄显示弹窗.
func showMessageBox(message, title string, flags uint) int {
	return showMessageBoxWithParent(atomic.LoadUintptr(&globalParentHwnd), message, title, flags)
}

// showMessageBoxWithParent 调用 Windows MessageBoxW 显示弹窗.
func showMessageBoxWithParent(hwnd uintptr, message, title string, flags uint) int {
	user32 := syscall.NewLazyDLL("user32.dll")
	msgBoxW := user32.NewProc("MessageBoxW")

	msgPtr, _ := syscall.UTF16PtrFromString(message)
	titlePtr, _ := syscall.UTF16PtrFromString(title)

	ret, _, _ := msgBoxW.Call(
		hwnd,
		uintptr(unsafe.Pointer(msgPtr)),
		uintptr(unsafe.Pointer(titlePtr)),
		uintptr(flags|dialogBaseFlags),
	)

	return int(ret)
}
