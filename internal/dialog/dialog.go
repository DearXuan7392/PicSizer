package dialog

import (
	"fmt"
	"sync/atomic"
	"syscall"
	"unsafe"

	"PicSizer/internal/core"
)

// 弹窗管理模块 - 统一负责所有弹窗功能

// Windows MessageBox 标志位常量
const (
	// MB_TASKMODAL 当未指定有效父窗口时, 阻塞当前线程(任务)中的所有顶层窗口
	MB_TASKMODAL uint = 0x00002000
	// MB_TOPMOST 使弹窗始终位于最顶层
	MB_TOPMOST uint = 0x00040000
	// MB_SETFOREGROUND 强制将弹窗设置为前台窗口(获取焦点)
	MB_SETFOREGROUND uint = 0x00010000
)

// dialogBaseFlags 弹窗基础标志位
// 任务模态 + 始终置顶 + 强制获取焦点
// 确保弹窗显示时强制获得焦点, 必须关闭该弹窗后才能操作其它界面
const dialogBaseFlags uint = MB_TASKMODAL | MB_TOPMOST | MB_SETFOREGROUND

// globalParentHwnd 默认弹窗父窗口句柄
// 使用原子操作保证并发安全
// 在 UI 主窗口创建后由 SetParentHwnd 设置
// 仅作为未显式指定父窗口时的回退, 大多数场景下调用方应显式传入
var globalParentHwnd uintptr

// SetParentHwnd 设置默认弹窗父窗口句柄
// 传入 0 表示无父窗口(恢复默认行为)
// 该函数通常在主窗口创建完成后调用一次
func SetParentHwnd(hwnd uintptr) {
	atomic.StoreUintptr(&globalParentHwnd, hwnd)
}

// ShowInfo 显示信息弹窗
func ShowInfo(message string) {
	showMessageBox(message, core.TitleMain, 0x40) // MB_ICONINFORMATION
}

// ShowInfoWithTitle 显示信息弹窗, 可指定自定义标题
// 用于设置界面中 (?) 提示链接, 标题使用对应 Label 的文本
// 使弹窗标题与控件名对应, 方便用户理解当前查看的是哪个设置的说明
// 父窗口使用 SetParentHwnd 全局设置的句柄 (默认为主窗口)
func ShowInfoWithTitle(title, message string) {
	// 标题为空时回退到默认主窗口标题
	if title == "" {
		title = core.TitleMain
	}
	showMessageBox(message, title, 0x40) // MB_ICONINFORMATION
}

// ShowInfoWithTitleAndParent 显示信息弹窗, 可同时指定自定义标题和父窗口句柄
// 用于子窗体 (如设置窗口) 中的 (?) 提示链接
// parentHwnd 通常传入当前触发弹窗的控件所在窗体的句柄 (例如设置窗口)
// 传入 0 时回退到 SetParentHwnd 设置的全局父窗口句柄
// 配合 MB_TASKMODAL 标志位, 弹窗在父窗口所在线程中任务模态:
//   - 必须先关闭弹窗才能继续操作父窗体
//   - 同时 MB_TOPMOST + MB_SETFOREGROUND 保证弹窗置顶并获取焦点
func ShowInfoWithTitleAndParent(parentHwnd uintptr, title, message string) {
	// 标题为空时回退到默认主窗口标题
	if title == "" {
		title = core.TitleMain
	}
	// 父窗口为 0 时回退到全局设置的句柄 (默认主窗口)
	if parentHwnd == 0 {
		parentHwnd = atomic.LoadUintptr(&globalParentHwnd)
	}
	showMessageBoxWithParent(parentHwnd, message, title, 0x40) // MB_ICONINFORMATION
}

// ShowWarning 显示警告弹窗
func ShowWarning(message string) {
	showMessageBox(message, core.TitleMain, 0x30) // MB_ICONWARNING
}

// ShowError 显示错误弹窗
func ShowError(message string) {
	showMessageBox(message, "错误", 0x10) // MB_ICONERROR
}

