package ui

import (
	"PicSizer/internal/core"
	"PicSizer/internal/core/setting"
	"PicSizer/internal/dialog"
	"PicSizer/internal/fileio"
	"PicSizer/internal/server"
	"fmt"
	"sync"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
	"github.com/lxn/win"
)

// MainForm 主窗口
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
	// poolMu 保护对 pool 的访问, 用于避免重复点击"开始压缩"时同时启动多个任务
	poolMu sync.Mutex
	pool   *server.ThreadPool
}

// NewMainForm 创建主窗口
func NewMainForm() *MainForm {
	mf := &MainForm{
		progressForm: NewProgressForm(),
		settingForm:  NewSettingForm(),
		aboutForm:    NewAboutForm(),
	}
	// 注入置顶变更回调, 使设置窗口保存后主窗口立即响应
	mf.settingForm.SetOnTopMostChanged(mf.applyTopMost)
	// 注入输出方式变更回调, 使设置窗口保存后主窗口的单选框立即同步
	mf.settingForm.SetOnOutputTypeChanged(mf.onOutputTypeFromSetting)
	return mf
}

// Run 运行主窗口
func (mf *MainForm) Run(appIcon *walk.Icon) error {
	mf.appIcon = appIcon
	mf.picListView = NewPicListView()

	var err error
	err = declarative.MainWindow{
		AssignTo: &mf.MainWindow,
		Title:    core.TitleMain,
		Icon:     appIcon,
		MinSize:  declarative.Size{Width: 600, Height: 500},
		Size:     declarative.Size{Width: 600, Height: 500},
		Layout:   declarative.VBox{MarginsZero: false, Margins: declarative.Margins{Left: 10, Top: 10, Right: 10, Bottom: 10}, SpacingZero: false, Spacing: 8},
		OnBoundsChanged: func() {
			// 启用网格线（只在第一次触发时设置）
			if mf.picListView.TableView != nil && mf.picListView.onSelChanged == nil {
				mf.picListView.SetGridlines(true)
				mf.picListView.onSelChanged = mf.updateSelectLabel
			}
		},
		MenuItems: []declarative.MenuItem{
			declarative.Menu{
				Text: core.TextFile,
				Items: []declarative.MenuItem{
					declarative.Action{Text: core.TextAddFiles, OnTriggered: mf.onAddFiles},
					declarative.Action{Text: core.TextAddFolder, OnTriggered: mf.onAddFolder},
					declarative.Separator{},
					declarative.Action{Text: core.TextExit, OnTriggered: mf.onExit},
				},
			},
			declarative.Menu{
				Text: core.TextEdit,
				Items: []declarative.MenuItem{
					declarative.Menu{
						Text: core.TextRemove,
						Items: []declarative.MenuItem{
							declarative.Action{Text: core.TextRemoveSelect, OnTriggered: mf.onRemoveSelected},
							declarative.Action{Text: core.TextRemoveDone, OnTriggered: mf.onRemoveDone},
							declarative.Action{Text: core.TextRemoveError, OnTriggered: mf.onRemoveError},
							declarative.Action{Text: core.TextRemoveAll, OnTriggered: mf.onRemoveAll},
						},
					},
					declarative.Menu{
						Text: core.TextSelect,
						Items: []declarative.MenuItem{
							declarative.Action{Text: core.TextSelectAll, OnTriggered: mf.onSelectAll},
							declarative.Action{Text: core.TextSelectReverse, OnTriggered: mf.onSelectReverse},
						},
					},
				},
			},
			declarative.Menu{
				Text: core.TextHelp,
				Items: []declarative.MenuItem{
					declarative.Action{Text: core.TextAbout, OnTriggered: mf.onAbout},
				},
			},
		},
		Children: []declarative.Widget{
			// 图片列表
			mf.picListView.PicListViewWidget(),

			// 左右分栏的主控制区
			declarative.Composite{
				Layout: declarative.HBox{MarginsZero: true, Spacing: 15},
				Children: []declarative.Widget{

					// 【左侧区域】：包含单选行和路径行
					declarative.Composite{
						Layout: declarative.VBox{MarginsZero: true, Spacing: 10},
						Children: []declarative.Widget{
							// 第一行：三个单选按钮
							declarative.Composite{
								Layout: declarative.HBox{MarginsZero: true, Spacing: 10},
								Children: []declarative.Widget{
									declarative.RadioButton{
										AssignTo:  &mf.dirRadio,
										Text:      core.TextOutputDir,
										OnClicked: mf.onRadioChange,
									},
									declarative.RadioButton{
										AssignTo:  &mf.coverRadio,
										Text:      core.TextCoverOrigin,
										OnClicked: mf.onRadioChange,
									},
									declarative.RadioButton{
										AssignTo:  &mf.structRadio,
										Text:      core.TextOutputStruct,
										OnClicked: mf.onRadioChange,
									},
									declarative.HSpacer{}, // 把单选框往左边推
								},
							},
							// 第二行：输入框和浏览按钮
							declarative.Composite{
								Layout: declarative.HBox{MarginsZero: true, Spacing: 5},
								Children: []declarative.Widget{
									declarative.Label{Text: core.TextOutputPath},
									declarative.LineEdit{
										AssignTo: &mf.outputDirEdit,
									},
									declarative.PushButton{
										AssignTo:  &mf.outputDirBtn,
										Text:      core.TextBrowse,
										OnClicked: mf.onBrowseDir,
									},
								},
							},
						},
					},

					// 【右侧区域】：垂直排列的“设置”与“开始”按钮
					declarative.Composite{
						Layout: declarative.VBox{MarginsZero: true, Spacing: 8},
						Children: []declarative.Widget{
							declarative.PushButton{
								AssignTo:  &mf.settingBtn,
								Text:      core.TextSetting,
								MinSize:   declarative.Size{Width: 95, Height: 0}, // 统一按钮宽度
								OnClicked: mf.onSetting,
							},
							declarative.PushButton{
								AssignTo:  &mf.startBtn,
								Text:      core.TextStart,
								MinSize:   declarative.Size{Width: 95, Height: 0}, // 统一按钮宽度
								OnClicked: mf.onStartCompress,
							},
						},
					},
				},
			},

			// 底部状态栏
			declarative.Composite{
				Layout: declarative.HBox{MarginsZero: false, Margins: declarative.Margins{Left: 10, Top: 5, Right: 10, Bottom: 5}},
				Children: []declarative.Widget{
					declarative.Label{
						AssignTo: &mf.selectLabel,
						Text:     "0/0",
					},
					declarative.HSpacer{},
					declarative.Label{Text: core.AppName + " " + core.AppVersion},
				},
			},
		},
		OnDropFiles: mf.onDropFiles,
	}.Create()

	if err != nil {
		return err
	}

	// 将主窗口句柄绑定到弹窗模块, 之后所有弹窗将以主窗口为父窗口
	// 配合 MB_TASKMODAL 等标志位, 实现弹窗任务模态行为
	dialog.SetParentHwnd(uintptr(mf.MainWindow.Handle()))

	// 窗口居中显示
	CenterWindow(mf.MainWindow)

	// 读取一次设置, 用于应用置顶状态
	setting := setting.GetSetting()
	mf.applyRadioToOutputType(setting.OutputType)
	// 根据当前设置更新输出目录控件的启用状态
	mf.onRadioChange()

	// 先显示窗口, 再应用置顶, 避免 Run() 内部 Show() 重置 Z-Order
	mf.MainWindow.Show()
	mf.applyTopMost(setting.TopMost)

	mf.MainWindow.Run()
	return nil
}

