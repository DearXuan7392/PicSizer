package ui

import (
	"fmt"
	"path/filepath"
	"strings"

	"PicSizer/internal/core"
	"PicSizer/internal/core/settingLoader"
	strs "PicSizer/internal/core/strings"
	"PicSizer/internal/fileio"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
)

// dupPolicy 表示输出路径重名时的处置策略.
type dupPolicy int

const (
	dupPolicyOverwrite dupPolicy = iota // 仍然覆盖
	dupPolicySuffix                     // 自动添加后缀
)

// maxDuplicatePreview 为提示框中最多列出的重名路径条数.
const maxDuplicatePreview = 5

// planOutputPaths 在压缩启动前一次性规划全部输出路径.
// 批量内的路径重名只在此刻检测: 存在重名时弹出提示框由用户选择处置策略.
// 返回 false 表示用户放弃本次压缩, 调用方应保持列表状态不变.
func (mf *MainForm) planOutputPaths(items []*core.PicItem) ([]string, bool) {
	set := settingLoader.GetSetting()
	logger.Debug("plan output paths, count: %d, output type: %s, template: %q, start index: %d",
		len(items), settingLoader.OutputTypeName(set.OutputType), set.OutputFilename, set.StartIndex)

	paths := make([]string, len(items))
	for i, item := range items {
		paths[i] = fileio.GetOutputPath(item.FullPath, set.StartIndex+i)
	}

	duplicates := fileio.FindDuplicates(paths)
	if len(duplicates) == 0 {
		logger.Debug("plan output paths done, duplicates: 0")
		return paths, true
	}

	logger.Warn("duplicate output paths detected, groups: %d", len(duplicates))
	for _, d := range duplicates {
		logger.Warn("duplicate output path, path: %s, count: %d", d.Path, d.Count)
	}

	policy, decided := showDuplicateDialog(mf.MainWindow, duplicates, mf.appIcon)
	if !decided {
		logger.Warn("duplicate policy not decided, ask user to abort compress")
		return nil, false
	}
	logger.Info("duplicate policy selected, policy: %s", dupPolicyName(policy))
	if policy == dupPolicySuffix {
		paths = fileio.MakeUniquePaths(paths)
	}
	return paths, true
}

// dupPolicyName 返回重名处置策略的英文名称, 用于日志.
func dupPolicyName(p dupPolicy) string {
	if p == dupPolicySuffix {
		return "suffix"
	}
	return "overwrite"
}

// duplicateMessage 生成提示框正文, 超出的条目以摘要形式给出.
func duplicateMessage(duplicates []fileio.DuplicatePath) string {
	var b strings.Builder
	b.WriteString(fmt.Sprintf(strs.MsgDuplicateHeaderFmt, len(duplicates)))

	for i, d := range duplicates {
		if i == maxDuplicatePreview {
			b.WriteString(fmt.Sprintf(strs.MsgDuplicateMoreFmt, len(duplicates)-maxDuplicatePreview))
			break
		}
		b.WriteString(fmt.Sprintf("\n    %s (%d 张)", filepath.Base(d.Path), d.Count))
	}

	b.WriteString(strs.MsgDuplicateFooter)
	return b.String()
}

// showDuplicateDialog 显示重名提示框, 返回用户选择的策略.
// decided 为 false 表示用户关闭了窗口, 调用方应保持原有策略.
func showDuplicateDialog(parent walk.Form, duplicates []fileio.DuplicatePath, appIcon *walk.Icon) (policy dupPolicy, decided bool) {
	var dlg *walk.Dialog

	err := declarative.Dialog{
		AssignTo:  &dlg,
		Title:     strs.TitleDuplicateOutput,
		Icon:      appIcon,
		FixedSize: true,
		MinSize:   declarative.Size{Width: 470, Height: 250},
		MaxSize:   declarative.Size{Width: 470, Height: 250},
		Size:      declarative.Size{Width: 470, Height: 250},
		Layout:    declarative.VBox{Margins: declarative.Margins{Left: 15, Top: 15, Right: 15, Bottom: 15}, Spacing: 10},
		Children: []declarative.Widget{
			declarative.Label{Text: duplicateMessage(duplicates)},
			declarative.VSpacer{},
			declarative.Composite{
				Layout: declarative.HBox{MarginsZero: true, Spacing: 8},
				Children: []declarative.Widget{
					declarative.HSpacer{},
					declarative.PushButton{
						Text: strs.TextDupOverwrite,
						OnClicked: func() {
							policy, decided = dupPolicyOverwrite, true
							dlg.Accept()
						},
					},
					declarative.PushButton{
						Text: strs.TextDupSuffix,
						OnClicked: func() {
							policy, decided = dupPolicySuffix, true
							dlg.Accept()
						},
					},
					declarative.PushButton{
						Text:      strs.TextCancel,
						OnClicked: func() { dlg.Cancel() },
					},
				},
			},
		},
	}.Create(parent)

	if err != nil {
		return dupPolicyOverwrite, false
	}

	CenterWindowToOwner(dlg, parent)
	ApplyInheritedTopMost(dlg)
	dlg.Run()

	return policy, decided
}
