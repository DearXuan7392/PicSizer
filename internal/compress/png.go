package compress

import (
	"PicSizer/internal/core"
	"PicSizer/internal/core/setting"
	"PicSizer/internal/fileio/codec"
	"image"
)

// pngCompressor PNG 压缩器
type pngCompressor struct {
	baseCompressor
	outputPath string
}

// NewPNGCompressor 创建 PNG 压缩器
func NewPNGCompressor(img image.Image, outputPath string) Compressor {
	return &pngCompressor{
		baseCompressor: baseCompressor{
			img:        img,
			maxQuality: 4,
		},
		outputPath: outputPath,
	}
}

// CompressByQuality 按质量压缩 PNG
func (c *pngCompressor) CompressByQuality(quality setting.QualityLevel) *core.PicResult {
	encode := func(q int) ([]byte, error) {
		return codec.EncodePNG(c.img, q, setting.GetSetting().PngKeepIndexedAlpha)
	}
	return c.compressByQuality(qualityToInt(quality), encode, c.outputPath)
}

// CompressByFileSize 按文件大小压缩 PNG
func (c *pngCompressor) CompressByFileSize(limitKB int64) *core.PicResult {
	encode := func(q int) ([]byte, error) {
		return codec.EncodePNG(c.img, q, setting.GetSetting().PngKeepIndexedAlpha)
	}
	return c.compressByFileSize(limitKB, encode, c.outputPath)
}

func qualityToInt(q setting.QualityLevel) int {
	switch q {
	case setting.QualityLevelPoor:
		return 1
	case setting.QualityLevelNormal:
		return 2
	case setting.QualityLevelClear:
		return 3
	case setting.QualityLevelBest:
		return 4
	default:
		return -1
	}
}
