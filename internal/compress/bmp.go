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

// NewBMPCompressor 创建一个 BMP 压缩器.
// BMP 格式不支持透明通道和压缩, 若有 Alpha 通道则先白底合成.
func NewBMPCompressor(img image.Image, outputPath string) Compressor {
	if utils.HasAlphaChannel(img) {
		img = utils.CompositeOnWhite(img, img.Bounds())
	}
	return &bmpCompressor{
		baseCompressor: baseCompressor{
			img:        img,
			maxQuality: 100,
		},
		outputPath: outputPath,
	}
}

// CompressByQuality 按指定画质等级压缩 BMP 图像.
// BMP 为无损格式, 忽略画质参数直接编码.
func (c *bmpCompressor) CompressByQuality(_ settingLoader.QualityLevel) *core.PicResult {
	encode := func(_ int) ([]byte, error) {
		return codec.EncodeBMP(c.img)
	}
	// BMP 为无损格式, 直接使用最高质量值编码
	return c.compressByQuality(100, encode, c.outputPath)
}

// CompressByFileSize 按指定文件大小限制压缩 BMP 图像.
// BMP 格式不支持压缩, 直接返回错误.
func (c *bmpCompressor) CompressByFileSize(_ int64) *core.PicResult {
	return core.GetError(strs.ErrBMPNoCompression)
}
