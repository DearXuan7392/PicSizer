package controls

import (
	"fmt"
	"strconv"
	"strings"
	"time"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
)

// NumberEditWidget 由 NumberEdit 创建的运行时控件包装, 兼容 walk.NumberEdit 的调用方式.
type NumberEditWidget struct {
	*walk.LineEdit
}

// Value 返回当前文本解析为 float64 的数值.
func (n *NumberEditWidget) Value() float64 {
	if n == nil || n.LineEdit == nil {
		return 0
	}
	text := n.Text()
	if text == "" {
		return 0
	}
	val, err := strconv.ParseFloat(text, 64)
	if err != nil {
		return 0
	}
	return val
}

// SetValue 设置文本为指定整数值.
func (n *NumberEditWidget) SetValue(v float64) {
	if n == nil || n.LineEdit == nil {
		return
	}
	n.SetText(fmt.Sprintf("%d", int64(v)))
}

// NumberEdit 用 LineEdit 模拟的声明式控件，仅支持非负整数输入
type NumberEdit struct {
	AssignTo       **NumberEditWidget
	Value          float64
	MinValue       float64
	MaxValue       float64
	Decimals       int // 保留接口兼容，内部不处理小数
	Enabled        bool
	ToolTipText    string
	MinSize        declarative.Size
	MaxSize        declarative.Size
	OnValueChanged walk.EventHandler // 支持数值变更回调
}

// Create 实现了 declarative.Widget 接口
func (ne NumberEdit) Create(builder *declarative.Builder) error {
	initialText := fmt.Sprintf("%d", int64(ne.Value))
	var le *walk.LineEdit

	decl := declarative.LineEdit{
		AssignTo:    &le,
		Text:        initialText,
		Enabled:     ne.Enabled,
		ToolTipText: ne.ToolTipText,
		MinSize:     ne.MinSize,
		MaxSize:     ne.MaxSize,
		OnBoundsChanged: func() {
			if le == nil {
				return
			}

			var lastValidText string = le.Text()

			le.KeyPress().Attach(func(key walk.Key) {
			})

			le.TextChanged().Attach(func() {
				text := le.Text()
				if text == "" {
					lastValidText = ""
					if ne.OnValueChanged != nil {
						ne.OnValueChanged()
					}
					return
				}

				hasInvalid := false
				for _, ch := range text {
					if ch < '0' || ch > '9' {
						hasInvalid = true
						break
					}
				}

				if hasInvalid {
					start, _ := le.TextSelection()
					le.SetText(lastValidText)
					newPos := start - 1
					if newPos < 0 {
						newPos = 0
					}
					if newPos > len(lastValidText) {
						newPos = len(lastValidText)
					}
					le.SetTextSelection(newPos, newPos)
					return
				}

				// 处理前导 0
				cleanText := text
				if len(cleanText) > 1 && strings.HasPrefix(cleanText, "0") {
					// 移除所有前导 0
					cleanText = strings.TrimLeft(cleanText, "0")
					if cleanText == "" {
						cleanText = "0"
					}
				}

				// 如果文本发生了前导 0 清理
				if cleanText != text {
					start, _ := le.TextSelection()
					// 重新设置清理后的文本
					le.SetText(cleanText)

					// 重新计算光标位置：去除多余 0 后，修正光标防跳跃
					newPos := start - (len(text) - len(cleanText))
					if newPos < 0 {
						newPos = 0
					}
					if newPos > len(cleanText) {
						newPos = len(cleanText)
					}
					le.SetTextSelection(newPos, newPos)
					text = cleanText
				}

				// 校验 MaxValue 范围：超出最大值时直接替换为 MaxValue
				if val, err := strconv.ParseInt(text, 10, 64); err == nil {
					if ne.MaxValue > 0 && val > int64(ne.MaxValue) {
						maxStr := fmt.Sprintf("%d", int64(ne.MaxValue))
						le.SetText(maxStr)
						le.SetTextSelection(len(maxStr), len(maxStr)) // 光标移动到末尾
						lastValidText = maxStr
						if ne.OnValueChanged != nil {
							ne.OnValueChanged()
						}
						return
					}
				}

				lastValidText = text

				// 只要文本发生有效改变，触发 OnValueChanged 回调
				if ne.OnValueChanged != nil {
					ne.OnValueChanged()
				}
			})

			// 监听焦点变动：获焦全选；失焦时如果为空或小于 MinValue，直接修正为 MinValue
			le.FocusedChanged().Attach(func() {
				if le.Focused() {
					time.AfterFunc(50*time.Millisecond, func() {
						le.Synchronize(func() {
							le.SetTextSelection(0, len(le.Text()))
						})
					})
				} else {
					text := le.Text()
					val, err := strconv.ParseInt(text, 10, 64)
					minVal := int64(ne.MinValue)

					// 当文本为空、无法解析或数值小于 MinValue 时，修正为 MinValue
					if err != nil || text == "" || val < minVal {
						corrected := fmt.Sprintf("%d", minVal)
						le.SetText(corrected)
						lastValidText = corrected

						if ne.OnValueChanged != nil {
							ne.OnValueChanged()
						}
					}
				}
			})
		},
	}

	err := decl.Create(builder)
	if err != nil {
		return err
	}

	// 将创建的 LineEdit 包装为 NumberEditWidget 并赋值给 AssignTo
	if ne.AssignTo != nil {
		*ne.AssignTo = &NumberEditWidget{LineEdit: le}
	}

	return nil
}
