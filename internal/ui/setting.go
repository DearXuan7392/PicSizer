package ui

import (
	"PicSizer/internal/core/settingLoader"
	strs "PicSizer/internal/core/strings"
	"PicSizer/internal/dialog"
	"os"
	"os/exec"
	"runtime"
	"strconv"
	"strings"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
)

// settingControlWidth 设置窗口中输入框 / 下拉框的统一固定宽度 (单位: 像素)
const settingControlWidth = 200

const groupVerticalSpacing = 8

// SettingForm 表示设置窗口, 包含三个选项卡页面: 常规设置、图像预处理和高级设置.
type SettingForm struct {
	*walk.Dialog

	// 常规设置控件
	qualityRadio      *walk.RadioButton
	fileSizeRadio     *walk.RadioButton
	qualityLabel      *walk.Label  // 用于显示当前画质等级文本
	qualitySlider     *walk.Slider // 画质滑块
	limitSizeEdit     *walk.NumberEdit
	sizeUnitCombo     *walk.ComboBox
	acceptExceedCheck *walk.CheckBox
	outputTypeCombo   *walk.ComboBox
	extensionCombo    *walk.ComboBox
	filenameEdit      *walk.LineEdit
	startIndexEdit    *walk.NumberEdit
	maxThreadsEdit    *walk.NumberEdit
	threadSlider      *walk.Slider
	topMostCheck      *walk.CheckBox

	// 预处理设置控件
	alphaCombo  *walk.ComboBox
	scaleCombo  *walk.ComboBox
	scaleWidth  *walk.NumberEdit
	scaleHeight *walk.NumberEdit

	// 高级设置
	jpegQualityEdit      *walk.NumberEdit
	webpQualityEdit      *walk.NumberEdit
	pngPaletteAlgoCombo  *walk.ComboBox
	pngKeepAlphaCheckBox *walk.CheckBox
	pngDitheringCheckBox *walk.CheckBox

	debugModeCheck *walk.CheckBox

	maxThreads int

	// workingCopy 存储设置的拷贝, 所有修改都作用于此拷贝
	workingCopy settingLoader.Setting

	onTopMostChanged    func(bool)
	onOutputTypeChanged func(settingLoader.OutputType)
}

// NewSettingForm 创建设置窗口实例.
func NewSettingForm() *SettingForm {
	return &SettingForm{}
}

// SetOnTopMostChanged 设置置顶变更回调, 由主窗口在创建后注入.
func (sf *SettingForm) SetOnTopMostChanged(fn func(bool)) {
	sf.onTopMostChanged = fn
}

// SetOnOutputTypeChanged 设置输出方式变更回调, 由主窗口在创建后注入.
func (sf *SettingForm) SetOnOutputTypeChanged(fn func(settingLoader.OutputType)) {
	sf.onOutputTypeChanged = fn
}

