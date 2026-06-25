package preprocess

import (
	"PicSizer/internal/core/setting"
	"image"
	"math"

	xdraw "golang.org/x/image/draw"
)

// ScaleProcessor 缩放预处理器.
// 根据 mode 选择不同的缩放策略, 详细语义见 core.ScaleType 各常量说明.
type ScaleProcessor struct {
	mode   setting.ScaleType
	width  int
	height int
}

// newScaleProcessor 创建缩放预处理器.
// 工厂方法统一返回非 nil 实例, 是否启用由 Enabled() 内部判断.
func newScaleProcessor(mode setting.ScaleType, width, height int) *ScaleProcessor {
	return &ScaleProcessor{mode: mode, width: width, height: height}
}

// Name 返回预处理器名称.
func (s *ScaleProcessor) Name() string {
	return "ScaleProcessor"
}

// Enabled 判断是否需要执行缩放处理.
// 仅当 mode != ScaleNone 时启用, 其他模式均会进入处理流程.
func (s *ScaleProcessor) Enabled() bool {
	return s.mode != setting.ScaleNone
}

// Process 执行缩放处理.
// 内部按 mode 分发到具体算法; 任何无效输入均兜底返回原图, 不影响主流程.
func (s *ScaleProcessor) Process(img image.Image) image.Image {
	if img == nil {
		return img
	}
	bounds := img.Bounds()
	srcW, srcH := bounds.Dx(), bounds.Dy()
	if srcW <= 0 || srcH <= 0 {
		return img
	}

	switch s.mode {
	case setting.ScaleNone:
		return img
	case setting.ScaleStretch:
		return s.stretch(srcW, srcH, img)
	case setting.ScaleFitOutside:
		return s.fitOutside(srcW, srcH, img)
	case setting.ScaleFitInside:
		return s.fitInside(srcW, srcH, img)
	case setting.ScaleFitOutsideCrop:
		return s.fitOutsideCrop(srcW, srcH, img)
	case setting.ScaleLockSide:
		return s.lockSide(srcW, srcH, img)
	default:
		return img
	}
}

// stretch 强制拉伸: 无视宽高比, 直接缩放到 (s.width, s.height).
func (s *ScaleProcessor) stretch(srcW, srcH int, img image.Image) image.Image {
	dstW, dstH := s.validSize(srcW, srcH)
	if dstW == srcW && dstH == srcH {
		return img
	}
	return resize(img, dstW, dstH)
}

// fitOutside 等比外接 (cover): 缩放后宽和高均 >= 目标, 即"填满目标框".
// 算法: ratio = max(dstW/srcW, dstH/srcH).
func (s *ScaleProcessor) fitOutside(srcW, srcH int, img image.Image) image.Image {
	dstW, dstH := s.validSize(srcW, srcH)
	ratio := math.Max(float64(dstW)/float64(srcW), float64(dstH)/float64(srcH))
	newW := int(math.Round(float64(srcW) * ratio))
	newH := int(math.Round(float64(srcH) * ratio))
	if newW == srcW && newH == srcH {
		return img
	}
	return resize(img, newW, newH)
}

// fitInside 等比内接 (contain): 缩放后宽和高均 <= 目标, 即"完整放入目标框".
// 算法: ratio = min(dstW/srcW, dstH/srcH).
func (s *ScaleProcessor) fitInside(srcW, srcH int, img image.Image) image.Image {
	dstW, dstH := s.validSize(srcW, srcH)
	ratio := math.Min(float64(dstW)/float64(srcW), float64(dstH)/float64(srcH))
	if ratio >= 1.0 {
		// 原图已小于目标, 不放大避免画质损失
		return img
	}
	newW := int(math.Round(float64(srcW) * ratio))
	newH := int(math.Round(float64(srcH) * ratio))
	return resize(img, newW, newH)
}

