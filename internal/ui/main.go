package ui

import (
	"PicSizer/internal/core"
	"PicSizer/internal/core/setting"
	strs "PicSizer/internal/core/strings"
	"PicSizer/internal/dialog"
	"PicSizer/internal/fileio"
	"PicSizer/internal/server"
	"fmt"
	"sync"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
	"github.com/lxn/win"
)

// MainForm 表示程序的主窗口, 包含图片列表、输出选项和压缩控制等核心交互区域.
type MainForm struct {
	*walk.MainWindow
	picListView   *PicListView
	outputDirEdit *walk.LineEdit
	outputDirBtn  *walk.PushButton
	startBtn      *walk.PushButton
	settingBtn    *walk.PushButton
	coverRadio    *walk.RadioButton
	dirRadio      *walk.RadioButton
	structRadio   *walk.RadioButton
	selectLabel   *walk.Label
	progressForm  *ProgressForm
	settingForm   *SettingForm
	aboutForm     *AboutForm
	appIcon       *walk.Icon
	poolMu        sync.Mutex
	pool          *server.ThreadPool
}

// NewMainForm 创建主窗口实例, 注入子窗体回调.
func NewMainForm() *MainForm {
	mf := &MainForm{
		progressForm: NewProgressForm(),
		settingForm:  NewSettingForm(),
		aboutForm:    NewAboutForm(),
	}
	mf.settingForm.SetOnTopMostChanged(mf.applyTopMost)
	mf.settingForm.SetOnOutputTypeChanged(mf.onOutputTypeFromSetting)
	return mf
}

