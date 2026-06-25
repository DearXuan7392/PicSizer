package codec

import (
	"bytes"
	"image"
	"image/jpeg"
)

// jpegCodec JPEG 格式编解码器
type jpegCodec struct{}

// Name 返回编解码器名称
func (c *jpegCodec) Name() string { return "jpeg" }

// IsType 判断数据是否为 JPEG 格式 (通过 SOI 标记 FF D8 FF 识别)
func (c *jpegCodec) IsType(data []byte) bool {
	return len(data) >= 3 && data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF
}

// Decode 解码 JPEG 图像数据
func (c *jpegCodec) Decode(data []byte) (image.Image, error) {
	// 一定是 YCbCr 格式
	return jpeg.Decode(bytes.NewReader(data))
}

// EncodeJPEG 编码为 JPEG 格式
func EncodeJPEG(img image.Image, quality int) ([]byte, error) {
	var buf bytes.Buffer
	err := jpeg.Encode(&buf, img, &jpeg.Options{Quality: quality})
	if err != nil {
		return nil, err
	}
	return buf.Bytes(), nil
}
