package codec

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/log"
	"image"
)

var (
	logger = log.NewLogger("codec")
)

// ImageCodec 定义图片编解码器接口.
// 每种图片格式需实现该接口以提供类型判断和解码能力.
type ImageCodec interface {
	Name() string
	IsType(data []byte) bool
	InitSetting(set settingLoader.Setting)
	Decode(data []byte) (image.Image, error)
}
