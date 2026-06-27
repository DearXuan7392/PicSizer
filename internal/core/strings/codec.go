package strs

// 编解码器相关字符串
var (
	CodecErrUnsupportedCodec  = "不支持的图片编码"
	CodecErrInvalidPNGQuality = "不支持的 PNG 压缩等级, 必须为: 1, 2, 3, 或 4"
	CodecErrTypeNotSupport    = "不支持的编码类型"
	CodecErrQualityRange      = "压缩等级超出范围: %d"
	CodecErrReadFileFailed    = "读取文件失败: %s"
	CodecErrDecodeFailed      = "解码图片失败: %s"
)
