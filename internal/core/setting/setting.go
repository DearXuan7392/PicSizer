package setting

import (
	"sync"
)

// Setting 配置结构体
type Setting struct {
	// 压缩设置
	CompressType CompressType // 压缩类型
	Quality      QualityLevel // 画质等级 (最佳/清晰/一般/较差), 内部映射到具体画质值
	LimitSize    int64        // 限制大小（数值）
	SizeUnit     SizeUnit     // 限制大小单位（KB/MB）
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
	JpegQuality int // JPEG 精细化画质 (0-100, 0 表示未启用)
	WebPQuality int // WebP 精细化画质 (0-100, 0 表示未启用)

	// PNG 有损压缩时, 是否在量化过程中为索引格式预留透明像素.
	// 该字段由用户在高级设置中勾选, 由 PNG 压缩代码自行读取使用.
	PngKeepIndexedAlpha bool
}

// 全局配置变量
var (
	CurrentSetting Setting
	mu             sync.RWMutex
)

// 默认配置
var DefaultSetting = Setting{
	CompressType:        CompressQuality,
	Quality:             QualityLevelClear, // 默认清晰画质 (映射后为 80)
	LimitSize:           400,
	SizeUnit:            UnitKB,
	AcceptExceed:        false,
	OutputType:          OutputDirection,
	Extension:           ExtJPEG,
	OutputFilename:      "{id}",
	StartIndex:          1,
	MaxThreads:          2,
	TopMost:             true,
	AlphaHandle:         AlphaKeep,
	Scale:               ScaleNone,
	ScaleWidth:          1920,
	ScaleHeight:         1080,
	JpegQuality:         0,    // 默认不启用精细化画质
	WebPQuality:         0,    // 默认不启用精细化画质
	PngKeepIndexedAlpha: true, // 默认在 PNG 索引格式压缩时预留透明像素
}

// InitSetting 初始化配置为默认值
func InitSetting() {
	CurrentSetting = DefaultSetting
}

// GetSetting 获取当前配置
func GetSetting() Setting {
	return CurrentSetting
}

// UpdateSetting 更新配置
func UpdateSetting(s Setting) {
	CurrentSetting = s
}

// UpdateSettingWithOutputType 仅更新输出类型
func UpdateSettingWithOutputType(ot OutputType) {
	CurrentSetting.OutputType = ot
}

// GetExtensionString 根据扩展名类型获取后缀字符串
func GetExtensionString(ext ExtensionType) string {
	switch ext {
	case ExtJPEG:
		return ".jpg"
	case ExtPNG:
		return ".png"
	case ExtWebP:
		return ".webp"
	default:
		return ""
	}
}
