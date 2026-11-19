package compress

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/utils"
	"image"

	"PicSizer/internal/core"
	"PicSizer/internal/fileio/codec"
)

type jpegCompressor struct {
	baseCompressor
	outputPath string
}

// NewJPEGCompressor 创建 JPEG 压缩器.
// JPEG 不支持透明, 输入图像含透明通道时先按白色背景合成.
func NewJPEGCompressor(img image.Image, outputPath string) Compressor {
	if utils.HasAlphaChannel(img) {
		img = utils.CompositeOnWhite(img, img.Bounds())
	}
	return &jpegCompressor{
		baseCompressor: baseCompressor{
			img:        img,
			maxQuality: 100,
			sizeUpper:  100,
		},
		outputPath: outputPath,
	}
}

// CompressByQuality 按画质等级压缩. 设置了 JPEG 精细化画质 (非 0) 时优先使用.
func (c *jpegCompressor) CompressByQuality(quality settingLoader.QualityLevel) *core.PicResult {
	qualityValue := settingLoader.QualityLevelValue(quality)
	if advanced := settingLoader.GetSetting().AdvancedJpegQuality; advanced != 0 {
		logger.Debug("jpeg quality overridden, level: %s, from: %d, to: %d",
			settingLoader.QualityLevelName(quality), qualityValue, advanced)
		qualityValue = advanced
	}
	encode := func(q int) ([]byte, error) {
		return codec.EncodeJPEG(c.img, q)
	}
	return c.compressByQuality(qualityValue, encode, c.outputPath)
}

func (c *jpegCompressor) CompressByFileSize(limitKB int64) *core.PicResult {
	encode := func(q int) ([]byte, error) {
		return codec.EncodeJPEG(c.img, q)
	}
	return c.compressByFileSize(limitKB, encode, c.outputPath)
}
