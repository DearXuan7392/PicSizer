package compress

import (
	"PicSizer/internal/core/settingLoader"
	"image"

	"PicSizer/internal/core"
	"PicSizer/internal/fileio/codec"
)

type webpCompressor struct {
	baseCompressor
	outputPath string
}

func NewWebPCompressor(img image.Image, outputPath string) Compressor {
	return &webpCompressor{
		baseCompressor: baseCompressor{
			img:        img,
			maxQuality: 100,
			sizeUpper:  100,
		},
		outputPath: outputPath,
	}
}

// CompressByQuality 按画质等级压缩. 设置了 WebP 精细化画质 (非 0) 时优先使用.
func (c *webpCompressor) CompressByQuality(quality settingLoader.QualityLevel) *core.PicResult {
	qualityValue := settingLoader.QualityLevelValue(quality)
	if advanced := settingLoader.GetSetting().AdvancedWebPQuality; advanced != 0 {
		logger.Debug("webp quality overridden, level: %s, from: %d, to: %d",
			settingLoader.QualityLevelName(quality), qualityValue, advanced)
		qualityValue = advanced
	}
	encode := func(q int) ([]byte, error) {
		return codec.EncodeWebP(c.img, q)
	}
	return c.compressByQuality(qualityValue, encode, c.outputPath)
}

func (c *webpCompressor) CompressByFileSize(limitKB int64) *core.PicResult {
	encode := func(q int) ([]byte, error) {
		return codec.EncodeWebP(c.img, q)
	}
	return c.compressByFileSize(limitKB, encode, c.outputPath)
}
