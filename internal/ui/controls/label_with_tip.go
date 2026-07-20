package controls

import (
	"PicSizer/internal/dialog"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
)

// LabelWithTip "文本标签 + (?) 提示链接"组合控件.
type LabelWithTip struct {
	Label        string
	Tip          string
	ParentHwndFn func() uintptr
}

// Create 实现 declarative.Widget 接口, 构造控件.
func (c LabelWithTip) Create(builder *declarative.Builder) error {
	label := c.Label
	tip := c.Tip
	hwndFn := c.ParentHwndFn

	comp := declarative.Composite{
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
	return comp.Create(builder)
}
