package settingLoader

import (
	strs "PicSizer/internal/core/strings"
	"strings"
)

// Setting 表示程序的所有配置项.
// 配置存储在全局变量 CurrentSetting 中, 程序启动时初始化为默认值.
type Setting struct {
	// 压缩设置
	CompressType CompressType // 压缩类型
	Quality      QualityLevel // 画质等级 (最佳/清晰/一般/较差), 内部映射到具体画质值
	LimitSize    int64        // 限制大小 (数值)
	SizeUnit     SizeUnit     // 限制大小单位 (KB/MB)
	AcceptExceed bool         // 是否接受超出大小的图片

	// 输出设置
	OutputType     OutputType    // 输出方式
	Extension      ExtensionType // 输出格式
	OutputFilename string        // 输出文件名模板
	StartIndex     int           // 输出图片的起始下标

	// 系统设置
	MaxThreads int  // 最大线程数
	TopMost    bool // 窗体置顶

	// 预处理设置
	AlphaHandle AlphaHandleType // 透明通道处理方式
	Scale       ScaleType       // 缩放方式
	ScaleWidth  int             // 目标宽度 (像素, 0 表示不限制)
	ScaleHeight int             // 目标高度 (像素, 0 表示不限制)

	// 按画质压缩时, 各格式的精细化画质覆盖.
	// 取值范围 0-100: 0 表示不启用 (按全局 QualityLevel 处理);
	// 非 0 表示覆盖对应格式的画质值, 由压缩代码自行读取使用.
	AdvancedJpegQuality int // JPEG 精细化画质 (0-100, 0 表示未启用)
	AdvancedWebPQuality int // WebP 精细化画质 (0-100, 0 表示未启用)

	// PNG 有损压缩时, 调色盘生成算法.
	AdvancedPngPaletteAlgo PaletteAlgoType

	// PNG 有损压缩时, 是否在量化过程中为索引格式预留透明像素.
	AdvancedPngKeepIndexedAlpha bool

	// PNG 有损压缩时, 是否启用 Floyd-Steinberg 抖动算法.
	AdvancedPngEnableDithering bool
}

var (
	// CurrentSetting 当前设置
	CurrentSetting Setting

	// debug 是否启用调试, 仅在程序启动时由参数设定, 无法自行修改
	debug bool
)

const (
	// HeightWidthMaxValue 宽高最大值
	HeightWidthMaxValue = 100000000
	// LimitSizeMaxValue 限制文件大小的最大值
	LimitSizeMaxValue = 100000
	// StartIndexMaxValue 起始下标最大值
	StartIndexMaxValue = 100000000
)

// DefaultSetting 表示配置的默认值.
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

// InitSetting 将 CurrentSetting 重置为默认值.
func InitSetting() {
	CurrentSetting = DefaultSetting
}

// GetSetting 返回当前配置的副本.
func GetSetting() Setting {
	return CurrentSetting
}

// GetSettingCopy 返回当前设置的深拷贝.
// 对拷贝的修改不会影响实际设置, 适用于需要临时修改配置的场景.
func GetSettingCopy() Setting {
	return CurrentSetting
}

// CheckErrors 检查设置中的错误, 返回错误信息数组.
// 如果返回值为空, 表示没有错误.
func (s Setting) CheckErrors() []string {
	var errors []string

	// 检查缩放方式为等比锁定单边时, 宽和高必须有一项为 0, 另一项不为 0
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

// CheckWarnings 检查设置中的警告, 返回警告信息数组.
// 如果返回值为空, 表示没有警告.
func (s Setting) CheckWarnings() []string {
	var warnings []string

	// 检查文件名模板是否包含 {id} 或 {name}
	if s.OutputType != OutputCoverOrigin {
		hasID := strings.Contains(s.OutputFilename, "{id}")
		hasName := strings.Contains(s.OutputFilename, "{name}")
		if !hasID && !hasName {
			warnings = append(warnings, strs.WarnOutputNameIsFixed)
		}
	}

	return warnings
}

// UpdateSetting 用指定配置更新 CurrentSetting.
func UpdateSetting(s Setting) {
	CurrentSetting = s
}

// EnableDebug 启用调试, 且启用后无法关闭
func EnableDebug() {
	debug = true
}

// IsDebug 获取调试状态
func IsDebug() bool {
	return debug
}

// UpdateSettingWithOutputType 仅更新当前配置的输出方式字段.
func UpdateSettingWithOutputType(ot OutputType) {
	CurrentSetting.OutputType = ot
}

// GetExtensionString 根据扩展名类型返回对应的文件后缀字符串.
// 若传入未知类型, 返回空字符串.
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
