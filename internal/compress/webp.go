package compress

import (
	"PicSizer/internal/core/setting"
	"image"

	"PicSizer/internal/core"
	"PicSizer/internal/fileio/codec"
)

// webpCompressor WebP 压缩器
type webpCompressor struct {
	baseCompressor
	outputPath string
}

// NewWebPCompressor 创建 WebP 压缩器
func NewWebPCompressor(img image.Image, outputPath string) Compressor {
	return &webpCompressor{
		baseCompressor: baseCompressor{
			img:        img,
			maxQuality: 100,
		},
		outputPath: outputPath,
	}
}

// CompressByQuality 按质量压缩 WebP
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

// CompressByFileSize 按文件大小压缩 WebP
func (c *webpCompressor) CompressByFileSize(limitKB int64) *core.PicResult {
	encode := func(q int) ([]byte, error) {
		return codec.EncodeWebP(c.img, q)
	}
	return c.compressByFileSize(limitKB, encode, c.outputPath)
}