// Run 创建并运行主窗口, 完成后进入 Windows 消息循环.
func (mf *MainForm) Run(appIcon *walk.Icon) error {
	mf.appIcon = appIcon
	mf.picListView = NewPicListView()

	var err error
	err = declarative.MainWindow{
		AssignTo: &mf.MainWindow,
		Title:    strs.TitleMain,
		Icon:     appIcon,
		MinSize:  declarative.Size{Width: 600, Height: 500},
		Size:     declarative.Size{Width: 600, Height: 500},
		Layout:   declarative.VBox{MarginsZero: false, Margins: declarative.Margins{Left: 10, Top: 10, Right: 10, Bottom: 10}, SpacingZero: false, Spacing: 8},
		OnBoundsChanged: func() {
			if mf.picListView.TableView != nil && mf.picListView.onSelChanged == nil {
				mf.picListView.SetGridlines(true)
				mf.picListView.onSelChanged = mf.updateSelectLabel
			}
		},
		MenuItems: []declarative.MenuItem{
			declarative.Menu{
				Text: strs.TextFile,
				Items: []declarative.MenuItem{
					declarative.Action{Text: strs.TextAddFiles, OnTriggered: mf.onAddFiles},
					declarative.Action{Text: strs.TextAddFolder, OnTriggered: mf.onAddFolder},
					declarative.Separator{},
					declarative.Action{Text: strs.TextExit, OnTriggered: mf.onExit},
				},
			},
			declarative.Menu{
				Text: strs.TextEdit,
				Items: []declarative.MenuItem{
					declarative.Menu{
						Text: strs.TextRemove,
						Items: []declarative.MenuItem{
							declarative.Action{Text: strs.TextRemoveSelect, OnTriggered: mf.onRemoveSelected},
							declarative.Action{Text: strs.TextRemoveDone, OnTriggered: mf.onRemoveDone},
							declarative.Action{Text: strs.TextRemoveError, OnTriggered: mf.onRemoveError},
							declarative.Action{Text: strs.TextRemoveAll, OnTriggered: mf.onRemoveAll},
						},
					},
					declarative.Menu{
						Text: strs.TextSelect,
						Items: []declarative.MenuItem{
							declarative.Action{Text: strs.TextSelectAll, OnTriggered: mf.onSelectAll},
							declarative.Action{Text: strs.TextSelectReverse, OnTriggered: mf.onSelectReverse},
						},
					},
				},
			},
			declarative.Menu{
				Text: strs.TextHelp,
				Items: []declarative.MenuItem{
					declarative.Action{Text: strs.TextAbout, OnTriggered: mf.onAbout},
				},
			},
		},
		Children: []declarative.Widget{
			mf.picListView.PicListViewWidget(),

			declarative.Composite{
				Layout: declarative.HBox{MarginsZero: true, Spacing: 15},
				Children: []declarative.Widget{
					declarative.Composite{
						Layout: declarative.VBox{MarginsZero: true, Spacing: 10},
						Children: []declarative.Widget{
							declarative.Composite{
								Layout: declarative.HBox{MarginsZero: true, Spacing: 10},
								Children: []declarative.Widget{
									declarative.RadioButton{
										AssignTo:  &mf.dirRadio,
										Text:      strs.TextOutputDir,
										OnClicked: mf.onRadioChange,
									},
									declarative.RadioButton{
										AssignTo:  &mf.coverRadio,
										Text:      strs.TextCoverOrigin,
										OnClicked: mf.onRadioChange,
									},
									declarative.RadioButton{
										AssignTo:  &mf.structRadio,
										Text:      strs.TextOutputStruct,
										OnClicked: mf.onRadioChange,
									},
									declarative.HSpacer{},
								},
							},
							declarative.Composite{
								Layout: declarative.HBox{MarginsZero: true, Spacing: 5},
								Children: []declarative.Widget{
									declarative.Label{Text: strs.TextOutputPath},
									declarative.LineEdit{
										AssignTo: &mf.outputDirEdit,
									},
									declarative.PushButton{
										AssignTo:  &mf.outputDirBtn,
										Text:      strs.TextBrowse,
										OnClicked: mf.onBrowseDir,
									},
								},
							},
						},
					},
					declarative.Composite{
						Layout: declarative.VBox{MarginsZero: true, Spacing: 8},
						Children: []declarative.Widget{
							declarative.PushButton{
								AssignTo:  &mf.settingBtn,
								Text:      strs.TextSetting,
								MinSize:   declarative.Size{Width: 95, Height: 0},
								OnClicked: mf.onSetting,
							},
							declarative.PushButton{
								AssignTo:  &mf.startBtn,
								Text:      strs.TextStart,
								MinSize:   declarative.Size{Width: 95, Height: 0},
								OnClicked: mf.onStartCompress,
							},
						},
					},
				},
			},
			declarative.Composite{
				Layout: declarative.HBox{MarginsZero: false, Margins: declarative.Margins{Left: 10, Top: 5, Right: 10, Bottom: 5}},
				Children: []declarative.Widget{
					declarative.Label{
						AssignTo: &mf.selectLabel,
						Text:     "0/0",
					},
					declarative.HSpacer{},
					declarative.Label{Text: strs.AppName + " " + strs.AppVersion},
				},
			},
		},
		OnDropFiles: mf.onDropFiles,
	}.Create()

	if err != nil {
		return err
	}

	dialog.SetParentHwnd(uintptr(mf.MainWindow.Handle()))
	CenterWindow(mf.MainWindow)

	setting := setting.GetSetting()
	mf.applyRadioToOutputType(setting.OutputType)
	mf.onRadioChange()

	mf.MainWindow.Show()
	mf.applyTopMost(setting.TopMost)

	mf.MainWindow.Run()
	return nil
}

// applyTopMost 通过 Windows API 设置或取消主窗口的置顶状态.
func (mf *MainForm) applyTopMost(topMost bool) {
	ApplyTopMostToWindow(mf.MainWindow, topMost)
}

// onAddFiles 打开文件选择对话框, 将选中的图片添加到列表中.
func (mf *MainForm) onAddFiles() {
	paths := ShowOpenImageDialog(mf.MainWindow)
	if len(paths) > 0 {
		mf.picListView.AddPicturesFromPaths(paths)
		mf.updateSelectLabel()
	}
}

// onAddFolder 打开文件夹选择对话框, 将文件夹中的所有图片添加到列表中.
func (mf *MainForm) onAddFolder() {
	dir := ShowBrowseFolderDialog(mf.MainWindow, strs.TextAddFolder)
	if dir != "" {
		mf.picListView.AddPicturesFromDirectory(dir)
		mf.updateSelectLabel()
	}
}

