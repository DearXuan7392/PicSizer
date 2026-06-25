package preprocess

import (
	"PicSizer/internal/core/setting"
	"image"
	"math"

	xdraw "golang.org/x/image/draw"
)

// ScaleProcessor 缩放预处理器, 根据 mode 选择不同的缩放策略.
type ScaleProcessor struct {
	mode   setting.ScaleType
	width  int
	height int
}

// newScaleProcessor 创建缩放预处理器实例.
func newScaleProcessor(mode setting.ScaleType, width, height int) *ScaleProcessor {
	return &ScaleProcessor{mode: mode, width: width, height: height}
}

// Name 返回预处理器名称.
func (s *ScaleProcessor) Name() string {
	return "ScaleProcessor"
}

// Enabled 判断是否需要执行缩放处理, 仅当 mode 不为 ScaleNone 时启用.
func (s *ScaleProcessor) Enabled() bool {
	return s.mode != setting.ScaleNone
}

// Process 执行缩放处理, 根据 mode 分发到具体的缩放算法.
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

// stretch 强制拉伸, 无视宽高比直接缩放到指定尺寸.
func (s *ScaleProcessor) stretch(srcW, srcH int, img image.Image) image.Image {
	dstW, dstH := s.validSize(srcW, srcH)
	if dstW == srcW && dstH == srcH {
		return img
	}
	return resize(img, dstW, dstH)
}

// fitOutside 等比外接（cover）, 缩放后宽和高均不小于目标尺寸.
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

// fitInside 等比内接（contain）, 缩放后宽和高均不超过目标尺寸. 原图已小于目标时不放大.
func (s *ScaleProcessor) fitInside(srcW, srcH int, img image.Image) image.Image {
	dstW, dstH := s.validSize(srcW, srcH)
	ratio := math.Min(float64(dstW)/float64(srcW), float64(dstH)/float64(srcH))
	if ratio >= 1.0 {
		return img
	}
	newW := int(math.Round(float64(srcW) * ratio))
	newH := int(math.Round(float64(srcH) * ratio))
	return resize(img, newW, newH)
}

// fitOutsideCrop 等比外接后居中裁剪到目标尺寸.
func (s *ScaleProcessor) fitOutsideCrop(srcW, srcH int, img image.Image) image.Image {
	dstW, dstH := s.validSize(srcW, srcH)
	ratio := math.Max(float64(dstW)/float64(srcW), float64(dstH)/float64(srcH))
	scaledW := int(math.Round(float64(srcW) * ratio))
	scaledH := int(math.Round(float64(srcH) * ratio))

	var scaled image.Image = img
	if scaledW != srcW || scaledH != srcH {
		scaled = resize(img, scaledW, scaledH)
	}

	offX := (scaledW - dstW) / 2
	offY := (scaledH - dstH) / 2
	cropRect := image.Rect(offX, offY, offX+dstW, offY+dstH)

	if !cropRect.In(image.Rect(0, 0, scaledW, scaledH)) {
		return scaled
	}
	return crop(scaled, cropRect)
}

// lockSide 等比锁定单边, 宽或高必须有一个为 0, 按另一条边等比缩放.
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
		return img
	}
}

// validSize 校验目标宽高合法性, 小于等于 0 时退化为源图尺寸.
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

// resize 使用 CatmullRom 算法进行高质量缩放, 输出 *image.NRGBA.
func resize(src image.Image, dstW, dstH int) *image.NRGBA {
	if dstW <= 0 || dstH <= 0 {
		bounds := src.Bounds()
		out := image.NewNRGBA(image.Rect(0, 0, bounds.Dx(), bounds.Dy()))
		xdraw.Draw(out, out.Bounds(), src, bounds.Min, xdraw.Src)
		return out
	}
	dst := image.NewNRGBA(image.Rect(0, 0, dstW, dstH))
	xdraw.CatmullRom.Scale(dst, dst.Bounds(), src, src.Bounds(), xdraw.Over, nil)
	return dst
}

// crop 按指定矩形区域裁剪图像, 输出 *image.NRGBA.
func crop(src image.Image, rect image.Rectangle) *image.NRGBA {
	w, h := rect.Dx(), rect.Dy()
	out := image.NewNRGBA(image.Rect(0, 0, w, h))
	xdraw.Draw(out, out.Bounds(), src, rect.Min, xdraw.Src)
	return out
}
