package codec

import (
	"PicSizer/internal/core/setting"
	"bytes"
	"image"
	"image/jpeg"
)

type jpegCodec struct{}

var (
	advancedJpegQuality = 0
)

func (c *jpegCodec) InitSetting(set setting.Setting) {
	advancedJpegQuality = set.AdvancedJpegQuality
}

// Name 返回编解码器名称.
func (c *jpegCodec) Name() string { return "jpeg" }

// IsType 通过 JPEG 文件头的 SOI 标记（FF D8 FF）判断数据是否为 JPEG 格式.
func (c *jpegCodec) IsType(data []byte) bool {
	return len(data) >= 3 && data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF
}

// Decode 将 JPEG 数据解码为 image.Image.
func (c *jpegCodec) Decode(data []byte) (image.Image, error) {
	return jpeg.Decode(bytes.NewReader(data))
}

// EncodeJPEG 将 image.Image 编码为 JPEG 格式的字节数据.
// quality 参数范围 1-100, 值越高画质越好.
func EncodeJPEG(img image.Image, quality int) ([]byte, error) {
	if advancedJpegQuality != 0 {
		quality = advancedJpegQuality
	}
	var buf bytes.Buffer
	err := jpeg.Encode(&buf, img, &jpeg.Options{Quality: quality})
	if err != nil {
		return nil, err
	}
	return buf.Bytes(), nil
}
