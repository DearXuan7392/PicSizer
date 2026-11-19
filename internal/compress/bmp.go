package compress

import (
	"PicSizer/internal/core"
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/fileio/codec"
	"PicSizer/internal/utils"
	"image"

	strs "PicSizer/internal/core/strings"
)

type bmpCompressor struct {
	baseCompressor
	outputPath string
}

// NewBMPCompressor 创建 BMP 压缩器.
// BMP 不支持透明与压缩, 输入图像含透明通道时先按白色背景合成.
func NewBMPCompressor(img image.Image, outputPath string) Compressor {
	if utils.HasAlphaChannel(img) {
		img = utils.CompositeOnWhite(img, img.Bounds())
	}
	return &bmpCompressor{
		baseCompressor: baseCompressor{
			img:        img,
			maxQuality: 100,
			sizeUpper:  100,
		},
		outputPath: outputPath,
	}
}

// CompressByQuality 按画质等级压缩. BMP 为无损格式, 忽略画质参数直接编码.
func (c *bmpCompressor) CompressByQuality(_ settingLoader.QualityLevel) *core.PicResult {
	encode := func(_ int) ([]byte, error) {
		return codec.EncodeBMP(c.img)
	}
	return c.compressByQuality(100, encode, c.outputPath)
}

// CompressByFileSize 按大小限制压缩. BMP 无法压缩体积, 直接返回错误.
func (c *bmpCompressor) CompressByFileSize(_ int64) *core.PicResult {
	return core.GetError(strs.ErrBMPNoCompression)
}