// ShowErrorWithParent 显示错误弹窗, 并指定父窗口句柄
// 用于子窗体 (如设置窗口) 中的错误提示, 弹窗强制模态于该父窗体
// parentHwnd 通常传入当前触发弹窗的控件所在窗体的句柄 (例如设置窗口)
// 配合 MB_TASKMODAL 标志位, 弹窗在父窗口所在线程中任务模态:
//   - 必须先关闭弹窗才能继续操作父窗体
//   - 同时 MB_TOPMOST + MB_SETFOREGROUND 保证弹窗置顶并获取焦点
//
// 父窗口为 0 时回退到 SetParentHwnd 设置的全局父窗口句柄
func ShowErrorWithParent(parentHwnd uintptr, message string) {
	showMessageBoxWithParent(parentHwnd, message, "错误", 0x10) // MB_ICONERROR
}

// ShowWarningWithParent 显示警告弹窗, 并指定父窗口句柄
// 用于子窗体 (如设置窗口) 中的警告提示, 弹窗强制模态于该父窗体
// parentHwnd 通常传入当前触发弹窗的控件所在窗体的句柄 (例如设置窗口)
// 配合 MB_TASKMODAL 标志位, 弹窗在父窗口所在线程中任务模态
// 父窗口为 0 时回退到 SetParentHwnd 设置的全局父窗口句柄
func ShowWarningWithParent(parentHwnd uintptr, message string) {
	showMessageBoxWithParent(parentHwnd, message, core.TitleMain, 0x30) // MB_ICONWARNING
}

// ShowConfirm 显示确认弹窗, 返回用户选择
func ShowConfirm(message string) bool {
	ret := showMessageBox(message, core.TitleMain, 0x1|0x20) // MB_OKCANCEL | MB_ICONQUESTION
	return ret == 1                                          // IDOK
}

// ShowConfirmWithParent 显示确认弹窗, 并指定父窗口句柄, 返回用户选择
// 用于子窗体 (如设置窗口) 中的二次确认场景, 弹窗强制模态于该父窗体
// 返回 true 表示用户点击了"确定", false 表示用户点击了"取消"或关闭了弹窗
// parentHwnd 通常传入当前触发弹窗的控件所在窗体的句柄 (例如设置窗口)
// 配合 MB_TASKMODAL 标志位, 弹窗在父窗口所在线程中任务模态
// 父窗口为 0 时回退到 SetParentHwnd 设置的全局父窗口句柄
func ShowConfirmWithParent(parentHwnd uintptr, message string) bool {
	ret := showMessageBoxWithParent(parentHwnd, message, core.TitleMain, 0x1|0x20) // MB_OKCANCEL | MB_ICONQUESTION
	return ret == 1                                                                // IDOK
}

// ShowCompressResult 显示压缩完成结果
func ShowCompressResult(total, success int) {
	message := fmt.Sprintf(core.MsgCompressResult, total, success, total-success)
	showMessageBox(message, core.MsgCompressFinish, 0x40)
}

// showMessageBox 调用 Windows MessageBox API
// 使用 SetParentHwnd 设置的默认父窗口句柄
// 默认叠加 MB_TASKMODAL | MB_TOPMOST | MB_SETFOREGROUND 标志位
// 确保弹窗为任务模态, 强制置顶并获取焦点, 必须关闭后才能操作其它界面
func showMessageBox(message, title string, flags uint) int {
	return showMessageBoxWithParent(atomic.LoadUintptr(&globalParentHwnd), message, title, flags)
}

// showMessageBoxWithParent 调用 Windows MessageBox API
// 显式指定父窗口句柄 hwnd, 通常传入触发弹窗的控件所在窗体
// (例如设置窗口的句柄), 使弹窗强制模态于该父窗体:
//   - 弹窗关闭前无法切换到该父窗体
//   - 弹窗关闭前无法操作该父窗体的子控件
//
// 叠加 MB_TASKMODAL | MB_TOPMOST | MB_SETFOREGROUND 标志位
// 确保弹窗为任务模态, 强制置顶并获取焦点
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
