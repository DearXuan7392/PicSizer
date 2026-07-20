package settingLoader

// ExtensionType 表示输出图片格式的枚举类型。
type ExtensionType int

const (
	ExtJPEG ExtensionType = iota
	ExtPNG
	ExtWebP
	ExtBMP
	ExtOrigin
)
