package codec

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/utils"
	"bytes"
	"image"

	"github.com/gen2brain/webp"
)

type webpCodec struct{}

var (
	advancedWebpQuality = 0
)

func (c *webpCodec) InitSetting(set settingLoader.Setting) {
	advancedWebpQuality = set.AdvancedWebPQuality
}

// Name 返回编解码器名称.
func (c *webpCodec) Name() string { return "webp" }

// IsType 通过 RIFF...WEBP 文件头判断数据是否为 WebP 格式.
func (c *webpCodec) IsType(data []byte) bool {
	if len(data) < 12 {
		return false
	}
	return bytes.Equal(data[0:4], []byte("RIFF")) && bytes.Equal(data[8:12], []byte("WEBP"))
}

// Decode 将 WebP 数据解码为 image.Image.
func (c *webpCodec) Decode(data []byte) (image.Image, error) {
	logger.Debug("decode webp data")
	img, err := webp.Decode(bytes.NewReader(data))
	if err != nil {
		return nil, err
	}

	switch img.(type) {
	case *image.YCbCr:
		logger.Debug("convert webp to RGBA image")
		return utils.ConvertToRGBA(img), nil
	case *image.NYCbCrA, *image.NRGBA:
		logger.Debug("convert webp to NRGBA image")
		return utils.ConvertToNRGBA(img), nil
	default:
		logger.Warn("unknown image type, convert to RGBA image")
		return utils.ConvertToRGBA(img), nil
	}
}

// EncodeWebP 将 image.Image 编码为 WebP 格式的字节数据.
// quality 参数范围 0-100；quality >= 100 时启用无损模式.
func EncodeWebP(img image.Image, quality int) ([]byte, error) {
	if advancedWebpQuality != 0 {
		quality = advancedWebpQuality
	}
	var buf bytes.Buffer
	err := webp.Encode(&buf, img, webp.Options{
		Lossless: quality >= 100,
		Quality:  quality,
	})
	if err != nil {
		return nil, err
	}
	return buf.Bytes(), nil
}
