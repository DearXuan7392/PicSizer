package codec

import "image"

// ImageCodec 图片编解码器标准接口
// 每种图片格式都需要实现该接口, 提供类型判断、编码和解码能力
type ImageCodec interface {
	// Name 返回编解码器名称 (如 "jpeg", "png", "webp")
	Name() string

	// IsType 判断给定的图片数据是否属于该格式
	IsType(data []byte) bool

	// Decode 解码图片数据为 image.Image
	Decode(data []byte) (image.Image, error)
}