// onDropFiles 处理拖放到主窗口的文件.
// 拖放到输出目录输入框区域时, 若为单个目录则设置为输出路径；否则作为图片添加到列表.
func (mf *MainForm) onDropFiles(files []string) {
	var cursorPos win.POINT
	win.GetCursorPos(&cursorPos)

	if mf.outputDirEdit != nil {
		editHwnd := mf.outputDirEdit.Handle()
		var editRect win.RECT
		win.GetWindowRect(editHwnd, &editRect)

		if cursorPos.X >= editRect.Left && cursorPos.X <= editRect.Right &&
			cursorPos.Y >= editRect.Top && cursorPos.Y <= editRect.Bottom {
			if len(files) == 1 && !mf.coverRadio.Checked() {
				info, err := fileio.GetFileInfo(files[0])
				if err == nil && info.IsDir() {
					mf.outputDirEdit.SetText(files[0])
					return
				}
			}
		}
	}

	var imageFiles []string
	for _, f := range files {
		info, err := fileio.GetFileInfo(f)
		if err != nil {
			continue
		}
		if info.IsDir() {
			mf.picListView.AddPicturesFromDirectory(f)
		} else if fileio.IsImageFile(f) {
			imageFiles = append(imageFiles, f)
		}
	}
	if len(imageFiles) > 0 {
		mf.picListView.AddPicturesFromPaths(imageFiles)
	}
	mf.updateSelectLabel()
}

// onBrowseDir 打开浏览文件夹对话框, 设置输出目录路径.
func (mf *MainForm) onBrowseDir() {
	dir := ShowBrowseFolderDialog(mf.MainWindow, strs.TextOutputDir)
	if dir != "" {
		mf.outputDirEdit.SetText(dir)
	}
}

// onRadioChange 响应输出方式单选按钮切换, 同步更新设置并切换输出目录控件的启用状态.
func (mf *MainForm) onRadioChange() {
	if mf.coverRadio != nil && mf.dirRadio != nil && mf.structRadio != nil {
		if mf.coverRadio.Checked() {
			setting.UpdateSettingWithOutputType(setting.OutputCoverOrigin)
		} else if mf.dirRadio.Checked() {
			setting.UpdateSettingWithOutputType(setting.OutputDirection)
		} else if mf.structRadio.Checked() {
			setting.UpdateSettingWithOutputType(setting.OutputStructure)
		}
	}
	enabled := !mf.coverRadio.Checked()
	mf.outputDirEdit.SetEnabled(enabled)
	mf.outputDirBtn.SetEnabled(enabled)
}

// applyRadioToOutputType 根据 OutputType 同步主窗口的无线电按钮选中状态.
func (mf *MainForm) applyRadioToOutputType(ot setting.OutputType) {
	if mf.coverRadio == nil || mf.dirRadio == nil || mf.structRadio == nil {
		return
	}
	mf.dirRadio.SetChecked(false)
	mf.coverRadio.SetChecked(false)
	mf.structRadio.SetChecked(false)
	switch ot {
	case setting.OutputCoverOrigin:
		mf.coverRadio.SetChecked(true)
	case setting.OutputDirection:
		mf.dirRadio.SetChecked(true)
	case setting.OutputStructure:
		mf.structRadio.SetChecked(true)
	}
}

// onOutputTypeFromSetting 接收设置窗口的输出方式变更通知, 同步主窗口的单选按钮状态.
func (mf *MainForm) onOutputTypeFromSetting(ot setting.OutputType) {
	mf.applyRadioToOutputType(ot)
	mf.onRadioChange()
}