// Show 显示设置窗口. 窗口包含三个选项卡页面, 关闭后调用方可通过回调获取用户修改的设置.
func (sf *SettingForm) Show(owner walk.Form, appIcon *walk.Icon) error {
	// 获取设置的拷贝, 所有修改都作用于此拷贝
	sf.workingCopy = settingLoader.GetSettingCopy()
	set := sf.workingCopy
	// 获取 CPU 逻辑核心数
	sf.maxThreads = runtime.NumCPU()

	// 判断当前压缩模式, 设置输入框禁用状态
	isQualityMode := (set.CompressType == settingLoader.CompressQuality)

	var err error
	err = declarative.Dialog{
		AssignTo:  &sf.Dialog,
		Title:     strs.TitleSetting,
		Icon:      appIcon,
		FixedSize: true,
		MinSize:   declarative.Size{Width: 380, Height: 600},
		MaxSize:   declarative.Size{Width: 380, Height: 600},
		Size:      declarative.Size{Width: 380, Height: 600},
		Layout:    declarative.VBox{Margins: declarative.Margins{Left: 10, Top: 10, Right: 10, Bottom: 10}},
		Children: []declarative.Widget{
			declarative.TabWidget{
				Pages: []declarative.TabPage{
					// 第一页: 常规设置
					declarative.TabPage{
						Title:  strs.TextGeneralSetting,
						Layout: declarative.VBox{Margins: declarative.Margins{Left: 5, Top: 10, Right: 5, Bottom: 5}, Spacing: groupVerticalSpacing},
						Children: []declarative.Widget{
							// 压缩设置组
							declarative.GroupBox{
								Title:  strs.TextCompressSetting,
								Layout: declarative.Grid{Columns: 2},
								Children: []declarative.Widget{
									sf.makeLabelWithTip(strs.TextCompressMode, strs.TipCompressMode),
									declarative.Composite{
										Layout:  declarative.HBox{Spacing: 20, MarginsZero: true},
										MinSize: declarative.Size{Width: settingControlWidth, Height: 0},
										MaxSize: declarative.Size{Width: settingControlWidth, Height: 0},
										Children: []declarative.Widget{
											declarative.HSpacer{},
											declarative.RadioButton{
												AssignTo:  &sf.qualityRadio,
												Text:      strs.TextQualityMode,
												Value:     isQualityMode,
												OnClicked: sf.onCompressModeChange,
											},
											declarative.RadioButton{
												AssignTo:  &sf.fileSizeRadio,
												Text:      strs.TextFileSizeMode,
												Value:     !isQualityMode,
												OnClicked: sf.onCompressModeChange,
											},
										},
									},
									sf.makeLabelWithTip(strs.TextQuality, strs.TipQuality),
									// 画质滑块组合
									declarative.Composite{
										Layout: declarative.HBox{Spacing: 5, MarginsZero: true},
										Children: []declarative.Widget{
											declarative.HSpacer{},
											declarative.Label{
												AssignTo:  &sf.qualityLabel,
												Text:      qualityLevelToString(set.Quality),
												MinSize:   declarative.Size{Width: settingControlWidth - 145, Height: 0},
												Alignment: declarative.AlignHFarVCenter,
											},
											declarative.Slider{
												AssignTo:       &sf.qualitySlider,
												MinValue:       0,
												MaxValue:       3,
												Value:          qualityLevelToSliderValue(set.Quality),
												Tracking:       true,
												Enabled:        isQualityMode,
												MinSize:        declarative.Size{Width: 140, Height: 0},
												MaxSize:        declarative.Size{Width: 140, Height: 0},
												OnValueChanged: func() { sf.onQualitySliderChange() },
											},
										},
									},
									sf.makeLabelWithTip(strs.TextLimitSize, strs.TipLimitSize),
									declarative.Composite{
										Layout: declarative.HBox{Spacing: 5, MarginsZero: true},
										Children: []declarative.Widget{
											declarative.HSpacer{},
											declarative.NumberEdit{
												AssignTo:      &sf.limitSizeEdit,
												Value:         float64(set.LimitSize),
												MinValue:      1,
												MaxValue:      100000,
												Decimals:      0,
												Enabled:       !isQualityMode,
												StretchFactor: 1,
												MinSize:       declarative.Size{Width: settingControlWidth - 45, Height: 0},
												MaxSize:       declarative.Size{Width: settingControlWidth - 45, Height: 0},
											},
											declarative.ComboBox{
												AssignTo: &sf.sizeUnitCombo,
												Value:    sizeUnitToString(set.SizeUnit),
												Editable: false,
												Model:    []string{strs.TextUnitKB, strs.TextUnitMB},
												Enabled:  !isQualityMode,
												MinSize:  declarative.Size{Width: 40, Height: 0},
												MaxSize:  declarative.Size{Width: 40, Height: 0},
											},
										},
									},
									declarative.Label{Text: " "},
									sf.makeCheckBoxWithTip(&sf.acceptExceedCheck, strs.TextAcceptExceed, set.AcceptExceed, strs.TipAcceptExceed),
								},
							},
							// 输出设置组
							declarative.GroupBox{
								Title:  strs.TextOutputFormat,
								Layout: declarative.Grid{Columns: 2},
								Children: []declarative.Widget{
									sf.makeLabelWithTip(strs.TextOutputMode, strs.TipOutputType),
									declarative.Composite{
										Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
										Children: []declarative.Widget{
											declarative.HSpacer{},
											declarative.ComboBox{
												AssignTo:              &sf.outputTypeCombo,
												Value:                 outputTypeToString(set.OutputType),
												Editable:              false,
												Model:                 []string{strs.TextOutputDir, strs.TextCoverOrigin, strs.TextOutputStruct},
												OnCurrentIndexChanged: func() { sf.onOutputTypeChange() },
												MinSize:               declarative.Size{Width: settingControlWidth, Height: 0},
												MaxSize:               declarative.Size{Width: settingControlWidth, Height: 0},
											},
										},
									},
									sf.makeLabelWithTip(strs.TextOutputFormat, strs.TipExtension),
									declarative.Composite{
										Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
										Children: []declarative.Widget{
											declarative.HSpacer{},
											declarative.ComboBox{
												AssignTo: &sf.extensionCombo,
												Value:    extensionToString(set.Extension),
												Editable: false,
												Model:    []string{strs.TextFormatJPEG, strs.TextFormatPNG, strs.TextFormatWebP, strs.TextFormatOrigin},
												MinSize:  declarative.Size{Width: settingControlWidth, Height: 0},
												MaxSize:  declarative.Size{Width: settingControlWidth, Height: 0},
											},
										},
									},
									sf.makeLabelWithTip(strs.TextFilenameTpl, strs.TipFilenameTpl),
									declarative.Composite{
										Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
										Children: []declarative.Widget{
											declarative.HSpacer{},
											declarative.LineEdit{
												AssignTo: &sf.filenameEdit,
												Text:     set.OutputFilename,
												MinSize:  declarative.Size{Width: settingControlWidth, Height: 0},
												MaxSize:  declarative.Size{Width: settingControlWidth, Height: 0},
											},
										},
									},
									sf.makeLabelWithTip(strs.TextStartIndex, strs.TipStartIndex),
									declarative.Composite{
										Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
										Children: []declarative.Widget{
											declarative.HSpacer{},
											declarative.NumberEdit{
												AssignTo: &sf.startIndexEdit,
												Value:    float64(set.StartIndex),
												MinValue: 1,
												MaxValue: 99999,
												Decimals: 0,
												MinSize:  declarative.Size{Width: settingControlWidth, Height: 0},
												MaxSize:  declarative.Size{Width: settingControlWidth, Height: 0},
											},
										},
									},
								},
							},
							// 系统设置组
							declarative.GroupBox{
								Title:  strs.TextSystemSetting,
								Layout: declarative.Grid{Columns: 2},
								Children: []declarative.Widget{
									sf.makeLabelWithTip(strs.TextMaxThreads, strs.TipMaxThreads),
									declarative.Composite{
										Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
										Children: []declarative.Widget{
											declarative.HSpacer{},
											declarative.Composite{
												Layout:  declarative.VBox{SpacingZero: true, MarginsZero: true},
												MinSize: declarative.Size{Width: settingControlWidth, Height: 0},
												MaxSize: declarative.Size{Width: settingControlWidth, Height: 0},
												Children: []declarative.Widget{
													declarative.NumberEdit{
														AssignTo:       &sf.maxThreadsEdit,
														Value:          float64(set.MaxThreads),
														MinValue:       1,
														MaxValue:       float64(sf.maxThreads),
														Decimals:       0,
														OnValueChanged: func() { sf.onThreadEditChange() },
													},
													declarative.Slider{
														AssignTo:       &sf.threadSlider,
														MinValue:       1,
														MaxValue:       sf.maxThreads,
														Value:          set.MaxThreads,
														Tracking:       true,
														ToolTipText:    strs.TipThreadSlider,
														OnValueChanged: func() { sf.onThreadSliderChange() },
													},
												},
											},
										},
									},
									declarative.Label{Text: " "},
									sf.makeCheckBoxWithTip(&sf.topMostCheck, strs.TextTopMost, set.TopMost, strs.TipTopMost),
								},
							},
							declarative.VSpacer{},
						},
					},
					// 第二页: 图像预处理
					declarative.TabPage{
						Title:  strs.TextPreprocessSetting,
						Layout: declarative.VBox{Margins: declarative.Margins{Left: 5, Top: 10, Right: 5, Bottom: 5}, Spacing: groupVerticalSpacing},
						Children: []declarative.Widget{
							// 缩放方式组
							declarative.GroupBox{
								Title:  strs.TextScale,
								Layout: declarative.Grid{Columns: 2},
								Children: []declarative.Widget{
									sf.makeLabelWithTip(strs.TextScale, strs.TipScale),
									declarative.Composite{
										Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
										Children: []declarative.Widget{
											declarative.HSpacer{},
											declarative.ComboBox{
												AssignTo: &sf.scaleCombo,
												Value:    scaleToString(set.Scale),
												Editable: false,
												Model: []string{
													strs.ScaleNoneText,
													strs.ScaleStretchText,
													strs.ScaleFitOutText,
													strs.ScaleFitInText,
													strs.ScaleFitOutCrop,
													strs.ScaleLockSideText,
												},
												OnCurrentIndexChanged: func() { sf.onScaleModeChange() },
												MinSize:               declarative.Size{Width: settingControlWidth, Height: 0},
												MaxSize:               declarative.Size{Width: settingControlWidth, Height: 0},
											},
										},
									},
									sf.makeLabelWithTip(strs.TextScaleWidth, strs.TipScaleWidth),
									declarative.Composite{
										Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
										Children: []declarative.Widget{
											declarative.HSpacer{},
											declarative.NumberEdit{
												AssignTo: &sf.scaleWidth,
												Value:    float64(set.ScaleWidth),
												MinValue: 0,
												MaxValue: 99999,
												Decimals: 0,
												Enabled:  set.Scale != settingLoader.ScaleNone,
												MinSize:  declarative.Size{Width: settingControlWidth, Height: 0},
												MaxSize:  declarative.Size{Width: settingControlWidth, Height: 0},
											},
										},
									},
									sf.makeLabelWithTip(strs.TextScaleHeight, strs.TipScaleHeight),
									declarative.Composite{
										Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
										Children: []declarative.Widget{
											declarative.HSpacer{},
											declarative.NumberEdit{
												AssignTo: &sf.scaleHeight,
												Value:    float64(set.ScaleHeight),
												MinValue: 0,
												MaxValue: 99999,
												Decimals: 0,
												Enabled:  set.Scale != settingLoader.ScaleNone,
												MinSize:  declarative.Size{Width: settingControlWidth, Height: 0},
												MaxSize:  declarative.Size{Width: settingControlWidth, Height: 0},
											},
										},
									},
								},
							},
							// 透明度处理组
							declarative.GroupBox{
								Title:  strs.TextAlphaHandle,
								Layout: declarative.Grid{Columns: 2},
								Children: []declarative.Widget{
									sf.makeLabelWithTip(strs.TextAlphaHandle, strs.TipAlphaHandle),
									declarative.Composite{
										Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
										Children: []declarative.Widget{
											declarative.HSpacer{},
											declarative.ComboBox{
												AssignTo: &sf.alphaCombo,
												Value:    alphaHandleToString(set.AlphaHandle),
												Editable: false,
												Model: []string{
													strs.AlphaKeepText,
													strs.AlphaSmartRemoveText,
													strs.AlphaRemoveText,
												},
												MinSize: declarative.Size{Width: settingControlWidth, Height: 0},
												MaxSize: declarative.Size{Width: settingControlWidth, Height: 0},
											},
										},
									},
								},
							},
							declarative.VSpacer{},
						},
					},
					// 第三页: 高级设置
					declarative.TabPage{
						Title:  strs.TextAdvancedSetting,
						Layout: declarative.VBox{Margins: declarative.Margins{Left: 5, Top: 10, Right: 5, Bottom: 5}, Spacing: groupVerticalSpacing},
						Children: []declarative.Widget{
							// JPEG 设置容器
							declarative.GroupBox{
								Title:  strs.TextAdvJPEGSettings,
								Layout: declarative.Grid{Columns: 2},
								Children: []declarative.Widget{
									sf.makeLabelWithTip(strs.TextAdvFineQuality, strs.TipAdvFineQuality),
									declarative.Composite{
										Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
										Children: []declarative.Widget{
											declarative.HSpacer{},
											declarative.NumberEdit{
												AssignTo:    &sf.jpegQualityEdit,
												Value:       float64(set.AdvancedJpegQuality),
												MinValue:    0,
												MaxValue:    100,
												Decimals:    0,
												ToolTipText: strs.TipAdvFineQuality,
												MinSize:     declarative.Size{Width: settingControlWidth, Height: 0},
												MaxSize:     declarative.Size{Width: settingControlWidth, Height: 0},
											},
										},
									},
								},
							},
							// PNG 设置容器
							declarative.GroupBox{
								Title:  strs.TextAdvPNGSettings,
								Layout: declarative.Grid{Columns: 2},
								Children: []declarative.Widget{
									sf.makeLabelWithTip(strs.TextPngPaletteAlgo, strs.TipPngPaletteAlgo),
									declarative.Composite{
										Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
										Children: []declarative.Widget{
											declarative.HSpacer{},
											declarative.ComboBox{
												AssignTo: &sf.pngPaletteAlgoCombo,
												Value:    paletteAlgoToString(set.AdvancedPngPaletteAlgo),
												Editable: false,
												Model:    []string{strs.TextPaletteMedianCut, strs.TextPaletteKMeans},
												MinSize:  declarative.Size{Width: settingControlWidth, Height: 0},
												MaxSize:  declarative.Size{Width: settingControlWidth, Height: 0},
											},
										},
									},
									declarative.Label{
										Text:    " ",
										MinSize: declarative.Size{Width: 150, Height: 0},
									},
									sf.makeCheckBoxWithTip(&sf.pngKeepAlphaCheckBox, strs.TextPngKeepIndexedAlpha, set.AdvancedPngKeepIndexedAlpha, strs.TipPngKeepIndexedAlpha),
									declarative.Label{
										Text:    " ",
										MinSize: declarative.Size{Width: 150, Height: 0},
									},
									sf.makeCheckBoxWithTip(&sf.pngDitheringCheckBox, strs.TextPngEnableDithering, set.AdvancedPngEnableDithering, strs.TipPngEnableDithering),
								},
							},
							// WebP 设置容器
							declarative.GroupBox{
								Title:  strs.TextAdvWebPSettings,
								Layout: declarative.Grid{Columns: 2},
								Children: []declarative.Widget{
									sf.makeLabelWithTip(strs.TextAdvFineQuality, strs.TipAdvFineQualityWebP),
									declarative.Composite{
										Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
										Children: []declarative.Widget{
											declarative.HSpacer{},
											declarative.NumberEdit{
												AssignTo:    &sf.webpQualityEdit,
												Value:       float64(set.AdvancedWebPQuality),
												MinValue:    0,
												MaxValue:    100,
												Decimals:    0,
												ToolTipText: strs.TipAdvFineQualityWebP,
												MinSize:     declarative.Size{Width: settingControlWidth, Height: 0},
												MaxSize:     declarative.Size{Width: settingControlWidth, Height: 0},
											},
										},
									},
								},
							},
							declarative.VSpacer{},
							declarative.Composite{
								Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
								Children: []declarative.Widget{
									declarative.HSpacer{},
									declarative.Composite{
										Layout: declarative.HBox{Spacing: 4, MarginsZero: true},
										Children: []declarative.Widget{
											declarative.HSpacer{},
											declarative.CheckBox{
												AssignTo: &sf.debugModeCheck,
												Text:     strs.TextDebugMode,
												Checked:  false,
											},
											declarative.LinkLabel{
												Text:        "<a>(?)</a>",
												ToolTipText: strs.TipDebugMode,
												OnLinkActivated: func(link *walk.LinkLabelLink) {
													var parentHwnd uintptr
													if sf.Dialog != nil {
														parentHwnd = uintptr(sf.Dialog.Handle())
													}
													dialog.ShowInfoWithTitleAndParent(parentHwnd, strs.TextDebugMode, strs.TipDebugMode)
												},
											},
										},
									},
								},
							},
						},
					},
				},
			},
			// 底部按钮
			declarative.Composite{
				Layout: declarative.HBox{
					Margins: declarative.Margins{Top: 5, Bottom: 2},
					Spacing: 10,
				},
				Children: []declarative.Widget{
					declarative.HSpacer{},
					declarative.PushButton{
						Text:      strs.TextCancel,
						OnClicked: func() { sf.Cancel() },
					},
					declarative.PushButton{
						Text: strs.TextOK,
						OnClicked: func() {
							if sf.saveSetting() {
								sf.Accept()
							}
						},
					},
				},
			},
		},
	}.Create(owner)

	if err != nil {
		return err
	}

	ApplyInheritedTopMost(sf.Dialog)
	CenterWindow(sf.Dialog)

	if set.CompressType == settingLoader.CompressQuality {
		sf.qualityRadio.SetChecked(true)
	} else {
		sf.fileSizeRadio.SetChecked(true)
	}
	sf.onCompressModeChange()
	sf.onOutputTypeChange()
	sf.updateDebugMode()

	sf.Dialog.Run()
	return nil
}

