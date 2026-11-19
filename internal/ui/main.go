package ui

import (
	"PicSizer/internal/core"
	"PicSizer/internal/core/settingLoader"
	strs "PicSizer/internal/core/strings"
	"PicSizer/internal/dialog"
	"PicSizer/internal/fileio"
	"PicSizer/internal/server"
	"fmt"
	"os"
	"path/filepath"
	"sync"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
	"github.com/lxn/win"
)

// MainForm 主窗口: 图片列表、输出方式与压缩控制.
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
	startupPaths  []string

	// outputPathUserModified 记录用户是否主动修改过输出路径.
	// 一旦为 true, 不再自动计算输出路径.
	outputPathUserModified bool
	// currentCommonDir 当前所有图片的公共父目录 (未手动修改输出路径时使用).
	currentCommonDir string
	// suppressPathUpdate 程序主动 SetText 时抑制 TextChanged 触发.
	suppressPathUpdate bool
}

// NewMainForm 创建主窗口实例, 注入子窗体回调.
// startupPaths 为通过拖拽文件/文件夹到程序图标传入的路径, 窗口创建后自动加载.
func NewMainForm(startupPaths []string) *MainForm {
	mf := &MainForm{
		progressForm: NewProgressForm(),
		settingForm:  NewSettingForm(),
		aboutForm:    NewAboutForm(),
		startupPaths: startupPaths,
	}
	mf.settingForm.SetOnTopMostChanged(mf.applyTopMost)
	mf.settingForm.SetOnOutputTypeChanged(mf.onOutputTypeFromSetting)
	return mf
}

// Run 创建并运行主窗口.
func (mf *MainForm) Run(appIcon *walk.Icon) error {
	mf.appIcon = appIcon
	mf.picListView = NewPicListView(appIcon)

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
				mf.picListView.updateContextMenuState()
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

	setting := settingLoader.GetSetting()
	mf.applyRadioToOutputType(setting.OutputType)
	mf.onRadioChange()

	mf.initOutputDirEdit()

	mf.MainWindow.Show()
	mf.applyTopMost(setting.TopMost)

	mf.processStartupPaths()
	mf.updateOutputPathFromCommonDir()

	mf.MainWindow.Run()
	return nil
}

func (mf *MainForm) applyTopMost(topMost bool) {
	ApplyTopMostToWindow(mf.MainWindow, topMost)
}

func (mf *MainForm) onAddFiles() {
	paths := ShowOpenImageDialog(mf.MainWindow)
	if len(paths) == 0 {
		logger.Debug("add files cancelled by user")
		return
	}
	logger.Info("add files, selected: %d", len(paths))
	mf.picListView.AddPicturesFromPaths(paths)
	mf.updateSelectLabel()
	mf.updateOutputPathFromCommonDir()
	logger.Debug("add files done, total: %d", mf.picListView.GetModel().ItemCount())
}

func (mf *MainForm) onAddFolder() {
	dir := ShowBrowseFolderDialog(mf.MainWindow, strs.TextAddFolder)
	if dir == "" {
		logger.Debug("add folder cancelled by user")
		return
	}
	logger.Info("add folder, dir: %s", dir)
	mf.picListView.AddPicturesFromDirectory(dir)
	mf.updateSelectLabel()
	mf.updateOutputPathFromCommonDir()
	logger.Debug("add folder done, total: %d", mf.picListView.GetModel().ItemCount())
}

// onDropFiles 处理拖放文件.
// 落在输出目录输入框上且为单个目录时设为输出路径; 否则作为图片加入列表.
func (mf *MainForm) onDropFiles(files []string) {
	logger.Info("drop files, count: %d", len(files))

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
					mf.suppressPathUpdate = true
					mf.outputDirEdit.SetText(files[0])
					mf.suppressPathUpdate = false
					mf.outputPathUserModified = true
					mf.updateOutputDirFont()
					logger.Info("set output dir by drop, dir: %s", files[0])
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
	mf.updateOutputPathFromCommonDir()
	logger.Debug("drop files done, total: %d", mf.picListView.GetModel().ItemCount())
}

func (mf *MainForm) onBrowseDir() {
	dir := ShowBrowseFolderDialog(mf.MainWindow, strs.TextOutputDir)
	if dir == "" {
		logger.Debug("browse output dir cancelled by user")
		return
	}
	logger.Info("browse output dir, dir: %s", dir)
	mf.suppressPathUpdate = true
	mf.outputDirEdit.SetText(dir)
	mf.suppressPathUpdate = false
	mf.outputPathUserModified = true
	mf.updateOutputDirFont()
}

