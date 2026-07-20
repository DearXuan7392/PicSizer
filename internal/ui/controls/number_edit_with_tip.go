package controls

import (
	"PicSizer/internal/dialog"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
)

// NumberEditWithTip "文本标签 + (?) 提示链接 + 数字输入框"组合控件.
type NumberEditWithTip struct {
	Label        string
	Tip          string
	AssignTo     **NumberEditWidget
	Value        float64
	MinValue     float64
	MaxValue     float64
	Decimals     int
	Enabled      *bool
	ToolTipText  string
	Width        int
	ParentHwndFn func() uintptr
}

// BuildLabel 构造 Grid 左列: 文本标签 + 提示链接.
func (c *NumberEditWithTip) BuildLabel() declarative.Composite {
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

// BuildInput 构造 Grid 右列: 数字输入框.
func (c *NumberEditWithTip) BuildInput() declarative.Composite {
	width := c.Width
	if width <= 0 {
		width = 200
	}

	var localEdit *NumberEditWidget
	target := c.AssignTo
	if target == nil {
		target = &localEdit
	}

	enabled := true
	if c.Enabled != nil {
		enabled = *c.Enabled
	}

	return declarative.Composite{
		Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
		Children: []declarative.Widget{
			declarative.HSpacer{},
			NumberEdit{
				AssignTo:    target,
				Value:       c.Value,
				MinValue:    c.MinValue,
				MaxValue:    c.MaxValue,
				Decimals:    c.Decimals,
				Enabled:     enabled,
				ToolTipText: c.ToolTipText,
				MinSize:     declarative.Size{Width: width, Height: 0},
				MaxSize:     declarative.Size{Width: width, Height: 0},
			},
		},
	}
}