// onCompressModeChange 响应压缩模式切换, 启用或禁用相关控件.
func (sf *SettingForm) onCompressModeChange() {
	if sf.qualityRadio == nil || sf.fileSizeRadio == nil {
		return
	}
	isQualityMode := sf.qualityRadio.Checked()
	if sf.qualityLabel != nil {
		sf.qualityLabel.SetEnabled(isQualityMode)
	}
	if sf.qualitySlider != nil {
		sf.qualitySlider.SetEnabled(isQualityMode)
	}
	if sf.limitSizeEdit != nil {
		sf.limitSizeEdit.SetEnabled(!isQualityMode)
	}
	if sf.sizeUnitCombo != nil {
		sf.sizeUnitCombo.SetEnabled(!isQualityMode)
	}
	if sf.acceptExceedCheck != nil {
		sf.acceptExceedCheck.SetEnabled(!isQualityMode)
	}
}

// onQualitySliderChange 响应画质滑块值变化, 更新画质等级标签文字.
func (sf *SettingForm) onQualitySliderChange() {
	if sf.qualitySlider == nil || sf.qualityLabel == nil {
		return
	}
	val := sf.qualitySlider.Value()
	level := sliderValueToQualityLevel(val)
	sf.qualityLabel.SetText(qualityLevelToString(level))
}

