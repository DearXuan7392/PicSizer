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

// plannedOutputPaths 按线程池取用任务的顺序, 预先计算当前列表的输出路径.
func (mf *MainForm) plannedOutputPaths() []string {
	return mf.buildOutputPaths(mf.picListView.GetModel().GetItems())
}

// buildOutputPaths 为待压缩项目规划输出路径, 并按当前策略处理重名.
func (mf *MainForm) buildOutputPaths(items []*core.PicItem) []string {
	set := settingLoader.GetSetting()

	paths := make([]string, 0, len(items))
	for i, item := range items {
		paths = append(paths, fileio.GetOutputPath(item.FullPath, set.StartIndex+i))
	}

	if mf.dupPolicy == dupPolicySuffix {
		paths = fileio.MakeUniquePaths(paths)
	}
	return paths
}

// refreshDuplicateCheck 在图片列表或命名规则变化后检查输出路径重名, 必要时提示用户.
// 同一组重名只提示一次, 以免添加图片时反复弹窗.
func (mf *MainForm) refreshDuplicateCheck() {
	if mf.picListView == nil || mf.MainWindow == nil {
		return
	}
	if mf.picListView.GetModel().ItemCount() == 0 {
		mf.lastDuplicateSignature = ""
		return
	}

	duplicates := fileio.FindDuplicates(mf.plannedOutputPaths())
	if len(duplicates) == 0 {
		mf.lastDuplicateSignature = ""
		return
	}

	signature := duplicateSignature(duplicates)
	if signature == mf.lastDuplicateSignature {
		return
	}
	mf.lastDuplicateSignature = signature

	if policy, decided := showDuplicateDialog(mf.MainWindow, duplicates, mf.appIcon); decided {
		mf.dupPolicy = policy
	}
}

// duplicateSignature 生成用于判断重名情况是否已提示过的标识.
func duplicateSignature(duplicates []fileio.DuplicatePath) string {
	var b strings.Builder
	for _, d := range duplicates {
		fmt.Fprintf(&b, "%s|%d;", strings.ToLower(d.Path), d.Count)
	}
	return b.String()
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
