package codec

import (
	"bytes"
	"encoding/binary"
	"errors"
	"image"
	"image/color"
	"image/draw"
	"image/png"

	"github.com/ericpauley/go-quantize/quantize"
)

// pngCodec PNG 格式编解码器
type pngCodec struct{}

// Name 返回编解码器名称
func (c *pngCodec) Name() string { return "png" }

// IsType 判断数据是否为 PNG 格式 (通过 8 字节文件签名识别)
func (c *pngCodec) IsType(data []byte) bool {
	pngSignature := []byte{0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A}
	return len(data) >= 8 && bytes.Equal(data[:8], pngSignature)
}

// Decode 解码 PNG 图像数据
// 解码后会对图像类型进行标准化处理:
//   - 64 位图像 (RGBA64/NRGBA64) 强制降为 32 位 (RGBA/NRGBA)
//   - 调色板图像: 含透明通道则转为 NRGBA, 否则转为 RGBA
//   - Gray 格式 (Gray/Gray16) 转为 RGBA
//   - 其他格式保持不变
func (c *pngCodec) Decode(data []byte) (image.Image, error) {
	img, err := png.Decode(bytes.NewReader(data))
	if err != nil {
		return nil, err
	}
	hasAlphaChannel := c.HasAlphaChannel(data)
	return normalizePNGImage(img, hasAlphaChannel), nil
}

// normalizePNGImage 将 PNG 解码后的图像标准化为 32 位图像
//   - 64 位 RGBA64 -> 32 位 RGBA (预乘 alpha)
//   - 64 位 NRGBA64 -> 32 位 NRGBA (非预乘 alpha)
//   - 调色板图像: 含透明通道 -> NRGBA, 不含透明通道 -> RGBA
//   - Gray / Gray16 灰度图像 -> RGBA
//   - 其他类型 (如 NRGBA, RGBA) 保持原样返回
func normalizePNGImage(src image.Image, hasAlphaChannel bool) image.Image {
	if src == nil {
		return nil
	}

	bounds := src.Bounds()

	// 有 A 通道, 强制转为 NRGBA
	if hasAlphaChannel {
		if _, ok := src.(*image.NRGBA); ok {
			return src
		}

		dst := image.NewNRGBA(bounds)
		draw.Draw(dst, bounds, src, bounds.Min, draw.Src)
		return dst
	}

	// 没有 A 通道, 强制转为 RGBA
	if _, ok := src.(*image.RGBA); ok {
		return src
	}

	dst := image.NewRGBA(bounds)
	draw.Draw(dst, bounds, src, bounds.Min, draw.Src)
	return dst
}

const (
	QualityLossless    = 4 // 等级 4：Go 原生纯无损压缩 (画质最高)
	QualityQuantize256 = 3 // 等级 3：go-quantize 中位切分 256 色
	QualityQuantize128 = 2 // 等级 2：go-quantize 中位切分 128 色
	QualityQuantize64  = 1 // 等级 1：go-quantize 中位切分 64 色 (体积最小)
)

func EncodePNG(img image.Image, qualityLevel int, keepIndexedAlpha bool) ([]byte, error) {
	var buf bytes.Buffer
	bounds := img.Bounds()

	// 初始化默认的 PNG 编码器
	// 无损和有损量化均采用 BestCompression 极限压缩，以在对应等级下获得最小体积
	encoder := &png.Encoder{
		CompressionLevel: png.BestCompression,
	}

	// ------------------ 等级 4：Go 原生纯无损压缩 ------------------
	if qualityLevel == QualityLossless {
		err := encoder.Encode(&buf, img)
		if err != nil {
			return nil, err
		}
		return buf.Bytes(), nil
	}

	// ------------------ 等级 1-3：go-quantize 有损量化压缩 ------------------
	var maxColors int
	switch qualityLevel {
	case QualityQuantize256:
		maxColors = 256
	case QualityQuantize128:
		maxColors = 128
	case QualityQuantize64:
		maxColors = 64
	default:
		return nil, errors.New("invalid quality level: must be 1, 2, 3, or 4")
	}

	// 1. 初始化 go-quantize 均值聚合的中位切分算法
	quantizer := quantize.MedianCutQuantizer{
		Aggregation:    quantize.Mean,    // 使用均值聚合, 获得更平滑的颜色过渡
		AddTransparent: keepIndexedAlpha, // 自动保护和预留纯透明通道，防止透明背景变色或产生毛边
	}

	// 2. 零内存分配量化图像，生成指定颜色数量的调色板
	p := make([]color.Color, 0, maxColors)
	palette := quantizer.Quantize(p, img)

	// 3. 创建对应的索引色图像 (Paletted Image)
	palettedImg := image.NewPaletted(bounds, palette)

	// 4. 将原图像素映射到调色板
	// 修正：显式使用 FloydSteinberg 误差扩散抖动算法，极大缓解 128色/64色 下的色带断层现象
	draw.FloydSteinberg.Draw(palettedImg, bounds, img, image.Point{})

	// 5. 使用 PNG 编码器输出体积优化后的数据
	err := encoder.Encode(&buf, palettedImg)
	if err != nil {
		return nil, err
	}

	return buf.Bytes(), nil
}

// HasAlphaChannel 100% 精准判定 PNG 原图在硬盘上是否物理携带 A 通道
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

func hasTRNSChunk(data []byte) bool {
	// PNG 签名: 137 80 78 71 13 10 26 10
	if len(data) < 8 || !bytes.Equal(data[:8], []byte{137, 80, 78, 71, 13, 10, 26, 10}) {
		return false
	}

	offset := 8
	for offset < len(data) {
		if offset+8 > len(data) {
			break
		}

		// 读取块长度 (4 bytes)
		length := binary.BigEndian.Uint32(data[offset : offset+4])
		offset += 4

		// 读取块类型 (4 bytes)
		chunkType := string(data[offset : offset+4])
		offset += 4

		if chunkType == "tRNS" {
			return true
		}

		// 跳过块数据和 CRC (4 bytes)
		offset += int(length) + 4

		if chunkType == "IEND" {
			break
		}
	}
	return false
}
