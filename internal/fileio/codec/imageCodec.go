package codec

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/log"
	"image"
)

var (
	logger = log.NewLogger("codec")
)

// ImageCodec 各图片格式的编解码器接口, 实现类通过 init() 注册到 load.go 的注册表.
type ImageCodec interface {
	Name() string
	IsType(data []byte) bool
	InitSetting(set settingLoader.Setting)
	Decode(data []byte) (image.Image, error)
}
