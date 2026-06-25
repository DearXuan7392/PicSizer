package ui

import (
	"PicSizer/internal/core/setting"

	"github.com/lxn/walk"
)

// runGUI 运行图形界面模式
func Run() {
	// 从资源文件加载图标
	appIcon, _ := walk.NewIconFromResourceId(2)
	if appIcon == nil {
		appIcon = walk.IconApplication() // 保底默认图标
	}

	// 初始化配置
	setting.InitSetting()

	mainForm := NewMainForm()
	if err := mainForm.Run(appIcon); err != nil {
		panic(err)
	}
}
