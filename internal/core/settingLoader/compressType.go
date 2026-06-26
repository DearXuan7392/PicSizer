package settingLoader

// CompressType 表示压缩模式的枚举类型。
type CompressType int

const (
	CompressQuality  CompressType = iota // 按质量压缩
	CompressFileSize                     // 按文件大小压缩
)
