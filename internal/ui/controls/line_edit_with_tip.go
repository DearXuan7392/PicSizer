package controls

import (
	"PicSizer/internal/dialog"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
)

// LineEditWithTip "文本标签 + (?) 提示链接 + 单行输入框"组合控件.
type LineEditWithTip struct {
	Label        string
	Tip          string
	AssignTo     **walk.LineEdit
	Text         string
	Width        int
	ParentHwndFn func() uintptr
}

// BuildLabel 构造 Grid 左列: 文本标签 + 提示链接.
func (c *LineEditWithTip) BuildLabel() declarative.Composite {
	label := c.Label
	tip := c.Tip
	hwndFn := c.ParentHwndFn

	return declarative.Composite{
		Layout: declarative.HBox{Spacing: 4, MarginsZero: true},
		Children: []declarative.Widget{
			declarative.Label{
				Text: label,
			},
			declarative.LinkLabel{
				Text:        "<a>(?)</a>",
				ToolTipText: tip,
				OnLinkActivated: func(link *walk.LinkLabelLink) {
					var parentHwnd uintptr
					if hwndFn != nil {
						parentHwnd = hwndFn()
					}
					dialog.ShowInfoWithTitleAndParent(parentHwnd, label, tip)
				},
			},
		},
	}
}

// BuildInput 构造 Grid 右列: 单行输入框.
func (c *LineEditWithTip) BuildInput() declarative.Composite {
	width := c.Width
	if width <= 0 {
		width = 200
	}

	var localEdit *walk.LineEdit
	target := c.AssignTo
	if target == nil {
		target = &localEdit
	}

	attached := false

	return declarative.Composite{
		Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
		Children: []declarative.Widget{
			declarative.HSpacer{},
			declarative.LineEdit{
				AssignTo: target,
				Text:     c.Text,
				MinSize:  declarative.Size{Width: width, Height: 0},
				MaxSize:  declarative.Size{Width: width, Height: 0},
				OnBoundsChanged: func() {
					if !attached && target != nil && *target != nil {
						attached = true
						edit := *target
						edit.FocusedChanged().Attach(func() {
							if edit.Focused() {
								if form := edit.Form(); form != nil {
									form.Synchronize(func() {
										edit.SetTextSelection(0, len(edit.Text()))
									})
								}
							}
						})
					}
				},
			},
		},
	}
}
