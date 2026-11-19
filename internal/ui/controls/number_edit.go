package controls

import (
	"fmt"
	"strconv"
	"strings"
	"time"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
)

// NumberEditWidget NumberEdit 创建后的运行时控件包装, 模拟 walk.NumberEdit 的调用方式.
type NumberEditWidget struct {
	*walk.LineEdit
}

// Value 将当前文本解析为 float64, 解析失败或为空时返回 0.
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

func (n *NumberEditWidget) SetValue(v float64) {
	if n == nil || n.LineEdit == nil {
		return
	}
	n.SetText(fmt.Sprintf("%d", int64(v)))
}

// NumberEdit 用 LineEdit 模拟的数字输入控件 (walk 缺 NumberEdit), 仅接受非负整数.
type NumberEdit struct {
	AssignTo       **NumberEditWidget
	Value          float64
	MinValue       float64
	MaxValue       float64
	Decimals       int // 仅保留字段兼容, 内部不处理小数
	Enabled        *bool
	ToolTipText    string
	MinSize        declarative.Size
	MaxSize        declarative.Size
	OnValueChanged walk.EventHandler
}

// Create 实现 declarative.Widget 接口.
func (ne NumberEdit) Create(builder *declarative.Builder) error {
	initialText := fmt.Sprintf("%d", int64(ne.Value))

	// Enabled 为 nil 时默认启用
	enabled := true
	if ne.Enabled != nil {
		enabled = *ne.Enabled
	}

	var le *walk.LineEdit

	decl := declarative.LineEdit{
		AssignTo:    &le,
		Text:        initialText,
		Enabled:     enabled,
		ToolTipText: ne.ToolTipText,
		MinSize:     ne.MinSize,
		MaxSize:     ne.MaxSize,
	}

	if err := decl.Create(builder); err != nil {
		return err
	}

	ne.bindEvents(le)

	if ne.AssignTo != nil {
		*ne.AssignTo = &NumberEditWidget{LineEdit: le}
	}

	return nil
}

// bindEvents 在控件创建完成后挂载输入约束与焦点事件, 每个控件仅挂载一次.
func (ne NumberEdit) bindEvents(le *walk.LineEdit) {
	lastValidText := le.Text()

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

		// 剥离前导 0
		cleanText := text
		if len(cleanText) > 1 && strings.HasPrefix(cleanText, "0") {
			cleanText = strings.TrimLeft(cleanText, "0")
			if cleanText == "" {
				cleanText = "0"
			}
		}

		// 剥离后文本变短, 重设后同步修正光标位置防跳动
		if cleanText != text {
			start, _ := le.TextSelection()
			le.SetText(cleanText)
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

		// 超过 MaxValue 时直接替换为 MaxValue
		if val, err := strconv.ParseInt(text, 10, 64); err == nil {
			if ne.MaxValue > 0 && val > int64(ne.MaxValue) {
				maxStr := fmt.Sprintf("%d", int64(ne.MaxValue))
				le.SetText(maxStr)
				le.SetTextSelection(len(maxStr), len(maxStr))
				lastValidText = maxStr
				if ne.OnValueChanged != nil {
					ne.OnValueChanged()
				}
				return
			}
		}

		lastValidText = text

		if ne.OnValueChanged != nil {
			ne.OnValueChanged()
		}
	})

	// 获焦全选; 失焦时若为空或小于 MinValue 则修正为 MinValue
	le.FocusedChanged().Attach(func() {
		if le.Focused() {
			time.AfterFunc(50*time.Millisecond, func() {
				le.Synchronize(func() {
					le.SetTextSelection(0, len(le.Text()))
				})
			})
			return
		}

		text := le.Text()
		val, err := strconv.ParseInt(text, 10, 64)
		minVal := int64(ne.MinValue)

		// 当文本为空、无法解析或数值小于 MinValue 时, 修正为 MinValue
		if err != nil || text == "" || val < minVal {
			corrected := fmt.Sprintf("%d", minVal)
			le.SetText(corrected)
			lastValidText = corrected

			if ne.OnValueChanged != nil {
				ne.OnValueChanged()
			}
		}
	})
}