func (mf *MainForm) onRadioChange() {
	if mf.coverRadio != nil && mf.dirRadio != nil && mf.structRadio != nil {
		if mf.coverRadio.Checked() {
			settingLoader.UpdateSettingWithOutputType(settingLoader.OutputCoverOrigin)
		} else if mf.dirRadio.Checked() {
			settingLoader.UpdateSettingWithOutputType(settingLoader.OutputDirection)
		} else if mf.structRadio.Checked() {
			settingLoader.UpdateSettingWithOutputType(settingLoader.OutputStructure)
		}
	}
	enabled := !mf.coverRadio.Checked()
	mf.outputDirEdit.SetEnabled(enabled)
	mf.outputDirBtn.SetEnabled(enabled)

	logger.Debug("output type changed, type: %s", settingLoader.OutputTypeName(settingLoader.GetSetting().OutputType))
}

func (mf *MainForm) applyRadioToOutputType(ot settingLoader.OutputType) {
	if mf.coverRadio == nil || mf.dirRadio == nil || mf.structRadio == nil {
		return
	}
	mf.dirRadio.SetChecked(false)
	mf.coverRadio.SetChecked(false)
	mf.structRadio.SetChecked(false)
	switch ot {
	case settingLoader.OutputCoverOrigin:
		mf.coverRadio.SetChecked(true)
	case settingLoader.OutputDirection:
		mf.dirRadio.SetChecked(true)
	case settingLoader.OutputStructure:
		mf.structRadio.SetChecked(true)
	}
}

func (mf *MainForm) onOutputTypeFromSetting(ot settingLoader.OutputType) {
	mf.applyRadioToOutputType(ot)
	mf.onRadioChange()
}