// onOutputTypeChange 响应输出方式切换, 启用或禁用输出格式相关控件.
func (sf *SettingForm) onOutputTypeChange() {
	if sf.outputTypeCombo == nil {
		return
	}
	isCoverOrigin := sf.outputTypeCombo.CurrentIndex() == 1
	enabled := !isCoverOrigin
	if sf.extensionCombo != nil {
		sf.extensionCombo.SetEnabled(enabled)
	}
	if sf.filenameEdit != nil {
		sf.filenameEdit.SetEnabled(enabled)
	}
	if sf.startIndexEdit != nil {
		sf.startIndexEdit.SetEnabled(enabled)
	}
}

// onThreadEditChange 响应线程数输入框变化, 同步更新滑块位置.
func (sf *SettingForm) onThreadEditChange() {
	if sf.maxThreadsEdit != nil && sf.threadSlider != nil {
		sf.threadSlider.SetValue(int(sf.maxThreadsEdit.Value()))
	}
}

// onThreadSliderChange 响应线程数滑块变化, 同步更新输入框数值.
func (sf *SettingForm) onThreadSliderChange() {
	if sf.threadSlider != nil && sf.maxThreadsEdit != nil {
		sf.maxThreadsEdit.SetValue(float64(sf.threadSlider.Value()))
	}
}

