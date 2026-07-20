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

// Name 返回编解码器名称.
func (c *bmpCodec) Name() string { return "bmp" }

// IsType 通过 BMP 文件头 (BM) 判断数据是否为 BMP 格式.
func (c *bmpCodec) IsType(data []byte) bool {
	return len(data) >= 2 && data[0] == 0x42 && data[1] == 0x4D
}

// Decode 将 BMP 数据解码为 image.Image.
func (c *bmpCodec) Decode(data []byte) (image.Image, error) {
	logger.Debug("decode bmp data")
	img, err := bmp.Decode(bytes.NewReader(data))
	if err != nil {
		return nil, err
	}

	logger.Debug("convert bmp to RGBA image")
	return utils.ConvertToRGBA(img), nil
}

// EncodeBMP 将 image.Image 编码为 BMP 格式的字节数据.
func EncodeBMP(img image.Image) ([]byte, error) {
	var buf bytes.Buffer
	err := bmp.Encode(&buf, img)
	if err != nil {
		return nil, err
	}
	return buf.Bytes(), nil
}
