package codec

import "image"

// ImageCodec 定义图片编解码器接口。
// 每种图片格式需实现该接口以提供类型判断和解码能力。
type ImageCodec interface {
	Name() string
	IsType(data []byte) bool
	Decode(data []byte) (image.Image, error)
}
