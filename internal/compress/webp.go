package compress

import (
	"PicSizer/internal/core/setting"
	"image"

	"PicSizer/internal/core"
	"PicSizer/internal/fileio/codec"
)

type webpCompressor struct {
	baseCompressor
	outputPath string
}

// NewWebPCompressor 创建一个 WebP 压缩器.
func NewWebPCompressor(img image.Image, outputPath string) Compressor {
	return &webpCompressor{
		baseCompressor: baseCompressor{
			img:        img,
			maxQuality: 100,
		},
		outputPath: outputPath,
	}
}

// CompressByQuality 按指定的画质等级压缩 WebP 图像.
// 若配置中设置了 WebP 精细化画质（WebPQuality 非0）, 则优先使用该精细画质值.
func (c *webpCompressor) CompressByQuality(quality setting.QualityLevel) *core.PicResult {
	qualityValue := setting.QualityLevelValue(quality)
	if setting.GetSetting().JpegQuality != 0 {
		qualityValue = setting.GetSetting().JpegQuality
	}
	encode := func(q int) ([]byte, error) {
		return codec.EncodeWebP(c.img, q)
	}
	return c.compressByQuality(qualityValue, encode, c.outputPath)
}

// CompressByFileSize 按指定的文件大小限制压缩 WebP 图像.
func (c *webpCompressor) CompressByFileSize(limitKB int64) *core.PicResult {
	encode := func(q int) ([]byte, error) {
		return codec.EncodeWebP(c.img, q)
	}
	return c.compressByFileSize(limitKB, encode, c.outputPath)
}
