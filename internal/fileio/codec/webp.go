package codec

import (
	"bytes"
	"image"

	"github.com/gen2brain/webp"
)

// webpCodec WebP 格式编解码器
type webpCodec struct{}

// Name 返回编解码器名称
func (c *webpCodec) Name() string { return "webp" }

// IsType 判断数据是否为 WebP 格式 (通过 RIFF...WEBP 文件头识别)
func (c *webpCodec) IsType(data []byte) bool {
	if len(data) < 12 {
		return false
	}
	return bytes.Equal(data[0:4], []byte("RIFF")) && bytes.Equal(data[8:12], []byte("WEBP"))
}

// Decode 解码 WebP 图像数据
func (c *webpCodec) Decode(data []byte) (image.Image, error) {
	return webp.Decode(bytes.NewReader(data))
}

// EncodeWebP 编码为 WebP 格式
func EncodeWebP(img image.Image, quality int) ([]byte, error) {
	var buf bytes.Buffer
	// quality 0-100 映射到 webp 质量 0-100
	err := webp.Encode(&buf, img, webp.Options{
		Lossless: quality >= 100,
		Quality:  quality,
	})
	if err != nil {
		return nil, err
	}
	return buf.Bytes(), nil
}
