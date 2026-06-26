package ui

import (
	"PicSizer/internal/core/setting"
	strs "PicSizer/internal/core/strings"
	"PicSizer/internal/dialog"
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

	maxThreads int

	onTopMostChanged    func(bool)
	onOutputTypeChanged func(setting.OutputType)
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
func (sf *SettingForm) SetOnOutputTypeChanged(fn func(setting.OutputType)) {
	sf.onOutputTypeChanged = fn
}

// Show 显示设置窗口. 窗口包含三个选项卡页面, 关闭后调用方可通过回调获取用户修改的设置.
func (sf *SettingForm) Show(owner walk.Form, appIcon *walk.Icon) error {
	set := setting.GetSetting()
	// 获取 CPU 逻辑核心数
	sf.maxThreads = runtime.NumCPU()

	// 判断当前压缩模式, 设置输入框禁用状态
	isQualityMode := (set.CompressType == setting.CompressQuality)

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
												Enabled:  set.Scale != setting.ScaleNone,
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
												Enabled:  set.Scale != setting.ScaleNone,
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

	if set.CompressType == setting.CompressQuality {
		sf.qualityRadio.SetChecked(true)
	} else {
		sf.fileSizeRadio.SetChecked(true)
	}
	sf.onCompressModeChange()
	sf.onOutputTypeChange()

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
// 返回 false 表示用户取消保存（校验失败或用户取消了警告弹窗）.
func (sf *SettingForm) saveSetting() bool {
	set := setting.GetSetting()
	oldTopMost := set.TopMost
	oldOutputType := set.OutputType
	var parentHwnd uintptr
	if sf.Dialog != nil {
		parentHwnd = uintptr(sf.Dialog.Handle())
	}

	// ============================================================
	// 第一阶段: 从 UI 控件读取所有值到本地变量
	// ============================================================
	var newCompressType setting.CompressType
	if sf.qualityRadio != nil {
		if sf.qualityRadio.Checked() {
			newCompressType = setting.CompressQuality
		} else {
			newCompressType = setting.CompressFileSize
		}
	}

	var newQuality setting.QualityLevel
	if sf.qualitySlider != nil {
		newQuality = sliderValueToQualityLevel(sf.qualitySlider.Value())
	} else {
		newQuality = setting.QualityLevelClear
	}

	var newLimitSize int64
	if sf.limitSizeEdit != nil {
		newLimitSize = int64(sf.limitSizeEdit.Value())
	}
	var newSizeUnit setting.SizeUnit
	if sf.sizeUnitCombo != nil {
		switch sf.sizeUnitCombo.CurrentIndex() {
		case 0:
			newSizeUnit = setting.UnitKB
		case 1:
			newSizeUnit = setting.UnitMB
		}
	}
	var newAcceptExceed bool
	if sf.acceptExceedCheck != nil {
		newAcceptExceed = sf.acceptExceedCheck.Checked()
	}

	var newOutputType setting.OutputType
	switch sf.outputTypeCombo.CurrentIndex() {
	case 0:
		newOutputType = setting.OutputDirection
	case 1:
		newOutputType = setting.OutputCoverOrigin
	case 2:
		newOutputType = setting.OutputStructure
	}

	var newExtension setting.ExtensionType
	switch sf.extensionCombo.CurrentIndex() {
	case 0:
		newExtension = setting.ExtJPEG
	case 1:
		newExtension = setting.ExtPNG
	case 2:
		newExtension = setting.ExtWebP
	case 3:
		newExtension = setting.ExtOrigin
	}

	var newOutputFilename string
	if sf.filenameEdit != nil {
		newOutputFilename = sf.filenameEdit.Text()
	}
	var newStartIndex int
	if sf.startIndexEdit != nil {
		newStartIndex = int(sf.startIndexEdit.Value())
	}

	var newMaxThreads int
	if sf.maxThreadsEdit != nil {
		newMaxThreads = int(sf.maxThreadsEdit.Value())
	}
	var newTopMost bool
	if sf.topMostCheck != nil {
		newTopMost = sf.topMostCheck.Checked()
	}

	var newAlphaHandle setting.AlphaHandleType
	if sf.alphaCombo != nil {
		switch sf.alphaCombo.CurrentIndex() {
		case 0:
			newAlphaHandle = setting.AlphaKeep
		case 1:
			newAlphaHandle = setting.AlphaSmartRemove
		case 2:
			newAlphaHandle = setting.AlphaRemove
		}
	}

	var newScale setting.ScaleType
	if sf.scaleCombo != nil {
		switch sf.scaleCombo.CurrentIndex() {
		case 0:
			newScale = setting.ScaleNone
		case 1:
			newScale = setting.ScaleStretch
		case 2:
			newScale = setting.ScaleFitOutside
		case 3:
			newScale = setting.ScaleFitInside
		case 4:
			newScale = setting.ScaleFitOutsideCrop
		case 5:
			newScale = setting.ScaleLockSide
		}
	}
	var newScaleWidth int
	if sf.scaleWidth != nil {
		newScaleWidth = int(sf.scaleWidth.Value())
	}
	var newScaleHeight int
	if sf.scaleHeight != nil {
		newScaleHeight = int(sf.scaleHeight.Value())
	}

	var newJpegQuality int
	if sf.jpegQualityEdit != nil {
		newJpegQuality = int(sf.jpegQualityEdit.Value())
	}
	var newWebPQuality int
	if sf.webpQualityEdit != nil {
		newWebPQuality = int(sf.webpQualityEdit.Value())
	}

	var newPngKeepIndexedAlpha bool
	if sf.pngKeepAlphaCheckBox != nil {
		newPngKeepIndexedAlpha = sf.pngKeepAlphaCheckBox.Checked()
	}

	var newPngPaletteAlgo setting.PaletteAlgoType
	if sf.pngPaletteAlgoCombo != nil {
		switch sf.pngPaletteAlgoCombo.CurrentIndex() {
		case 0:
			newPngPaletteAlgo = setting.PaletteMedianCut
		case 1:
			newPngPaletteAlgo = setting.PaletteKMeans
		}
	}

	var newPngEnableDithering bool
	if sf.pngDitheringCheckBox != nil {
		newPngEnableDithering = sf.pngDitheringCheckBox.Checked()
	}

	// ============================================================
	// 第二阶段: 错误检测
	// ============================================================
	if newScale == setting.ScaleLockSide {
		w0 := newScaleWidth == 0
		h0 := newScaleHeight == 0
		if (w0 && h0) || (!w0 && !h0) {
			dialog.ShowErrorWithParent(parentHwnd, strs.ErrScaleLockSideInvalid)
			return false
		}
	}

	// ============================================================
	// 第三阶段: 警告检测
	// ============================================================
	if newOutputType != setting.OutputCoverOrigin {
		hasID := strings.Contains(newOutputFilename, "{id}")
		hasName := strings.Contains(newOutputFilename, "{name}")
		if !hasID && !hasName {
			if !dialog.ShowConfirmWithParent(parentHwnd, strs.WarnFilenameTplMissing) {
				return false
			}
		}
	}

	// ============================================================
	// 第四阶段: 写入 setting 持久化
	// ============================================================
	set.CompressType = newCompressType
	set.Quality = newQuality
	set.LimitSize = newLimitSize
	set.SizeUnit = newSizeUnit
	set.AcceptExceed = newAcceptExceed
	set.OutputType = newOutputType
	set.Extension = newExtension
	set.OutputFilename = newOutputFilename
	set.StartIndex = newStartIndex
	set.MaxThreads = newMaxThreads
	set.TopMost = newTopMost
	set.AlphaHandle = newAlphaHandle
	set.Scale = newScale
	set.ScaleWidth = newScaleWidth
	set.ScaleHeight = newScaleHeight
	set.AdvancedJpegQuality = newJpegQuality
	set.AdvancedWebPQuality = newWebPQuality
	set.AdvancedPngKeepIndexedAlpha = newPngKeepIndexedAlpha
	set.AdvancedPngPaletteAlgo = newPngPaletteAlgo
	set.AdvancedPngEnableDithering = newPngEnableDithering

	setting.UpdateSetting(set)

	if set.TopMost != oldTopMost && sf.onTopMostChanged != nil {
		sf.onTopMostChanged(set.TopMost)
	}
	if set.OutputType != oldOutputType && sf.onOutputTypeChanged != nil {
		sf.onOutputTypeChanged(set.OutputType)
	}

	return true
}

// 辅助函数: 输出类型转字符串
func outputTypeToString(t setting.OutputType) string {
	switch t {
	case setting.OutputCoverOrigin:
		return strs.TextCoverOrigin
	case setting.OutputDirection:
		return strs.TextOutputDir
	case setting.OutputStructure:
		return strs.TextOutputStruct
	default:
		return strs.TextOutputDir
	}
}

// 辅助函数: 扩展名类型转字符串
func extensionToString(t setting.ExtensionType) string {
	switch t {
	case setting.ExtJPEG:
		return strs.TextFormatJPEG
	case setting.ExtPNG:
		return strs.TextFormatPNG
	case setting.ExtWebP:
		return strs.TextFormatWebP
	case setting.ExtOrigin:
		return strs.TextFormatOrigin
	default:
		return strs.TextFormatJPEG
	}
}

// 辅助函数: 透明通道处理方式转字符串
func alphaHandleToString(t setting.AlphaHandleType) string {
	switch t {
	case setting.AlphaKeep:
		return strs.AlphaKeepText
	case setting.AlphaSmartRemove:
		return strs.AlphaSmartRemoveText
	case setting.AlphaRemove:
		return strs.AlphaRemoveText
	default:
		return strs.AlphaKeepText
	}
}

// 辅助函数: 缩放方式转字符串
func scaleToString(t setting.ScaleType) string {
	switch t {
	case setting.ScaleNone:
		return strs.ScaleNoneText
	case setting.ScaleStretch:
		return strs.ScaleStretchText
	case setting.ScaleFitOutside:
		return strs.ScaleFitOutText
	case setting.ScaleFitInside:
		return strs.ScaleFitInText
	case setting.ScaleFitOutsideCrop:
		return strs.ScaleFitOutCrop
	case setting.ScaleLockSide:
		return strs.ScaleLockSideText
	default:
		return strs.ScaleNoneText
	}
}

// 辅助函数: 调色盘生成算法转字符串
func paletteAlgoToString(t setting.PaletteAlgoType) string {
	switch t {
	case setting.PaletteMedianCut:
		return strs.TextPaletteMedianCut
	case setting.PaletteKMeans:
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
func sizeUnitToString(unit setting.SizeUnit) string {
	switch unit {
	case setting.UnitKB:
		return strs.TextUnitKB
	case setting.UnitMB:
		return strs.TextUnitMB
	default:
		return strs.TextUnitKB
	}
}

// 辅助函数: 画质等级转字符串
func qualityLevelToString(level setting.QualityLevel) string {
	switch level {
	case setting.QualityLevelBest:
		return strs.TextQualityBest
	case setting.QualityLevelClear:
		return strs.TextQualityClear
	case setting.QualityLevelNormal:
		return strs.TextQualityNormal
	case setting.QualityLevelPoor:
		return strs.TextQualityPoor
	default:
		return strs.TextQualityClear
	}
}

// 辅助函数: 将 core.QualityLevel 转换为滑块刻度值 (0-3)
func qualityLevelToSliderValue(level setting.QualityLevel) int {
	switch level {
	case setting.QualityLevelPoor:
		return 0
	case setting.QualityLevelNormal:
		return 1
	case setting.QualityLevelClear:
		return 2
	case setting.QualityLevelBest:
		return 3
	default:
		return 2
	}
}

// 辅助函数: 将滑块刻度值 (0-3) 转换为 core.QualityLevel
func sliderValueToQualityLevel(val int) setting.QualityLevel {
	switch val {
	case 0:
		return setting.QualityLevelPoor
	case 1:
		return setting.QualityLevelNormal
	case 2:
		return setting.QualityLevelClear
	case 3:
		return setting.QualityLevelBest
	default:
		return setting.QualityLevelClear
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
