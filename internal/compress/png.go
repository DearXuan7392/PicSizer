package compress

import (
	"PicSizer/internal/core"
	"PicSizer/internal/core/setting"
	"PicSizer/internal/fileio/codec"
	"image"
)

type pngCompressor struct {
	baseCompressor
	outputPath string
}

// NewPNGCompressor 创建一个 PNG 压缩器.
func NewPNGCompressor(img image.Image, outputPath string) Compressor {
	return &pngCompressor{
		baseCompressor: baseCompressor{
			img:        img,
			maxQuality: 4,
		},
		outputPath: outputPath,
	}
}

// CompressByQuality 按指定的画质等级压缩 PNG 图像.
// PNG 的 quality 参数映射到 1-4, 分别对应不同的量化等级.
func (c *pngCompressor) CompressByQuality(quality setting.QualityLevel) *core.PicResult {
	encode := func(q int) ([]byte, error) {
		return codec.EncodePNG(c.img, q)
	}
	return c.compressByQuality(qualityToInt(quality), encode, c.outputPath)
}

// CompressByFileSize 按指定的文件大小限制压缩 PNG 图像.
func (c *pngCompressor) CompressByFileSize(limitKB int64) *core.PicResult {
	encode := func(q int) ([]byte, error) {
		return codec.EncodePNG(c.img, q)
	}
	return c.compressByFileSize(limitKB, encode, c.outputPath)
}

// qualityToInt 将 QualityLevel 枚举转换为 PNG 编码器使用的 1-4 等级.
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