// onScaleModeChange 响应缩放方式切换, 启用或禁用缩放宽高输入框.
func (sf *SettingForm) onScaleModeChange() {
	if sf.scaleCombo == nil {
		return
	}
	enabled := sf.scaleCombo.CurrentIndex() != 0
	if sf.scaleWidth != nil {
		sf.scaleWidth.SetEnabled(enabled)
	}
	if sf.scaleHeight != nil {
		sf.scaleHeight.SetEnabled(enabled)
	}
}

// saveSetting 从 UI 控件读取所有值, 进行校验后写入全局配置.
// 返回 false 表示用户取消保存 (校验失败或用户取消了警告弹窗).
func (sf *SettingForm) saveSetting() bool {
	// 使用 workingCopy 作为基础, 保留旧值用于回调判断
	oldTopMost := sf.workingCopy.TopMost
	oldOutputType := sf.workingCopy.OutputType
	var parentHwnd uintptr
	if sf.Dialog != nil {
		parentHwnd = uintptr(sf.Dialog.Handle())
	}

	// ============================================================
	// 第一阶段: 从 UI 控件读取所有值到 workingCopy
	// ============================================================
	if sf.qualityRadio != nil {
		if sf.qualityRadio.Checked() {
			sf.workingCopy.CompressType = settingLoader.CompressQuality
		} else {
			sf.workingCopy.CompressType = settingLoader.CompressFileSize
		}
	}

	if sf.qualitySlider != nil {
		sf.workingCopy.Quality = sliderValueToQualityLevel(sf.qualitySlider.Value())
	} else {
		sf.workingCopy.Quality = settingLoader.QualityLevelClear
	}

	if sf.limitSizeEdit != nil {
		sf.workingCopy.LimitSize = int64(sf.limitSizeEdit.Value())
	}
	if sf.sizeUnitCombo != nil {
		switch sf.sizeUnitCombo.CurrentIndex() {
		case 0:
			sf.workingCopy.SizeUnit = settingLoader.UnitKB
		case 1:
			sf.workingCopy.SizeUnit = settingLoader.UnitMB
		}
	}
	if sf.acceptExceedCheck != nil {
		sf.workingCopy.AcceptExceed = sf.acceptExceedCheck.Checked()
	}

	switch sf.outputTypeCombo.CurrentIndex() {
	case 0:
		sf.workingCopy.OutputType = settingLoader.OutputDirection
	case 1:
		sf.workingCopy.OutputType = settingLoader.OutputCoverOrigin
	case 2:
		sf.workingCopy.OutputType = settingLoader.OutputStructure
	}

	switch sf.extensionCombo.CurrentIndex() {
	case 0:
		sf.workingCopy.Extension = settingLoader.ExtJPEG
	case 1:
		sf.workingCopy.Extension = settingLoader.ExtPNG
	case 2:
		sf.workingCopy.Extension = settingLoader.ExtWebP
	case 3:
		sf.workingCopy.Extension = settingLoader.ExtOrigin
	}

	if sf.filenameEdit != nil {
		sf.workingCopy.OutputFilename = sf.filenameEdit.Text()
	}
	if sf.startIndexEdit != nil {
		sf.workingCopy.StartIndex = int(sf.startIndexEdit.Value())
	}

	if sf.maxThreadsEdit != nil {
		sf.workingCopy.MaxThreads = int(sf.maxThreadsEdit.Value())
	}
	if sf.topMostCheck != nil {
		sf.workingCopy.TopMost = sf.topMostCheck.Checked()
	}

	if sf.alphaCombo != nil {
		switch sf.alphaCombo.CurrentIndex() {
		case 0:
			sf.workingCopy.AlphaHandle = settingLoader.AlphaKeep
		case 1:
			sf.workingCopy.AlphaHandle = settingLoader.AlphaSmartRemove
		case 2:
			sf.workingCopy.AlphaHandle = settingLoader.AlphaRemove
		}
	}

	if sf.scaleCombo != nil {
		switch sf.scaleCombo.CurrentIndex() {
		case 0:
			sf.workingCopy.Scale = settingLoader.ScaleNone
		case 1:
			sf.workingCopy.Scale = settingLoader.ScaleStretch
		case 2:
			sf.workingCopy.Scale = settingLoader.ScaleFitOutside
		case 3:
			sf.workingCopy.Scale = settingLoader.ScaleFitInside
		case 4:
			sf.workingCopy.Scale = settingLoader.ScaleFitOutsideCrop
		case 5:
			sf.workingCopy.Scale = settingLoader.ScaleLockSide
		}
	}
	if sf.scaleWidth != nil {
		sf.workingCopy.ScaleWidth = int(sf.scaleWidth.Value())
	}
	if sf.scaleHeight != nil {
		sf.workingCopy.ScaleHeight = int(sf.scaleHeight.Value())
	}

	if sf.jpegQualityEdit != nil {
		sf.workingCopy.AdvancedJpegQuality = int(sf.jpegQualityEdit.Value())
	}
	if sf.webpQualityEdit != nil {
		sf.workingCopy.AdvancedWebPQuality = int(sf.webpQualityEdit.Value())
	}

	if sf.pngKeepAlphaCheckBox != nil {
		sf.workingCopy.AdvancedPngKeepIndexedAlpha = sf.pngKeepAlphaCheckBox.Checked()
	}

	if sf.pngPaletteAlgoCombo != nil {
		switch sf.pngPaletteAlgoCombo.CurrentIndex() {
		case 0:
			sf.workingCopy.AdvancedPngPaletteAlgo = settingLoader.PaletteMedianCut
		case 1:
			sf.workingCopy.AdvancedPngPaletteAlgo = settingLoader.PaletteKMeans
		}
	}

	if sf.pngDitheringCheckBox != nil {
		sf.workingCopy.AdvancedPngEnableDithering = sf.pngDitheringCheckBox.Checked()
	}

	// ============================================================
	// 第二阶段: 错误检测
	// ============================================================
	errors := sf.workingCopy.CheckErrors()
	if len(errors) > 0 {
		errMsg := strings.Join(errors, "\n")
		dialog.ShowErrorWithParent(parentHwnd, errMsg)
		return false
	}

	// ============================================================
	// 第三阶段: 警告检测
	// ============================================================
	warnings := sf.workingCopy.CheckWarnings()
	if len(warnings) > 0 {
		warnMsg := strings.Join(warnings, "\n")
		if !dialog.ShowConfirmWithParent(parentHwnd, warnMsg) {
			return false
		}
	}

	// ============================================================
	// 第四阶段: 写入 settingLoader 持久化
	// ============================================================
	settingLoader.UpdateSetting(sf.workingCopy)

	if sf.workingCopy.TopMost != oldTopMost && sf.onTopMostChanged != nil {
		sf.onTopMostChanged(sf.workingCopy.TopMost)
	}
	if sf.workingCopy.OutputType != oldOutputType && sf.onOutputTypeChanged != nil {
		sf.onOutputTypeChanged(sf.workingCopy.OutputType)
	}

	return true
}

