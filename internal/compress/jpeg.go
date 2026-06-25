package compress

import (
	"PicSizer/internal/core/setting"
	"PicSizer/internal/utils"
	"image"

	"PicSizer/internal/core"
	"PicSizer/internal/fileio/codec"
)

// jpegCompressor JPEG 压缩器
type jpegCompressor struct {
	baseCompressor
	outputPath string
}

// NewJPEGCompressor 创建 JPEG 压缩器
func NewJPEGCompressor(img image.Image, outputPath string) Compressor {
	// 如果有透明通道, 需要先转为非透明
	if utils.HasAlphaChannel(img) {
		img = utils.CompositeOnWhite(img, img.Bounds())
	}
	return &jpegCompressor{
		baseCompressor: baseCompressor{
			img:        img,
			maxQuality: 100,
		},
		outputPath: outputPath,
	}
}

// CompressByQuality 按质量压缩 JPEG
func (c *jpegCompressor) CompressByQuality(quality setting.QualityLevel) *core.PicResult {
	qualityValue := setting.QualityLevelValue(quality)
	if setting.GetSetting().JpegQuality != 0 {
		qualityValue = setting.GetSetting().JpegQuality
	}
	encode := func(q int) ([]byte, error) {
		return codec.EncodeJPEG(c.img, q)
	}
	return c.compressByQuality(qualityValue, encode, c.outputPath)
}

// CompressByFileSize 按文件大小压缩 JPEG
func (c *jpegCompressor) CompressByFileSize(limitKB int64) *core.PicResult {
	encode := func(q int) ([]byte, error) {
		return codec.EncodeJPEG(c.img, q)
	}
	return c.compressByFileSize(limitKB, encode, c.outputPath)
}
