package ui

import (
	"PicSizer/internal/core/setting"

	"github.com/lxn/walk"
)

// Run 启动图形界面模式, 创建主窗口并进入事件循环.
func Run() {
	appIcon, _ := walk.NewIconFromResourceId(2)
	if appIcon == nil {
		appIcon = walk.IconApplication()
	}

	setting.InitSetting()

	mainForm := NewMainForm()
	if err := mainForm.Run(appIcon); err != nil {
		panic(err)
	}
}