// 辅助函数: 输出类型转字符串
func outputTypeToString(t settingLoader.OutputType) string {
	switch t {
	case settingLoader.OutputCoverOrigin:
		return strs.TextCoverOrigin
	case settingLoader.OutputDirection:
		return strs.TextOutputDir
	case settingLoader.OutputStructure:
		return strs.TextOutputStruct
	default:
		return strs.TextOutputDir
	}
}

// 辅助函数: 扩展名类型转字符串
func extensionToString(t settingLoader.ExtensionType) string {
	switch t {
	case settingLoader.ExtJPEG:
		return strs.TextFormatJPEG
	case settingLoader.ExtPNG:
		return strs.TextFormatPNG
	case settingLoader.ExtWebP:
		return strs.TextFormatWebP
	case settingLoader.ExtOrigin:
		return strs.TextFormatOrigin
	default:
		return strs.TextFormatJPEG
	}
}

// 辅助函数: 透明通道处理方式转字符串
func alphaHandleToString(t settingLoader.AlphaHandleType) string {
	switch t {
	case settingLoader.AlphaKeep:
		return strs.AlphaKeepText
	case settingLoader.AlphaSmartRemove:
		return strs.AlphaSmartRemoveText
	case settingLoader.AlphaRemove:
		return strs.AlphaRemoveText
	default:
		return strs.AlphaKeepText
	}
}

