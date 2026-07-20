package controls

import (
	"PicSizer/internal/dialog"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
)

// CheckBoxWithTip "复选框 + (?) 提示链接"组合控件.
type CheckBoxWithTip struct {
	AssignTo     **walk.CheckBox
	Text         string
	Checked      bool
	Tip          string
	Width        int // 控件宽度, 0 表示使用默认值 200
	ParentHwndFn func() uintptr
}

// Create 实现 declarative.Widget 接口, 构造控件.
func (c CheckBoxWithTip) Create(builder *declarative.Builder) error {
	text := c.Text
	tip := c.Tip
	hwndFn := c.ParentHwndFn
	width := c.Width
	if width <= 0 {
		width = 200
	}

	comp := declarative.Composite{
		Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
		Children: []declarative.Widget{
			declarative.HSpacer{},
			declarative.Composite{
				Layout:  declarative.HBox{Spacing: 4, MarginsZero: true},
				MinSize: declarative.Size{Width: width, Height: 0},
				MaxSize: declarative.Size{Width: width, Height: 0},
				Children: []declarative.Widget{
					declarative.CheckBox{
						AssignTo: c.AssignTo,
						Text:     c.Text,
						Checked:  c.Checked,
					},
					declarative.LinkLabel{
						Text:        "<a>(?)</a>",
						ToolTipText: tip,
						OnLinkActivated: func(link *walk.LinkLabelLink) {
							var parentHwnd uintptr
							if hwndFn != nil {
								parentHwnd = hwndFn()
							}
							dialog.ShowInfoWithTitleAndParent(parentHwnd, text, tip)
						},
					},
					declarative.HSpacer{},
				},
			},
		},
	}
	return comp.Create(builder)
}
