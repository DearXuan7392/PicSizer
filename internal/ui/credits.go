package ui

import (
	"os"
	"os/exec"
	"path/filepath"
	"strings"

	strs "PicSizer/internal/core/strings"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
)

// CreditsForm 表示第三方开源协议声明窗口, 使用 WebView 以 HTML 形式展示内容.
type CreditsForm struct {
	*walk.Dialog
	webView *walk.WebView
}

// NewCreditsForm 创建第三方开源协议声明窗口实例.
func NewCreditsForm() *CreditsForm {
	return &CreditsForm{}
}

// Show 显示第三方开源协议声明窗口
func (cf *CreditsForm) Show(owner walk.Form, appIcon *walk.Icon) error {
	err := declarative.Dialog{
		AssignTo:  &cf.Dialog,
		Title:     strs.TitleCredits,
		Icon:      appIcon,
		FixedSize: true,
		MinSize:   declarative.Size{Width: 720, Height: 520},
		MaxSize:   declarative.Size{Width: 720, Height: 520},
		Size:      declarative.Size{Width: 720, Height: 520},
		Layout:    declarative.VBox{Margins: declarative.Margins{Left: 10, Top: 10, Right: 10, Bottom: 10}},
		Children: []declarative.Widget{
			declarative.WebView{
				AssignTo: &cf.webView,
				OnNavigating: func(data *walk.WebViewNavigatingEventData) {
					urlStr := data.Url()
					if strings.HasPrefix(urlStr, "http://") || strings.HasPrefix(urlStr, "https://") {
						data.SetCanceled(true)
						exec.Command("cmd", "/c", "start", "", urlStr).Start() //nolint:errcheck
					}
				},
				OnNewWindow: func(data *walk.WebViewNewWindowEventData) {
					urlStr := data.Url()
					if strings.HasPrefix(urlStr, "http://") || strings.HasPrefix(urlStr, "https://") {
						data.SetCanceled(true)
						exec.Command("cmd", "/c", "start", "", urlStr).Start() //nolint:errcheck
					}
				},
			},
			declarative.Composite{
				Layout: declarative.HBox{MarginsZero: true},
				Children: []declarative.Widget{
					declarative.HSpacer{},
					declarative.PushButton{
						Text: strs.TextOK,
						OnClicked: func() {
							cf.Accept()
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

	// 将 HTML 内容写入临时文件, 然后通过 file:// URL 加载到 WebView 中
	tmpFile, err := os.CreateTemp("", "picsizer_credits_*.html")
	if err != nil {
		return err
	}
	defer os.Remove(tmpFile.Name())

	if _, err := tmpFile.WriteString(strs.CreditsHTML); err != nil {
		tmpFile.Close()
		return err
	}
	tmpFile.Close()

	absPath, err := filepath.Abs(tmpFile.Name())
	if err != nil {
		return err
	}

	// Windows 文件路径转 file:// URL: C:\path -> file:///C:/path
	fileURL := "file:///" + strings.ReplaceAll(absPath, "\\", "/")
	if err := cf.webView.SetURL(fileURL); err != nil {
		return err
	}

	ApplyInheritedTopMost(cf.Dialog)
	CenterWindowToOwner(cf.Dialog, owner)

	cf.Dialog.Run()
	return nil
}
