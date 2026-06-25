package setting

// 压缩类型
type CompressType int

const (
	CompressQuality  CompressType = iota // 按质量压缩
	CompressFileSize                     // 按文件大小压缩
)