// onStartCompress 启动压缩流程.
// 执行校验、重置状态、创建线程池等步骤, 在独立 goroutine 中异步执行压缩.
func (mf *MainForm) onStartCompress() {
	logger.Info("start compress, items: %d", mf.picListView.GetModel().ItemCount())

	mf.poolMu.Lock()
	if mf.pool != nil || mf.progressForm.IsVisible() {
		mf.poolMu.Unlock()
		logger.Warn("start compress rejected, task already running")
		dialog.ShowWarning(strs.WarnCompressRunning)
		return
	}
	mf.poolMu.Unlock()

	allItems := mf.picListView.GetModel().GetItems()
	if len(allItems) == 0 {
		logger.Warn("start compress rejected, list is empty")
		dialog.ShowWarning(strs.ErrNoPictures)
		return
	}

	alreadyDone := mf.picListView.GetModel().CountByState(settingLoader.StateSuccess)
	if alreadyDone > 0 {
		confirmed := dialog.ShowConfirm(fmt.Sprintf(strs.WarnOverwriteDoneFmt, alreadyDone))
		logger.Info("confirm overwrite finished items, count: %d, accepted: %t", alreadyDone, confirmed)
		if !confirmed {
			return
		}
	}

	if !mf.coverRadio.Checked() {
		core.OutputDirPath = mf.outputDirEdit.Text()
		if core.OutputDirPath == "" {
			logger.Warn("start compress rejected, output dir is empty")
			dialog.ShowWarning(strs.ErrOutputDirInvalid)
			return
		}
	}

	if mf.coverRadio.Checked() {
		logger.Info("output mode, type: cover origin")
	} else if mf.dirRadio.Checked() {
		core.OutputDirPath = mf.outputDirEdit.Text()
		if core.OutputDirPath == "" {
			logger.Warn("start compress rejected, output dir is empty")
			dialog.ShowWarning(strs.ErrOutputDirInvalid)
			return
		}
		logger.Info("output mode, type: direction, dir: %s", core.OutputDirPath)
	} else {
		var paths []string
		for _, item := range allItems {
			paths = append(paths, item.FullPath)
		}
		prefix, ok := fileio.GetCommonPrefix(paths)
		if !ok {
			logger.Warn("start compress rejected, no common prefix among input files")
			dialog.ShowWarning(strs.ErrNoCommonPrefix)
			return
		}
		core.PublicDirPath = prefix
		logger.Info("output mode, type: structure, public dir: %s, dir: %s", prefix, core.OutputDirPath)
	}

	// 输出路径在启动前一次性规划完成, 线程池只按下标取用.
	// 批量内的重名检测同样推迟到此刻, 用户取消则放弃本次压缩.
	outputPaths, planned := mf.planOutputPaths(allItems)
	if !planned {
		logger.Warn("start compress aborted, output path planning cancelled")
		return
	}

	mf.picListView.StopAutoScrollDown()
	mf.picListView.GetModel().ResetAllToWaiting()
	for i, item := range allItems {
		item.OutputPath = outputPaths[i]
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
		logger.Error("show progress form failed, err: %v", err)
		dialog.ShowError(err.Error())
		return
	}

	logSettings(settingLoader.GetSetting())

	pool := server.NewThreadPool(allItems, outputPaths,
		func(current, errCount, total int) {
			mf.progressForm.UpdateProgress(current, errCount, total)
		},
		func(item *core.PicItem) {
			mf.picListView.GetModel().PublishRowChangedByItem(item)
		},
		func(success, errCount, total int) {
			logger.Info("compress finished, success: %d, failed: %d, total: %d", success, errCount, total)
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

func (mf *MainForm) onSetting() {
	logger.Info("open setting dialog")
	mf.settingForm.Show(mf.MainWindow, mf.appIcon)
	setting := settingLoader.GetSetting()
	mf.applyTopMost(setting.TopMost)
	logger.Debug("setting dialog closed, top most: %t", setting.TopMost)
}

func (mf *MainForm) onAbout() {
	logger.Debug("open about dialog")
	mf.aboutForm.Show(mf.MainWindow, mf.appIcon)
}

func (mf *MainForm) onRemoveSelected() {
	model := mf.picListView.GetModel()
	count := len(mf.picListView.TableView.SelectedIndexes())
	model.RemoveSelected(mf.picListView.TableView)
	mf.updateSelectLabel()
	logger.Info("remove selected, count: %d, remain: %d", count, model.ItemCount())
}

func (mf *MainForm) onRemoveDone() {
	model := mf.picListView.GetModel()
	removed := model.CountByState(settingLoader.StateSuccess)
	model.RemoveByState(settingLoader.StateSuccess)
	mf.updateSelectLabel()
	logger.Info("remove finished, count: %d, remain: %d", removed, model.ItemCount())
}

func (mf *MainForm) onRemoveError() {
	model := mf.picListView.GetModel()
	removed := model.CountByState(settingLoader.StateError) + model.CountByState(settingLoader.StateOutOfLimit)
	model.RemoveByState(settingLoader.StateError)
	model.RemoveByState(settingLoader.StateOutOfLimit)
	mf.updateSelectLabel()
	logger.Info("remove failed, count: %d, remain: %d", removed, model.ItemCount())
}

func (mf *MainForm) onRemoveAll() {
	confirmed := dialog.ShowConfirm(strs.MsgClearList)
	logger.Info("clear list, confirmed: %t, count: %d", confirmed, mf.picListView.GetModel().ItemCount())
	if confirmed {
		mf.picListView.GetModel().Clear()
		mf.updateSelectLabel()
	}
}

func (mf *MainForm) onSelectAll() {
	mf.picListView.GetModel().SelectAll(mf.picListView.TableView)
	mf.updateSelectLabel()
	logger.Debug("select all, count: %d", mf.picListView.GetModel().ItemCount())
}

func (mf *MainForm) onSelectReverse() {
	mf.picListView.GetModel().SelectReverse(mf.picListView.TableView)
	mf.updateSelectLabel()
	logger.Debug("select reverse, selected: %d", len(mf.picListView.TableView.SelectedIndexes()))
}

func (mf *MainForm) onExit() {
	logger.Info("exit requested by user")
	mf.MainWindow.Close()
}

func (mf *MainForm) initOutputDirEdit() {
	mf.outputDirEdit.TextChanged().Attach(func() {
		if mf.suppressPathUpdate {
			return
		}
		mf.outputPathUserModified = true
		mf.updateOutputDirFont()
	})
	mf.updateOutputDirFont()
}

// updateOutputDirFont 输出路径的字体样式: 自动生成时斜体灰色, 手动修改后正体黑色.
func (mf *MainForm) updateOutputDirFont() {
	if mf.outputDirEdit == nil {
		return
	}
	if mf.outputPathUserModified {
		f, _ := walk.NewFont("", 9, 0)
		mf.outputDirEdit.SetFont(f)
		mf.outputDirEdit.SetTextColor(walk.RGB(0, 0, 0))
	} else {
		f, _ := walk.NewFont("", 9, walk.FontItalic)
		mf.outputDirEdit.SetFont(f)
		mf.outputDirEdit.SetTextColor(walk.RGB(128, 128, 128))
	}
}

// updateOutputPathFromCommonDir 根据当前列表的公共父目录自动生成输出目录.
// 仅在用户未手动修改输出路径时生效.
func (mf *MainForm) updateOutputPathFromCommonDir() {
	if mf.outputPathUserModified || mf.picListView == nil {
		return
	}
	items := mf.picListView.GetModel().GetItems()
	if len(items) == 0 {
		return
	}
	var paths []string
	for _, item := range items {
		paths = append(paths, item.FullPath)
	}
	prefix, ok := fileio.GetCommonPrefix(paths)
	if !ok {
		return
	}
	mf.currentCommonDir = prefix

	baseDir := filepath.Join(prefix, "PicSizerOutput")
	outputDir := findAvailableOutputDir(baseDir)

	mf.suppressPathUpdate = true
	mf.outputDirEdit.SetText(outputDir)
	mf.suppressPathUpdate = false
}

// findAvailableOutputDir 优先使用 baseDir, 若已存在且非空则依次尝试 baseDir2, baseDir3...
func findAvailableOutputDir(baseDir string) string {
	if !pathExists(baseDir) || isDirEmpty(baseDir) {
		return baseDir
	}
	for i := 2; i <= 100; i++ {
		d := fmt.Sprintf("%s%d", baseDir, i)
		if !pathExists(d) || isDirEmpty(d) {
			return d
		}
	}
	return baseDir
}

func pathExists(path string) bool {
	_, err := os.Stat(path)
	return err == nil
}

// isDirEmpty 目录为空时返回 true, 目录不存在时也视为空.
func isDirEmpty(path string) bool {
	f, err := os.Open(path)
	if err != nil {
		return true
	}
	defer f.Close()
	_, err = f.Readdirnames(1)
	return err != nil
}

func (mf *MainForm) processStartupPaths() {
	if len(mf.startupPaths) == 0 {
		return
	}
	logger.Info("process startup paths, count: %d", len(mf.startupPaths))

	var imageFiles []string
	for _, path := range mf.startupPaths {
		info, err := fileio.GetFileInfo(path)
		if err != nil {
			continue
		}
		if info.IsDir() {
			mf.picListView.AddPicturesFromDirectory(path)
		} else if fileio.IsImageFile(path) {
			imageFiles = append(imageFiles, path)
		}
	}
	if len(imageFiles) > 0 {
		mf.picListView.AddPicturesFromPaths(imageFiles)
	}
	mf.updateSelectLabel()
	logger.Debug("process startup paths done, total: %d", mf.picListView.GetModel().ItemCount())
}

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

// logSettings 输出当前设置的英文摘要, 便于调试时还原压缩参数.
func logSettings(set settingLoader.Setting) {
	logger.Info("settings, compress: %s, quality: %s, limit: %d %s, accept exceed: %t, output: %s, extension: %s, template: %q, start index: %d, threads: %d",
		settingLoader.CompressTypeName(set.CompressType),
		settingLoader.QualityLevelName(set.Quality),
		set.LimitSize,
		settingLoader.SizeUnitName(set.SizeUnit),
		set.AcceptExceed,
		settingLoader.OutputTypeName(set.OutputType),
		settingLoader.ExtensionName(set.Extension),
		set.OutputFilename,
		set.StartIndex,
		set.MaxThreads,
	)
	logger.Info("preprocess and advanced, alpha: %s, scale: %s, scale size: %dx%d, jpeg quality: %d, webp quality: %d, png palette: %s, png keep alpha: %t, png dithering: %t",
		settingLoader.AlphaHandleName(set.AlphaHandle),
		settingLoader.ScaleName(set.Scale),
		set.ScaleWidth,
		set.ScaleHeight,
		set.AdvancedJpegQuality,
		set.AdvancedWebPQuality,
		settingLoader.PaletteAlgoName(set.AdvancedPngPaletteAlgo),
		set.AdvancedPngKeepIndexedAlpha,
		set.AdvancedPngEnableDithering,
	)
}
