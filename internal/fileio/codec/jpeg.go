package codec

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/utils"
	"bytes"
	"image"
	"image/jpeg"
)

type jpegCodec struct{}

func (c *jpegCodec) InitSetting(_ settingLoader.Setting) {}

func (c *jpegCodec) Name() string { return "jpeg" }

// IsType 通过 JPEG 的 SOI 标记 (FF D8 FF) 识别.
func (c *jpegCodec) IsType(data []byte) bool {
	return len(data) >= 3 && data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF
}

func (c *jpegCodec) Decode(data []byte) (image.Image, error) {
	logger.Debug("decode data, format: jpeg, size: %d", len(data))
	img, err := jpeg.Decode(bytes.NewReader(data))
	if err != nil {
		logger.Error("decode jpeg failed, size: %d, err: %v", len(data), err)
		return nil, err
	}

	logger.Debug("convert image, format: jpeg, target: RGBA")
	return utils.ConvertToRGBA(img), nil

}

// EncodeJPEG 编码为 JPEG, quality 1-100, 值越高画质越好.
func EncodeJPEG(img image.Image, quality int) ([]byte, error) {
	var buf bytes.Buffer
	err := jpeg.Encode(&buf, img, &jpeg.Options{Quality: quality})
	if err != nil {
		return nil, err
	}
	return buf.Bytes(), nil
}
