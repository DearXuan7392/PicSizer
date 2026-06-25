package strs

// 错误信息
var (
	ErrCompressError        = "压缩过程中发生错误"
	ErrOutOfLimit           = "无法压缩到指定大小"
	ErrNotImplemented       = "未实现的功能"
	ErrArgOutOfRange        = "参数超出范围"
	ErrFileNotFound         = "文件不存在"
	ErrBrokenFile           = "文件损坏"
	ErrNoPictures           = "没有待压缩的图片"
	ErrOutputDirInvalid     = "输出目录有误"
	ErrUnsupportedExt       = "不支持的格式"
	ErrNoCommonPrefix       = "无法找到共同前缀, 请重新选择图片, 或将图片分多次压缩\n(常见原因: 图片分布在多个不同的盘符下)"
	ErrScaleLockSideInvalid = "缩放方式为 \"等比锁定单边\" 时, 宽和高必须有一项为 0, 另一项不为 0"
)
