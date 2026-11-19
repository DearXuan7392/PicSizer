package codec

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/utils"
	"bytes"
	"image"

	"github.com/gen2brain/webp"
)

type webpCodec struct{}

func (c *webpCodec) InitSetting(_ settingLoader.Setting) {}

func (c *webpCodec) Name() string { return "webp" }

// IsType 通过 RIFF....WEBP 文件头识别.
func (c *webpCodec) IsType(data []byte) bool {
	if len(data) < 12 {
		return false
	}
	return bytes.Equal(data[0:4], []byte("RIFF")) && bytes.Equal(data[8:12], []byte("WEBP"))
}

// Decode 解码后统一转换为 RGBA/NRGBA: 后续管道 (预处理/白底合成/类型判断) 只处理这两种类型.
func (c *webpCodec) Decode(data []byte) (image.Image, error) {
	logger.Debug("decode data, format: webp, size: %d", len(data))
	img, err := webp.Decode(bytes.NewReader(data))
	if err != nil {
		logger.Error("decode webp failed, size: %d, err: %v", len(data), err)
		return nil, err
	}

	switch img.(type) {
	case *image.YCbCr:
		logger.Debug("convert image, format: webp, target: RGBA")
		return utils.ConvertToRGBA(img), nil
	case *image.NYCbCrA, *image.NRGBA:
		logger.Debug("convert image, format: webp, target: NRGBA")
		return utils.ConvertToNRGBA(img), nil
	default:
		logger.Warn("convert image, format: webp, unknown source type: %T, target: RGBA", img)
		return utils.ConvertToRGBA(img), nil
	}
}

// EncodeWebP 编码为 WebP, quality 0-100 (0 为最低可用画质), >= 100 时启用无损模式.
func EncodeWebP(img image.Image, quality int) ([]byte, error) {
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
