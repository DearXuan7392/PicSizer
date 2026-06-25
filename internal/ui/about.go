package ui

import (
	"PicSizer/internal/core"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
)

// AboutForm 关于窗口
type AboutForm struct {
	*walk.Dialog
}

// NewAboutForm 创建关于窗口
func NewAboutForm() *AboutForm {
	return &AboutForm{}
}

// Show 显示关于窗口
func (af *AboutForm) Show(owner walk.Form, appIcon *walk.Icon) error {
	err := declarative.Dialog{
		AssignTo:  &af.Dialog,
		Title:     core.TitleAbout,
		Icon:      appIcon,
		FixedSize: true,
		// 窗体大小，防止放大的图标和文字显得拥挤
		MinSize: declarative.Size{Width: 340, Height: 280},
		MaxSize: declarative.Size{Width: 340, Height: 280},
		Size:    declarative.Size{Width: 340, Height: 280},
		Layout: declarative.VBox{
			// Top: 20 控制了整个布局内容距离窗口顶部的距离
			Margins: declarative.Margins{Top: 20, Bottom: 15, Left: 0, Right: 0},
			Spacing: 15,
		},
		Children: []declarative.Widget{
			// 1. 图标区域
			declarative.Composite{
				Layout: declarative.HBox{MarginsZero: true},
				Children: []declarative.Widget{
					declarative.HSpacer{},
					declarative.ImageView{
						Image: appIcon,
						// 设置缩放模式，让图标画面跟随控件一起放大
						Mode: declarative.ImageViewModeZoom,
						// =================【如何调整图标大小】=================
						// 如果觉得 64x64 依然小，可以直接修改下方的 Width 和 Height 数值。
						// 例如改为 Width: 80, Height: 80。
						// 提示：修改时请保持 MinSize 和 MaxSize 的数值一致。
						MinSize: declarative.Size{Width: 96, Height: 96},
						MaxSize: declarative.Size{Width: 96, Height: 96},
						// ====================================================
					},
					declarative.HSpacer{},
				},
			},

			// 2. 文本区域：使用嵌套 Composite 来精准控制文字的左右 Padding
			declarative.Composite{
				Layout: declarative.VBox{
					// Left: 10, Right: 10 实现了文字距离窗体边缘 padding 为 10 的要求
					Margins: declarative.Margins{Top: 0, Bottom: 0, Left: 10, Right: 10},
					Spacing: 8,
				},
				Children: []declarative.Widget{
					// 程序标题
					declarative.Label{
						Text:      core.AboutTitle,
						Alignment: declarative.AlignHCenterVCenter,
						Font: declarative.Font{
							PointSize: 16, // 放大标题字体
							Bold:      true,
						},
					},
					// 版本号
					declarative.Label{
						Text:      core.AboutVersion,
						Alignment: declarative.AlignHCenterVCenter,
						Font: declarative.Font{
							PointSize: 11, // 放大版本号字体
						},
						TextColor: walk.RGB(100, 100, 100),
					},
					// 多行介绍文本
					// 修正：移除了非法的 MultiLine 字段，Label 默认在空间不足或有\n时会自动换行
					declarative.Label{
						Text:      core.AboutDesc,
						Alignment: declarative.AlignHCenterVCenter, // 使多行文本整体在水平方向上居中对齐
						Font: declarative.Font{
							PointSize: 11, // 放大介绍文字的字体
						},
					},
				},
			},

			// 垂直弹簧，把按钮顶到最下方
			declarative.VSpacer{},

			// 3. 确定按钮区域（让按钮两边也留出 padding 保持美观）
			declarative.Composite{
				Layout: declarative.HBox{
					Margins: declarative.Margins{Left: 10, Right: 10},
				},
				Children: []declarative.Widget{
					declarative.HSpacer{},
					declarative.PushButton{
						Text: core.TextOK,
						OnClicked: func() {
							af.Accept()
						},
					},
					declarative.HSpacer{},
				},
			},
		},
	}.Create(owner)

	if err != nil {
		return err
	}

	// 继承主窗体的 TopMost 属性
	// 主窗体置顶时, 关于窗口也置顶; 主窗体取消置顶时, 关于窗口同步取消
	ApplyInheritedTopMost(af.Dialog)

	// Run() 阻塞直到窗体关闭, 返回 dialog command (int), 忽略返回值
	af.Dialog.Run()
	return nil
}
