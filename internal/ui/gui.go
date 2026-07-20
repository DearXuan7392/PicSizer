package ui

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/log"

	"github.com/lxn/walk"
)

var (
	logger = log.NewLogger("gui")
)

// Run 启动图形界面模式, 创建主窗口并进入事件循环.
// startupPaths 为通过拖拽文件到程序图标传入的路径, 会在窗口创建后自动加载.
func Run(startupPaths []string) {
	appIcon, _ := walk.NewIconFromResourceId(2)
	if appIcon == nil {
		logger.Warn("load app icon failed, use default icon instead")
		appIcon = walk.IconApplication()
	}

	settingLoader.InitSetting()

	mainForm := NewMainForm(startupPaths)
	if err := mainForm.Run(appIcon); err != nil {
		logger.Error("run gui failed: %s", err)
		panic(err)
	}
}
