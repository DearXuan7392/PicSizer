package settingLoader

import (
	strs "PicSizer/internal/core/strings"
	"strings"
)

// Setting 程序的全部配置项.
// 存储在全局变量 CurrentSetting 中, 程序启动时初始化为默认值.
type Setting struct {
	// 压缩设置
	CompressType CompressType
	Quality      QualityLevel // 画质等级, 内部映射到具体画质值
	LimitSize    int64        // 限制大小, 单位由 SizeUnit 决定
	SizeUnit     SizeUnit
	AcceptExceed bool         // 是否接受超出大小限制的图片

	// 输出设置
	OutputType     OutputType
	Extension      ExtensionType
	OutputFilename string // 输出文件名模板, 支持 {id} 与 {name}
	StartIndex     int    // 文件名模板中 {id} 的起始序号

	// 系统设置
	MaxThreads int
	TopMost    bool

	// 预处理设置
	AlphaHandle AlphaHandleType
	Scale       ScaleType
	ScaleWidth  int // 目标宽度 (像素, 0 表示自动)
	ScaleHeight int // 目标高度 (像素, 0 表示自动)

	// 按画质压缩时各格式的精细化画质覆盖.
	// 0 表示不启用 (按全局 QualityLevel 处理), 非 0 表示覆盖对应格式的画质值.
	AdvancedJpegQuality int
	AdvancedWebPQuality int

	// PNG 有损压缩时, 调色盘生成算法.
	AdvancedPngPaletteAlgo PaletteAlgoType

	// PNG 有损压缩时, 是否在量化过程中为索引格式预留透明像素.
	AdvancedPngKeepIndexedAlpha bool

	// PNG 有损压缩时, 是否启用 Floyd-Steinberg 抖动算法.
	AdvancedPngEnableDithering bool
}

var (
	CurrentSetting Setting

	// debug 仅在程序启动时由参数设定, 运行期间无法修改
	debug bool
)

const (
	HeightWidthMaxValue = 100000000
	LimitSizeMaxValue   = 100000
	StartIndexMaxValue  = 100000000
)

var DefaultSetting = Setting{
	CompressType:                CompressQuality,
	Quality:                     QualityLevelClear,
	LimitSize:                   400,
	SizeUnit:                    UnitKB,
	AcceptExceed:                false,
	OutputType:                  OutputDirection,
	Extension:                   ExtJPEG,
	OutputFilename:              "{name}",
	StartIndex:                  1,
	MaxThreads:                  2,
	TopMost:                     false,
	AlphaHandle:                 AlphaKeep,
	Scale:                       ScaleNone,
	ScaleWidth:                  1920,
	ScaleHeight:                 1080,
	AdvancedJpegQuality:         0,
	AdvancedWebPQuality:         0,
	AdvancedPngPaletteAlgo:      PaletteMedianCut,
	AdvancedPngKeepIndexedAlpha: true,
	AdvancedPngEnableDithering:  true,
}

func InitSetting() {
	CurrentSetting = DefaultSetting
}

func GetSetting() Setting {
	return CurrentSetting
}

// GetSettingCopy 返回当前设置的副本, 修改不影响实际设置.
func GetSettingCopy() Setting {
	return CurrentSetting
}

// CheckErrors 校验设置中的数值约束, 返回错误信息数组, 为空表示没有错误.
func (s Setting) CheckErrors() []string {
	var errors []string

	// 等比锁定单边时宽高必须恰有一项为 0, 其余缩放方式均需为正
	if s.Scale == ScaleLockSide {
		w0 := s.ScaleWidth == 0
		h0 := s.ScaleHeight == 0
		if (w0 && h0) || (!w0 && !h0) {
			errors = append(errors, strs.ErrHWMustOneZero)
		}
	} else if s.Scale != ScaleNone {
		if s.ScaleWidth <= 0 || s.ScaleHeight <= 0 {
			errors = append(errors, strs.ErrHWMustBePositive)
		}
	}

	return errors
}

// CheckWarnings 校验设置中的警告项, 返回警告信息数组, 为空表示没有警告.
func (s Setting) CheckWarnings() []string {
	var warnings []string

	// 模板不含 {id}/{name} 时多张图片会撞名覆盖, 仅在非覆盖模式下提示
	if s.OutputType != OutputCoverOrigin {
		hasID := strings.Contains(s.OutputFilename, "{id}")
		hasName := strings.Contains(s.OutputFilename, "{name}")
		if !hasID && !hasName {
			warnings = append(warnings, strs.WarnOutputNameIsFixed)
		}
	}

	return warnings
}

func UpdateSetting(s Setting) {
	CurrentSetting = s
}

// EnableDebug 启用调试, 启用后无法关闭.
func EnableDebug() {
	debug = true
}

func IsDebug() bool {
	return debug
}

func UpdateSettingWithOutputType(ot OutputType) {
	CurrentSetting.OutputType = ot
}

// GetExtensionString 返回扩展名类型对应的文件后缀 (含点号), 未知类型返回空字符串.
func GetExtensionString(ext ExtensionType) string {
	switch ext {
	case ExtJPEG:
		return ".jpg"
	case ExtPNG:
		return ".png"
	case ExtWebP:
		return ".webp"
	case ExtBMP:
		return ".bmp"
	default:
		return ""
	}
}
