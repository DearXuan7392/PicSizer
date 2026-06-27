package codec

import (
	"PicSizer/internal/core/settingLoader"
	"errors"
	"fmt"
	"image"
	"os"
	"sync"

	"PicSizer/internal/core/strings"
)

var (
	registry   []ImageCodec
	registryMu sync.RWMutex
)

// RegisterCodec 注册一个图片编解码器到全局注册表.
// 编解码器按注册顺序进行类型匹配, 先注册的优先匹配.
func RegisterCodec(codec ImageCodec) {
	logger.Debug("register codec: %s", codec.Name())
	registry = append(registry, codec)
}

// GetCodec 根据图片数据的文件头特征获取匹配的编解码器.
// 遍历注册表, 返回第一个能识别该数据格式的编解码器.
func GetCodec(data []byte) ImageCodec {
	for _, codec := range registry {
		if codec.IsType(data) {
			logger.Debug("get codec: %s", codec.Name())
			return codec
		}
	}
	return nil
}

func init() {
	// 按优先级注册: WebP 优先 (避免 RIFF 头被误判), 然后 PNG, 最后 JPEG
	RegisterCodec(&webpCodec{})
	RegisterCodec(&pngCodec{})
	RegisterCodec(&jpegCodec{})
}

func InitSetting(set settingLoader.Setting) {
	for _, codec := range registry {
		logger.Debug("init codec: %s", codec.Name())
		codec.InitSetting(set)
	}
}

// LoadImage 从指定路径加载图片文件, 自动检测格式并解码为 image.Image.
func LoadImage(imgPath string) (image.Image, error) {
	logger.Debug("load image: %s", imgPath)
	data, err := os.ReadFile(imgPath)
	if err != nil {
		return nil, fmt.Errorf(strs.CodecErrReadFileFailed, err.Error())
	}

	codec := GetCodec(data)
	if codec == nil {
		return nil, errors.New(strs.CodecErrUnsupportedCodec)
	}

	img, err := codec.Decode(data)
	if err != nil {
		return nil, fmt.Errorf(strs.CodecErrDecodeFailed, err.Error())
	}

	return img, nil
}
