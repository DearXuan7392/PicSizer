package codec

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/utils"
	"bytes"
	"image"

	"golang.org/x/image/bmp"
)

type bmpCodec struct{}

func (c *bmpCodec) InitSetting(_ settingLoader.Setting) {}

func (c *bmpCodec) Name() string { return "bmp" }

// IsType 通过 BMP 的 BM 文件头识别.
func (c *bmpCodec) IsType(data []byte) bool {
	return len(data) >= 2 && data[0] == 0x42 && data[1] == 0x4D
}

func (c *bmpCodec) Decode(data []byte) (image.Image, error) {
	logger.Debug("decode data, format: bmp, size: %d", len(data))
	img, err := bmp.Decode(bytes.NewReader(data))
	if err != nil {
		logger.Error("decode bmp failed, size: %d, err: %v", len(data), err)
		return nil, err
	}

	logger.Debug("convert image, format: bmp, target: RGBA")
	return utils.ConvertToRGBA(img), nil
}

func EncodeBMP(img image.Image) ([]byte, error) {
	var buf bytes.Buffer
	err := bmp.Encode(&buf, img)
	if err != nil {
		return nil, err
	}
	return buf.Bytes(), nil
}