// 辅助函数: 缩放方式转字符串
func scaleToString(t settingLoader.ScaleType) string {
	switch t {
	case settingLoader.ScaleNone:
		return strs.ScaleNoneText
	case settingLoader.ScaleStretch:
		return strs.ScaleStretchText
	case settingLoader.ScaleFitOutside:
		return strs.ScaleFitOutText
	case settingLoader.ScaleFitInside:
		return strs.ScaleFitInText
	case settingLoader.ScaleFitOutsideCrop:
		return strs.ScaleFitOutCrop
	case settingLoader.ScaleLockSide:
		return strs.ScaleLockSideText
	default:
		return strs.ScaleNoneText
	}
}

// 辅助函数: 调色盘生成算法转字符串
func paletteAlgoToString(t settingLoader.PaletteAlgoType) string {
	switch t {
	case settingLoader.PaletteMedianCut:
		return strs.TextPaletteMedianCut
	case settingLoader.PaletteKMeans:
		return strs.TextPaletteKMeans
	default:
		return strs.TextPaletteMedianCut
	}
}

// 辅助函数: 字符串转整数
func atoi(s string) int {
	v, _ := strconv.Atoi(s)
	return v
}

// 辅助函数: 文件大小单位转字符串
func sizeUnitToString(unit settingLoader.SizeUnit) string {
	switch unit {
	case settingLoader.UnitKB:
		return strs.TextUnitKB
	case settingLoader.UnitMB:
		return strs.TextUnitMB
	default:
		return strs.TextUnitKB
	}
}

