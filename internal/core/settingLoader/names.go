package settingLoader

// 枚举值到英文名称的映射, 仅供日志输出使用, 与面向用户的 core/strings 文本无关.

var (
	compressTypeNames = []string{"quality", "size"}
	outputTypeNames   = []string{"cover", "structure", "direction"}
	extensionNames    = []string{"jpeg", "png", "webp", "bmp", "origin"}
	stateNames        = []string{"waiting", "compressing", "success", "out-of-limit", "error"}
	alphaHandleNames  = []string{"keep", "smart-remove", "remove"}
	scaleNames        = []string{"none", "stretch", "fit-outside", "fit-inside", "fit-outside-crop", "lock-side"}
	paletteAlgoNames  = []string{"median-cut", "kmeans"}
	sizeUnitNames     = []string{"KB", "MB"}
	qualityNames      = []string{"best", "clear", "normal", "poor"}
)

func enumName(names []string, index int) string {
	if index < 0 || index >= len(names) {
		return "unknown"
	}
	return names[index]
}

// CompressTypeName 返回压缩模式的英文名称.
func CompressTypeName(t CompressType) string { return enumName(compressTypeNames, int(t)) }

// OutputTypeName 返回输出方式的英文名称.
func OutputTypeName(t OutputType) string { return enumName(outputTypeNames, int(t)) }

// ExtensionName 返回输出格式的英文名称.
func ExtensionName(t ExtensionType) string { return enumName(extensionNames, int(t)) }

// PicItemStateName 返回项目状态的英文名称.
func PicItemStateName(s PicItemState) string { return enumName(stateNames, int(s)) }

// AlphaHandleName 返回透明通道处理方式的英文名称.
func AlphaHandleName(t AlphaHandleType) string { return enumName(alphaHandleNames, int(t)) }

// ScaleName 返回缩放方式的英文名称.
func ScaleName(t ScaleType) string { return enumName(scaleNames, int(t)) }

// PaletteAlgoName 返回 PNG 调色盘算法的英文名称.
func PaletteAlgoName(t PaletteAlgoType) string { return enumName(paletteAlgoNames, int(t)) }

// SizeUnitName 返回体积单位的英文名称.
func SizeUnitName(u SizeUnit) string { return enumName(sizeUnitNames, int(u)) }

// QualityLevelName 返回画质等级的英文名称.
func QualityLevelName(q QualityLevel) string { return enumName(qualityNames, int(q)) }