// applyTopMost 应用窗口置顶状态
//
// 说明: 通过 Windows API SetWindowPos 设置/取消 HWND_TOPMOST 标志,
// 该标志由 Z-Order 决定, 不依赖 walk 框架内置的 AlwaysOnTop 字段.
// 内部委托给 ApplyTopMostToWindow 工具函数, 与子窗体使用同一套机制.
func (mf *MainForm) applyTopMost(topMost bool) {
	ApplyTopMostToWindow(mf.MainWindow, topMost)
}

// onAddFiles 添加文件
func (mf *MainForm) onAddFiles() {
	paths := ShowOpenImageDialog(mf.MainWindow)
	if len(paths) > 0 {
		mf.picListView.AddPicturesFromPaths(paths)
		mf.updateSelectLabel()
	}
}

// onAddFolder 添加文件夹
func (mf *MainForm) onAddFolder() {
	dir := ShowBrowseFolderDialog(mf.MainWindow, core.TextAddFolder)
	if dir != "" {
		mf.picListView.AddPicturesFromDirectory(dir)
		mf.updateSelectLabel()
	}
}

// onDropFiles 拖放文件到主窗口
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

// onBrowseDir 浏览输出目录
func (mf *MainForm) onBrowseDir() {
	dir := ShowBrowseFolderDialog(mf.MainWindow, core.TextOutputDir)
	if dir != "" {
		mf.outputDirEdit.SetText(dir)
	}
}