// 辅助函数: 画质等级转字符串
func qualityLevelToString(level settingLoader.QualityLevel) string {
	switch level {
	case settingLoader.QualityLevelBest:
		return strs.TextQualityBest
	case settingLoader.QualityLevelClear:
		return strs.TextQualityClear
	case settingLoader.QualityLevelNormal:
		return strs.TextQualityNormal
	case settingLoader.QualityLevelPoor:
		return strs.TextQualityPoor
	default:
		return strs.TextQualityClear
	}
}

// 辅助函数: 将 core.QualityLevel 转换为滑块刻度值 (0-3)
func qualityLevelToSliderValue(level settingLoader.QualityLevel) int {
	switch level {
	case settingLoader.QualityLevelPoor:
		return 0
	case settingLoader.QualityLevelNormal:
		return 1
	case settingLoader.QualityLevelClear:
		return 2
	case settingLoader.QualityLevelBest:
		return 3
	default:
		return 2
	}
}

// 辅助函数: 将滑块刻度值 (0-3) 转换为 core.QualityLevel
func sliderValueToQualityLevel(val int) settingLoader.QualityLevel {
	switch val {
	case 0:
		return settingLoader.QualityLevelPoor
	case 1:
		return settingLoader.QualityLevelNormal
	case 2:
		return settingLoader.QualityLevelClear
	case 3:
		return settingLoader.QualityLevelBest
	default:
		return settingLoader.QualityLevelClear
	}
}

// 更新调试按钮信息
func (sf *SettingForm) updateDebugMode() {
	if sf.debugModeCheck != nil {
		sf.debugModeCheck.SetChecked(settingLoader.IsDebug())
		sf.debugModeCheck.SetEnabled(!settingLoader.IsDebug())
		sf.debugModeCheck.CheckedChanged().Attach(func() {
			if sf.debugModeCheck.Checked() {
				var parentHwnd uintptr
				if sf.Dialog != nil {
					parentHwnd = uintptr(sf.Dialog.Handle())
				}

				confirmed := dialog.ShowConfirmWithParent(parentHwnd, strs.TipDebugMode)

				// 如果启用调试, 则以 --debug 参数重启
				if confirmed {
					exePath, err := os.Executable()
					if err != nil {
						dialog.ShowErrorWithParent(parentHwnd, strs.ErrCannotFindExecPath)
						sf.debugModeCheck.SetChecked(false)
					}

					cmd := exec.Command(exePath, "--debug")
					cmd.Start()
					os.Exit(0)
				} else {
					sf.debugModeCheck.SetChecked(false)
				}
			}
		})
	}
}

// makeLabelWithTip 构造"文本标签 + (?) 提示链接"组合控件.
func (sf *SettingForm) makeLabelWithTip(label, tip string) declarative.Composite {
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
					if sf.Dialog != nil {
						parentHwnd = uintptr(sf.Dialog.Handle())
					}
					dialog.ShowInfoWithTitleAndParent(parentHwnd, label, tip)
				},
			},
		},
	}
}

// makeCheckBoxWithTip 构造"复选框 + (?) 提示链接"组合控件.
func (sf *SettingForm) makeCheckBoxWithTip(assignTo **walk.CheckBox, text string, checked bool, tip string) declarative.Composite {
	return declarative.Composite{
		Layout: declarative.HBox{SpacingZero: true, MarginsZero: true},
		Children: []declarative.Widget{
			declarative.HSpacer{},
			declarative.Composite{
				Layout:  declarative.HBox{Spacing: 4, MarginsZero: true},
				MinSize: declarative.Size{Width: settingControlWidth, Height: 0},
				MaxSize: declarative.Size{Width: settingControlWidth, Height: 0},
				Children: []declarative.Widget{
					declarative.CheckBox{
						AssignTo: assignTo,
						Text:     text,
						Checked:  checked,
					},
					declarative.LinkLabel{
						Text:        "<a>(?)</a>",
						ToolTipText: tip,
						OnLinkActivated: func(link *walk.LinkLabelLink) {
							var parentHwnd uintptr
							if sf.Dialog != nil {
								parentHwnd = uintptr(sf.Dialog.Handle())
							}
							dialog.ShowInfoWithTitleAndParent(parentHwnd, text, tip)
						},
					},
					declarative.HSpacer{},
				},
			},
		},
	}
}
