package setting

// Setting 表示程序的所有配置项.
// 配置存储在全局变量 CurrentSetting 中, 程序启动时初始化为默认值.
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
	CurrentSetting Setting
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

// UpdateSetting 用指定配置更新 CurrentSetting.
func UpdateSetting(s Setting) {
	CurrentSetting = s
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
	default:
		return ""
	}
}