// onRadioChange 单选按钮变化
//
// 行为:
//  1. 根据当前选中的单选框, 立即调用 core.UpdateSettingWithOutputType
//     同步更新全局 setting, 保证设置界面再次打开时显示一致.
//  2. 切换输出目录输入框与浏览按钮的启用状态
//     (选中"覆盖源文件"时禁用, 其他两种启用).
func (mf *MainForm) onRadioChange() {
	if mf.coverRadio != nil && mf.dirRadio != nil && mf.structRadio != nil {
		// 三个单选按钮在主窗体创建时已根据 setting 选中, 此处无需空指针判断,
		// 但保守起见保留 nil 判断, 避免在 walk 框架初始化时序异常时崩溃.
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

// applyRadioToOutputType 同步主窗体单选框选中状态
//
// 内部按以下顺序操作:
//  1. 先取消全部三个 RadioButton 的选中, 避免多选
//  2. 再根据 ot 选中目标按钮
//
// 注意: walk 框架的 RadioButton.SetChecked 走 BM_SETCHECK 消息,
// 不会触发 BN_CLICKED, 因此同组其他按钮不会被自动取消选中,
// 必须手动先取消所有按钮再选中目标, 避免出现"多选"现象.
// 该方法不触发 OnClicked 事件, 也不会调用 onRadioChange.
func (mf *MainForm) applyRadioToOutputType(ot setting.OutputType) {
	if mf.coverRadio == nil || mf.dirRadio == nil || mf.structRadio == nil {
		return
	}
	// 1. 先取消所有按钮的选中状态, 防止多选
	mf.dirRadio.SetChecked(false)
	mf.coverRadio.SetChecked(false)
	mf.structRadio.SetChecked(false)
	// 2. 根据新值选中目标按钮
	switch ot {
	case setting.OutputCoverOrigin:
		mf.coverRadio.SetChecked(true)
	case setting.OutputDirection:
		mf.dirRadio.SetChecked(true)
	case setting.OutputStructure:
		mf.structRadio.SetChecked(true)
	}
}

// onOutputTypeFromSetting 接收设置窗口保存后的输出方式变更通知
//
// 说明: 由 SettingForm 在 saveSetting 检测到 OutputType 变化时回调,
// 同步主窗体单选框选中状态, 保证两边 UI 显示一致.
// 内部委托给 applyRadioToOutputType, 再触发 onRadioChange 同步输入框启用状态.
func (mf *MainForm) onOutputTypeFromSetting(ot setting.OutputType) {
	mf.applyRadioToOutputType(ot)
	// 同步输出目录输入框的启用状态与 setting
	mf.onRadioChange()
}

// onStartCompress 开始压缩
//
// 流程:
//  1. 首先判断是否已有压缩任务正在进行 (进度窗体是否已显示),
//     若是, 提示用户并直接返回, 避免同时启动多个压缩任务.
//  2. 校验图片列表与输出目录.
//  3. 若列表中存在已成功 (StateSuccess) 的项目, 弹出确认弹窗警告用户
//     这些已完成的任务会被覆盖. 用户选择取消则直接返回.
//  4. 启动前把所有任务的状态统一重置为 StateWaiting, 并清空 NewSize /
//     Message / OutputPath, 避免显示陈旧的压缩结果, 同时触发行颜色刷新.
//  5. 设置进度窗体的取消回调, 让用户在点击取消或关闭窗体时能够停止压缩.
//  6. 显示进度窗体 (模态, 居中于主窗体), 然后启动线程池.
//
// 注意: 不再"智能跳过"已成功完成的项目, 所有项目都会被本轮压缩.
func (mf *MainForm) onStartCompress() {
	// 1. 防止重复启动: 若已有任务正在进行, 提示并返回
	mf.poolMu.Lock()
	if mf.pool != nil || mf.progressForm.IsVisible() {
		mf.poolMu.Unlock()
		dialog.ShowWarning(core.WarnCompressRunning)
		return
	}
	mf.poolMu.Unlock()

	allItems := mf.picListView.GetModel().GetItems()
	if len(allItems) == 0 {
		dialog.ShowWarning(core.ErrNoPictures)
		return
	}

	// 2. 覆盖警告: 列表中存在已完成 (成功) 的项目时, 二次确认是否覆盖
	alreadyDone := mf.picListView.GetModel().CountByState(setting.StateSuccess)
	if alreadyDone > 0 {
		// 弹出确认弹窗, 用户取消则直接退出, 不做任何重置和压缩
		if !dialog.ShowConfirm(fmt.Sprintf(core.WarnOverwriteDoneFmt, alreadyDone)) {
			return
		}
	}

	if !mf.coverRadio.Checked() {
		core.OutputDirPath = mf.outputDirEdit.Text()
		if core.OutputDirPath == "" {
			dialog.ShowWarning(core.ErrOutputDirInvalid)
			return
		}
	}

	// 根据当前选中的输出方式设置运行时变量.
	// setting.OutputType 已在 onRadioChange 中实时同步, 此处无需再调用 UpdateSettingWithOutputType.
	// "输出到目录"和"保留结构"两种模式需要写入路径运行时变量 (OutputDirPath / PublicDirPath),
	// "覆盖源文件"模式不需要任何路径变量.
	if mf.coverRadio.Checked() {
		// 覆盖源文件: 无需设置输出目录与公共目录
	} else if mf.dirRadio.Checked() {
		core.OutputDirPath = mf.outputDirEdit.Text()
		if core.OutputDirPath == "" {
			dialog.ShowWarning(core.ErrOutputDirInvalid)
			return
		}
	} else {
		// 保留结构: 需要计算公共父目录作为结构基准
		var paths []string
		for _, item := range allItems {
			paths = append(paths, item.FullPath)
		}
		// 先校验共同前缀是否可计算 (例如 Windows 跨盘符的图片无法找到共同前缀)
		prefix, ok := fileio.GetCommonPrefix(paths)
		if !ok {
			// 无法找到共同前缀, 提示用户重新选图或分多次压缩
			dialog.ShowWarning(core.ErrNoCommonPrefix)
			return
		}
		core.PublicDirPath = prefix
	}

	// 3. 启动前重置所有项目为等待状态, 清空陈旧结果, 并按行刷新颜色
	//    先关闭自动滚动, 避免批量重置时滑块从顶部一路滚到底部
	mf.picListView.StopAutoScrollDown()
	mf.picListView.GetModel().ResetAllToWaiting()
	for _, item := range allItems {
		mf.picListView.GetModel().PublishRowChangedByItem(item)
	}
	// 启用自动下滚跟随: 新一轮压缩开始, 重置最大已滚动行号, 之后任意行状态变化都会触发下滚
	mf.picListView.ResetAutoScrollDown()

	// 4. 绑定进度窗体的取消回调: 用户点击"取消"或窗体叉叉时调用 pool.Stop
	//    ProgressForm 内部会进入"等待结束"状态 (标签切换 + 按钮变灰 + 阻止关闭),
	//    待所有子任务完成后由 pool.onComplete 统一关闭窗体.
	mf.progressForm.SetOnCancel(func() {
		mf.poolMu.Lock()
		pool := mf.pool
		mf.poolMu.Unlock()
		if pool != nil {
			pool.Stop()
		}
	})

	// 5. 绑定进度窗体的关闭完成回调: 无论是因为取消还是自然结束, 都清空 mf.pool 引用
	mf.progressForm.SetOnClosed(func() {
		mf.poolMu.Lock()
		mf.pool = nil
		mf.poolMu.Unlock()
	})

	// 6. 显示进度窗体 (传入主窗体作为 owner, 进度窗体会居中显示并禁用主窗体)
	//    total 使用"本轮待处理"数量, 而非整个列表数量, 让进度条准确反映本轮进度.
	if err := mf.progressForm.Show(mf.MainWindow, len(allItems), mf.appIcon); err != nil {
		dialog.ShowError(err.Error())
		return
	}

	// 7. 创建并启动线程池
	pool := server.NewThreadPool(allItems,
		func(current, errCount, total int) {
			mf.progressForm.UpdateProgress(current, errCount, total)
		},
		func(item *core.PicItem) {
			// 单项状态变更回调: 通知 UI 刷新对应行的颜色
			mf.picListView.GetModel().PublishRowChangedByItem(item)
		},
		func(success, errCount, total int) {
			// 全部任务已完成 (正常完成或被取消), 关闭进度窗体并弹出结果提示
			// 关闭自动下滚跟随, 恢复用户对滚动条的手动控制
			mf.picListView.StopAutoScrollDown()
			mf.progressForm.Close()
			dialog.ShowCompressResult(total, success)
		},
	)

	mf.poolMu.Lock()
	mf.pool = pool
	mf.poolMu.Unlock()

	// 在独立 goroutine 中启动并阻塞等待, 行为等价于旧的 "pool.Start() 阻塞至完成":
	//   - Start() 非阻塞, 立即返回, 把 worker 全部拉起;
	//   - Wait() 阻塞, 等待所有 worker 退出, 然后在内部触发 onComplete 回调
	//     (关闭进度窗体 + 弹结果提示), 因此 onComplete 仍运行在该 goroutine 中,
	//     与原行为保持一致.
	go func() {
		pool.Start()
		pool.Wait()
	}()
}

// onSetting 打开设置窗口
func (mf *MainForm) onSetting() {
	mf.settingForm.Show(mf.MainWindow, mf.appIcon)
	// 设置窗口关闭后, 重新应用 TopMost 设置, 避免对话框关闭时系统重置 Z-Order
	setting := setting.GetSetting()
	mf.applyTopMost(setting.TopMost)
}

// onAbout 打开关于窗口
func (mf *MainForm) onAbout() {
	mf.aboutForm.Show(mf.MainWindow, mf.appIcon)
}

// onRemoveSelected 移除选中项
func (mf *MainForm) onRemoveSelected() {
	mf.picListView.GetModel().RemoveSelected(mf.picListView.TableView)
	mf.updateSelectLabel()
}

// onRemoveDone 移除已完成项
func (mf *MainForm) onRemoveDone() {
	mf.picListView.GetModel().RemoveByState(setting.StateSuccess)
	mf.updateSelectLabel()
}

// onRemoveError 移除错误项
func (mf *MainForm) onRemoveError() {
	mf.picListView.GetModel().RemoveByState(setting.StateError)
	mf.picListView.GetModel().RemoveByState(setting.StateOutOfLimit)
	mf.updateSelectLabel()
}

// onRemoveAll 清空列表
func (mf *MainForm) onRemoveAll() {
	if dialog.ShowConfirm(core.MsgClearList) {
		mf.picListView.GetModel().Clear()
		mf.updateSelectLabel()
	}
}

// onSelectAll 全选
func (mf *MainForm) onSelectAll() {
	mf.picListView.GetModel().SelectAll(mf.picListView.TableView)
	mf.updateSelectLabel()
}

// onSelectReverse 反选
func (mf *MainForm) onSelectReverse() {
	mf.picListView.GetModel().SelectReverse(mf.picListView.TableView)
	mf.updateSelectLabel()
}

// onExit 退出程序
func (mf *MainForm) onExit() {
	mf.MainWindow.Close()
}

// updateSelectLabel 更新选择标签
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
