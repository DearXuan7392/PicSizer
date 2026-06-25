package ui

import (
	"PicSizer/internal/core"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
)

// AboutForm 表示关于窗口，显示应用程序的图标、标题、版本和说明信息。
type AboutForm struct {
	*walk.Dialog
}

// NewAboutForm 创建关于窗口实例。
func NewAboutForm() *AboutForm {
	return &AboutForm{}
}

// Show 显示关于窗口，继承主窗体的 TopMost 属性。
func (af *AboutForm) Show(owner walk.Form, appIcon *walk.Icon) error {
	err := declarative.Dialog{
		AssignTo:  &af.Dialog,
		Title:     core.TitleAbout,
		Icon:      appIcon,
		FixedSize: true,
		MinSize:   declarative.Size{Width: 340, Height: 280},
		MaxSize:   declarative.Size{Width: 340, Height: 280},
		Size:      declarative.Size{Width: 340, Height: 280},
		Layout: declarative.VBox{
			Margins: declarative.Margins{Top: 20, Bottom: 15, Left: 0, Right: 0},
			Spacing: 15,
		},
		Children: []declarative.Widget{
			declarative.Composite{
				Layout: declarative.HBox{MarginsZero: true},
				Children: []declarative.Widget{
					declarative.HSpacer{},
					declarative.ImageView{
						Image:   appIcon,
						Mode:    declarative.ImageViewModeZoom,
						MinSize: declarative.Size{Width: 96, Height: 96},
						MaxSize: declarative.Size{Width: 96, Height: 96},
					},
					declarative.HSpacer{},
				},
			},
			declarative.Composite{
				Layout: declarative.VBox{
					Margins: declarative.Margins{Top: 0, Bottom: 0, Left: 10, Right: 10},
					Spacing: 8,
				},
				Children: []declarative.Widget{
					declarative.Label{
						Text:      core.AboutTitle,
						Alignment: declarative.AlignHCenterVCenter,
						Font:      declarative.Font{PointSize: 16, Bold: true},
					},
					declarative.Label{
						Text:      core.AboutVersion,
						Alignment: declarative.AlignHCenterVCenter,
						Font:      declarative.Font{PointSize: 11},
						TextColor: walk.RGB(100, 100, 100),
					},
					declarative.Label{
						Text:      core.AboutDesc,
						Alignment: declarative.AlignHCenterVCenter,
						Font:      declarative.Font{PointSize: 11},
					},
				},
			},
			declarative.VSpacer{},
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

	ApplyInheritedTopMost(af.Dialog)

	af.Dialog.Run()
	return nil
}
