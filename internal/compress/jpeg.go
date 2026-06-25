package compress

import (
	"PicSizer/internal/core/setting"
	"PicSizer/internal/utils"
	"image"

	"PicSizer/internal/core"
	"PicSizer/internal/fileio/codec"
)

type jpegCompressor struct {
	baseCompressor
	outputPath string
}

// NewJPEGCompressor 创建一个 JPEG 压缩器.
// 若输入图像包含透明通道, 会先按白色背景合成后再进行压缩.
func NewJPEGCompressor(img image.Image, outputPath string) Compressor {
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

// CompressByQuality 按指定的画质等级压缩 JPEG 图像.
// 若配置中设置了 JPEG 精细化画质（JpegQuality 非0）, 则优先使用该精细画质值.
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

// CompressByFileSize 按指定的文件大小限制压缩 JPEG 图像.
func (c *jpegCompressor) CompressByFileSize(limitKB int64) *core.PicResult {
	encode := func(q int) ([]byte, error) {
		return codec.EncodeJPEG(c.img, q)
	}
	return c.compressByFileSize(limitKB, encode, c.outputPath)
}