// onStartCompress 启动压缩流程.
// 执行校验、重置状态、创建线程池等步骤, 在独立 goroutine 中异步执行压缩.
func (mf *MainForm) onStartCompress() {
	mf.poolMu.Lock()
	if mf.pool != nil || mf.progressForm.IsVisible() {
		mf.poolMu.Unlock()
		dialog.ShowWarning(strs.WarnCompressRunning)
		return
	}
	mf.poolMu.Unlock()

	allItems := mf.picListView.GetModel().GetItems()
	if len(allItems) == 0 {
		dialog.ShowWarning(strs.ErrNoPictures)
		return
	}

	alreadyDone := mf.picListView.GetModel().CountByState(setting.StateSuccess)
	if alreadyDone > 0 {
		if !dialog.ShowConfirm(fmt.Sprintf(strs.WarnOverwriteDoneFmt, alreadyDone)) {
			return
		}
	}

	if !mf.coverRadio.Checked() {
		core.OutputDirPath = mf.outputDirEdit.Text()
		if core.OutputDirPath == "" {
			dialog.ShowWarning(strs.ErrOutputDirInvalid)
			return
		}
	}

	if mf.coverRadio.Checked() {
	} else if mf.dirRadio.Checked() {
		core.OutputDirPath = mf.outputDirEdit.Text()
		if core.OutputDirPath == "" {
			dialog.ShowWarning(strs.ErrOutputDirInvalid)
			return
		}
	} else {
		var paths []string
		for _, item := range allItems {
			paths = append(paths, item.FullPath)
		}
		prefix, ok := fileio.GetCommonPrefix(paths)
		if !ok {
			dialog.ShowWarning(strs.ErrNoCommonPrefix)
			return
		}
		core.PublicDirPath = prefix
	}

	mf.picListView.StopAutoScrollDown()
	mf.picListView.GetModel().ResetAllToWaiting()
	for _, item := range allItems {
		mf.picListView.GetModel().PublishRowChangedByItem(item)
	}
	mf.picListView.ResetAutoScrollDown()

	mf.progressForm.SetOnCancel(func() {
		mf.poolMu.Lock()
		pool := mf.pool
		mf.poolMu.Unlock()
		if pool != nil {
			pool.Stop()
		}
	})

	mf.progressForm.SetOnClosed(func() {
		mf.poolMu.Lock()
		mf.pool = nil
		mf.poolMu.Unlock()
	})

	if err := mf.progressForm.Show(mf.MainWindow, len(allItems), mf.appIcon); err != nil {
		dialog.ShowError(err.Error())
		return
	}

	pool := server.NewThreadPool(allItems,
		func(current, errCount, total int) {
			mf.progressForm.UpdateProgress(current, errCount, total)
		},
		func(item *core.PicItem) {
			mf.picListView.GetModel().PublishRowChangedByItem(item)
		},
		func(success, errCount, total int) {
			mf.picListView.StopAutoScrollDown()
			mf.progressForm.Close()
			dialog.ShowCompressResult(total, success)
		},
	)

	mf.poolMu.Lock()
	mf.pool = pool
	mf.poolMu.Unlock()

	go func() {
		pool.Start()
		pool.Wait()
	}()
}

// onSetting 打开设置窗口, 关闭后重新应用置顶状态.
func (mf *MainForm) onSetting() {
	mf.settingForm.Show(mf.MainWindow, mf.appIcon)
	setting := setting.GetSetting()
	mf.applyTopMost(setting.TopMost)
}

// onAbout 打开关于窗口.
func (mf *MainForm) onAbout() {
	mf.aboutForm.Show(mf.MainWindow, mf.appIcon)
}

// onRemoveSelected 移除列表中当前选中的项目.
func (mf *MainForm) onRemoveSelected() {
	mf.picListView.GetModel().RemoveSelected(mf.picListView.TableView)
	mf.updateSelectLabel()
}

// onRemoveDone 移除列表中所有已成功完成的项目.
func (mf *MainForm) onRemoveDone() {
	mf.picListView.GetModel().RemoveByState(setting.StateSuccess)
	mf.updateSelectLabel()
}

// onRemoveError 移除列表中所有错误和超出限制的项目.
func (mf *MainForm) onRemoveError() {
	mf.picListView.GetModel().RemoveByState(setting.StateError)
	mf.picListView.GetModel().RemoveByState(setting.StateOutOfLimit)
	mf.updateSelectLabel()
}

// onRemoveAll 清空列表中的所有项目（需用户确认）.
func (mf *MainForm) onRemoveAll() {
	if dialog.ShowConfirm(strs.MsgClearList) {
		mf.picListView.GetModel().Clear()
		mf.updateSelectLabel()
	}
}

// onSelectAll 全选列表中的所有项目.
func (mf *MainForm) onSelectAll() {
	mf.picListView.GetModel().SelectAll(mf.picListView.TableView)
	mf.updateSelectLabel()
}

// onSelectReverse 反选列表中的项目.
func (mf *MainForm) onSelectReverse() {
	mf.picListView.GetModel().SelectReverse(mf.picListView.TableView)
	mf.updateSelectLabel()
}

// onExit 退出程序.
func (mf *MainForm) onExit() {
	mf.MainWindow.Close()
}

// updateSelectLabel 更新底部状态栏的选中/总数标签.
func (mf *MainForm) updateSelectLabel() {
	if mf.selectLabel != nil {
		selected := 0
		if mf.picListView.TableView != nil {
			selected = len(mf.picListView.TableView.SelectedIndexes())
		}
		total := mf.picListView.GetModel().ItemCount()
		mf.selectLabel.SetText(fmt.Sprintf("%d/%d", selected, total))
	}
}