// fitOutsideCrop 等比外接+裁剪: 先做等比外接, 再从中心裁剪到目标尺寸.
// 适用于缩略图/封面色块等"既要填满又要固定比例"的场景.
func (s *ScaleProcessor) fitOutsideCrop(srcW, srcH int, img image.Image) image.Image {
	dstW, dstH := s.validSize(srcW, srcH)
	ratio := math.Max(float64(dstW)/float64(srcW), float64(dstH)/float64(srcH))
	scaledW := int(math.Round(float64(srcW) * ratio))
	scaledH := int(math.Round(float64(srcH) * ratio))

	var scaled image.Image = img
	if scaledW != srcW || scaledH != srcH {
		scaled = resize(img, scaledW, scaledH)
	}

	// 中心裁剪到目标尺寸
	offX := (scaledW - dstW) / 2
	offY := (scaledH - dstH) / 2
	cropRect := image.Rect(offX, offY, offX+dstW, offY+dstH)

	// 边界保护: 若缩放后小于目标 (理论上 cover 模式不会发生, 但保留兜底)
	if !cropRect.In(image.Rect(0, 0, scaledW, scaledH)) {
		return scaled
	}
	return crop(scaled, cropRect)
}

// lockSide 等比锁定单边: 宽和高必须有一个为 0.
//   - width = 0: 高度固定为 s.height, 宽度按 srcW/srcH * s.height 计算.
//   - height = 0: 宽度固定为 s.width, 高度按 srcH/srcW * s.width 计算.
//   - 均为 0 或均非 0: 兜底返回原图 (由调用方控制输入合法性).
func (s *ScaleProcessor) lockSide(srcW, srcH int, img image.Image) image.Image {
	switch {
	case s.width == 0 && s.height == 0:
		return img
	case s.width == 0 && s.height > 0:
		ratio := float64(s.height) / float64(srcH)
		newW := int(math.Round(float64(srcW) * ratio))
		return resize(img, newW, s.height)
	case s.height == 0 && s.width > 0:
		ratio := float64(s.width) / float64(srcW)
		newH := int(math.Round(float64(srcH) * ratio))
		return resize(img, s.width, newH)
	default:
		// 宽高均非 0, 不符合"锁定单边"语义, 兜底返回原图
		return img
	}
}

// validSize 校验目标宽高合法性, 返回兜底后的可用值.
// 目标值 <= 0 时退化为源图尺寸 (即不缩放).
func (s *ScaleProcessor) validSize(srcW, srcH int) (int, int) {
	w := s.width
	h := s.height
	if w <= 0 {
		w = srcW
	}
	if h <= 0 {
		h = srcH
	}
	return w, h
}

// resize 使用 CatmullRom 算法将 src 缩放到 (dstW, dstH).
// 输出统一为 *image.NRGBA, 避免下游 PNG 编码器再次处理透明通道.
func resize(src image.Image, dstW, dstH int) *image.NRGBA {
	if dstW <= 0 || dstH <= 0 {
		// 非法目标尺寸, 退化为原图副本
		bounds := src.Bounds()
		out := image.NewNRGBA(image.Rect(0, 0, bounds.Dx(), bounds.Dy()))
		xdraw.Draw(out, out.Bounds(), src, bounds.Min, xdraw.Src)
		return out
	}
	dst := image.NewNRGBA(image.Rect(0, 0, dstW, dstH))
	xdraw.CatmullRom.Scale(dst, dst.Bounds(), src, src.Bounds(), xdraw.Over, nil)
	return dst
}

// crop 在 dst 坐标系中按 rect 截取子图, 返回 *image.NRGBA.
// 假设 rect 已通过调用方边界校验.
func crop(src image.Image, rect image.Rectangle) *image.NRGBA {
	w, h := rect.Dx(), rect.Dy()
	out := image.NewNRGBA(image.Rect(0, 0, w, h))
	xdraw.Draw(out, out.Bounds(), src, rect.Min, xdraw.Src)
	return out
}
