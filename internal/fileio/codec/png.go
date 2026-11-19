package codec

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/utils"
	"bytes"
	"encoding/binary"
	"errors"
	"image"
	"image/color"
	"image/draw"
	"image/png"

	strs "PicSizer/internal/core/strings"

	"github.com/ericpauley/go-quantize/quantize"
	"github.com/soniakeys/quant/mean"
)

type pngCodec struct{}

var (
	advancedPngPaletteAlgo      = settingLoader.PaletteMedianCut
	advancedPngKeepIndexedAlpha = true
	advancedPngEnableDithering  = true
)

func (c *pngCodec) InitSetting(set settingLoader.Setting) {
	advancedPngPaletteAlgo = set.AdvancedPngPaletteAlgo
	advancedPngKeepIndexedAlpha = set.AdvancedPngKeepIndexedAlpha
	advancedPngEnableDithering = set.AdvancedPngEnableDithering
}

func (c *pngCodec) Name() string { return "png" }

// IsType 通过 PNG 的 8 字节文件头签名识别.
func (c *pngCodec) IsType(data []byte) bool {
	pngSignature := []byte{0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A}
	return len(data) >= 8 && bytes.Equal(data[:8], pngSignature)
}

// Decode 解码后标准化为 8 位 RGBA 或 NRGBA (依据是否有透明通道),
// 16 位 / 灰度 / 索引等类型会被展开, 简化后续处理.
func (c *pngCodec) Decode(data []byte) (image.Image, error) {
	logger.Debug("decode data, format: png, size: %d", len(data))
	img, err := png.Decode(bytes.NewReader(data))
	if err != nil {
		logger.Error("decode png failed, size: %d, err: %v", len(data), err)
		return nil, err
	}

	if c.HasAlphaChannel(data) {
		logger.Debug("convert image, format: png, target: NRGBA, alpha channel: true")
		img = utils.ConvertToNRGBA(img)
	} else {
		logger.Debug("convert image, format: png, target: RGBA, alpha channel: false")
		img = utils.ConvertToRGBA(img)
	}
	return img, nil
}

const (
	QualityLossless    = 4 // 等级 4: Go 原生纯无损压缩 (画质最高)
	QualityQuantize256 = 3 // 等级 3: go-quantize 中位切分 256 色
	QualityQuantize128 = 2 // 等级 2: go-quantize 中位切分 128 色
	QualityQuantize64  = 1 // 等级 1: go-quantize 中位切分 64 色 (体积最小)
)

// EncodePNG 将图像编码为 PNG, 等级 1-3 为调色板量化压缩, 等级 4 为无损编码.
func EncodePNG(img image.Image, qualityLevel int) ([]byte, error) {
	logger.Debug("encode png, quality level: %d, palette: %s, keep indexed alpha: %t, dithering: %t",
		qualityLevel, settingLoader.PaletteAlgoName(advancedPngPaletteAlgo), advancedPngKeepIndexedAlpha, advancedPngEnableDithering)

	var buf bytes.Buffer
	bounds := img.Bounds()

	encoder := &png.Encoder{
		CompressionLevel: png.BestCompression,
	}

	// 等级 4: Go 标准库无损压缩
	if qualityLevel == QualityLossless {
		err := encoder.Encode(&buf, img)
		if err != nil {
			return nil, err
		}
		return buf.Bytes(), nil
	}

	// 等级 1-3: 调色板量化压缩
	var maxColors int
	switch qualityLevel {
	case QualityQuantize256:
		maxColors = 256
	case QualityQuantize128:
		maxColors = 128
	case QualityQuantize64:
		maxColors = 64
	default:
		return nil, errors.New(strs.CodecErrInvalidPNGQuality)
	}

	var palette color.Palette

	switch advancedPngPaletteAlgo {
	case settingLoader.PaletteMedianCut:
		quantizer := quantize.MedianCutQuantizer{
			Aggregation:    quantize.Mean,
			AddTransparent: advancedPngKeepIndexedAlpha,
		}
		p := make([]color.Color, 0, maxColors)
		palette = quantizer.Quantize(p, img)
	case settingLoader.PaletteKMeans:
		// kmeans 库无透明色预留能力, 手动从目标色数中扣除并附加透明色
		targetColors := maxColors
		if advancedPngKeepIndexedAlpha {
			targetColors--
			if targetColors <= 0 {
				targetColors = 1
			}
		}

		q := mean.Quantizer(targetColors)
		palette = q.Quantize(make(color.Palette, 0, maxColors), img)

		if advancedPngKeepIndexedAlpha {
			palette = append(palette, color.RGBA{0, 0, 0, 0})
		}
	default:
		// 永远不可能达到这里
		return nil, errors.New(strs.CodecErrInvalidPNGQuality)
	}

	palettedImg := image.NewPaletted(bounds, palette)

	if advancedPngEnableDithering {
		draw.FloydSteinberg.Draw(palettedImg, bounds, img, image.Point{})
	} else {
		draw.Draw(palettedImg, bounds, img, image.Point{}, draw.Src)
	}

	err := encoder.Encode(&buf, palettedImg)
	if err != nil {
		return nil, err
	}

	return buf.Bytes(), nil
}

// HasAlphaChannel 解析 PNG 文件头判断原图是否物理携带 Alpha 通道.
// 调色板图像无通道信息, 以 tRNS 块判断是否含透明.
func (c *pngCodec) HasAlphaChannel(data []byte) bool {
	conf, err := png.DecodeConfig(bytes.NewReader(data))
	if err != nil {
		return false
	}

	switch conf.ColorModel {
	case color.NRGBAModel, color.NRGBA64Model:
		return true
	case color.RGBAModel, color.RGBA64Model, color.GrayModel, color.Gray16Model:
		return false
	default:
		return hasTRNSChunk(data)
	}
}

// hasTRNSChunk 遍历 PNG 数据块查找 tRNS 块.
func hasTRNSChunk(data []byte) bool {
	if len(data) < 8 || !bytes.Equal(data[:8], []byte{137, 80, 78, 71, 13, 10, 26, 10}) {
		return false
	}

	offset := 8
	for offset < len(data) {
		if offset+8 > len(data) {
			break
		}

		length := binary.BigEndian.Uint32(data[offset : offset+4])
		offset += 4

		chunkType := string(data[offset : offset+4])
		offset += 4

		if chunkType == "tRNS" {
			return true
		}

		offset += int(length) + 4

		if chunkType == "IEND" {
			break
		}
	}
	return false
}
