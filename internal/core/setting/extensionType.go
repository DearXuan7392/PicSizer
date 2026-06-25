package setting

// 扩展名类型
type ExtensionType int

const (
	ExtJPEG ExtensionType = iota
	ExtPNG
	ExtWebP
	ExtOrigin // 保持原格式
)
