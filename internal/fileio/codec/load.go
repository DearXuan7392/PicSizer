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

// RegisterCodec 将编解码器注册到全局注册表, 按注册顺序匹配类型.
func RegisterCodec(codec ImageCodec) {
	logger.Debug("register codec, codec: %s", codec.Name())
	registry = append(registry, codec)
}

// GetCodec 按文件头内容嗅探返回第一个能识别数据的编解码器, 与扩展名无关.
func GetCodec(data []byte) ImageCodec {
	for _, codec := range registry {
		if codec.IsType(data) {
			logger.Debug("codec matched, codec: %s, data size: %d", codec.Name(), len(data))
			return codec
		}
	}
	return nil
}

func init() {
	// 按优先级注册: WebP 优先 (避免 RIFF 头被误判), 然后 PNG, 最后 JPEG 和 BMP
	RegisterCodec(&webpCodec{})
	RegisterCodec(&pngCodec{})
	RegisterCodec(&jpegCodec{})
	RegisterCodec(&bmpCodec{})
}

func InitSetting(set settingLoader.Setting) {
	logger.Debug("init codec settings, png palette: %s, png keep indexed alpha: %t, png dithering: %t",
		settingLoader.PaletteAlgoName(set.AdvancedPngPaletteAlgo), set.AdvancedPngKeepIndexedAlpha, set.AdvancedPngEnableDithering)
	for _, codec := range registry {
		codec.InitSetting(set)
	}
}

func LoadImage(imgPath string) (image.Image, error) {
	logger.Debug("load image, path: %s", imgPath)
	data, err := os.ReadFile(imgPath)
	if err != nil {
		logger.Error("read image file failed, path: %s, err: %v", imgPath, err)
		return nil, fmt.Errorf(strs.CodecErrReadFileFailed, err.Error())
	}
	logger.Debug("read image file, path: %s, size: %d", imgPath, len(data))

	codec := GetCodec(data)
	if codec == nil {
		logger.Error("no codec matched, path: %s, size: %d", imgPath, len(data))
		return nil, errors.New(strs.CodecErrUnsupportedCodec)
	}

	img, err := codec.Decode(data)
	if err != nil {
		logger.Error("decode image failed, codec: %s, path: %s, err: %v", codec.Name(), imgPath, err)
		return nil, fmt.Errorf(strs.CodecErrDecodeFailed, err.Error())
	}

	logger.Debug("decode image done, codec: %s, type: %T, bounds: %v", codec.Name(), img, img.Bounds())
	return img, nil
}
