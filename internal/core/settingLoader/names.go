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

func CompressTypeName(t CompressType) string { return enumName(compressTypeNames, int(t)) }

func OutputTypeName(t OutputType) string { return enumName(outputTypeNames, int(t)) }

func ExtensionName(t ExtensionType) string { return enumName(extensionNames, int(t)) }

func PicItemStateName(s PicItemState) string { return enumName(stateNames, int(s)) }

func AlphaHandleName(t AlphaHandleType) string { return enumName(alphaHandleNames, int(t)) }

func ScaleName(t ScaleType) string { return enumName(scaleNames, int(t)) }

func PaletteAlgoName(t PaletteAlgoType) string { return enumName(paletteAlgoNames, int(t)) }

func SizeUnitName(u SizeUnit) string { return enumName(sizeUnitNames, int(u)) }

func QualityLevelName(q QualityLevel) string { return enumName(qualityNames, int(q)) }
