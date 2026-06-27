package preprocess

import (
	"PicSizer/internal/core/settingLoader"
	"image"
	"math"

	xdraw "golang.org/x/image/draw"
)

// ScaleProcessor 缩放预处理器, 根据 mode 选择不同的缩放策略.
type ScaleProcessor struct {
	mode   settingLoader.ScaleType
	width  int
	height int
}

// newScaleProcessor 创建缩放预处理器实例.
func newScaleProcessor(mode settingLoader.ScaleType, width, height int) *ScaleProcessor {
	return &ScaleProcessor{mode: mode, width: width, height: height}
}

// Name 返回预处理器名称.
func (s *ScaleProcessor) Name() string {
	return "ScaleProcessor"
}

// Enabled 判断是否需要执行缩放处理, 仅当 mode 不为 ScaleNone 时启用.
func (s *ScaleProcessor) Enabled() bool {
	return s.mode != settingLoader.ScaleNone
}

// Process 执行缩放处理, 根据 mode 分发到具体的缩放算法.
func (s *ScaleProcessor) Process(img image.Image) image.Image {
	logger.Debug("start preprocess: %s", s.Name())

	if img == nil {
		return img
	}

	if s.mode == settingLoader.ScaleLockSide {
		if s.width <= 0 && s.height <= 0 {
			logger.Error("scale lock side: width and height are both 0, do nothing")
			return img
		}
	} else {
		if s.width <= 0 || s.height <= 0 {
			logger.Error("width or height is 0, do nothing")
			return img
		}
	}

	switch s.mode {
	case settingLoader.ScaleStretch:
		return s.stretch(img)
	case settingLoader.ScaleFitOutside:
		return s.fitOutside(img)
	case settingLoader.ScaleFitInside:
		return s.fitInside(img)
	case settingLoader.ScaleFitOutsideCrop:
		return s.fitOutsideCrop(img)
	case settingLoader.ScaleLockSide:
		return s.lockSide(img)
	default:
		return img
	}
}

// stretch 强制拉伸, 无视宽高比直接缩放到指定尺寸.
func (s *ScaleProcessor) stretch(img image.Image) image.Image {
	logger.Debug("scale stretch: stretch image to %dx%d", s.width, s.height)
	return resize(img, s.width, s.height)
}

// fitOutside 等比外接 (cover), 缩放后宽和高均不小于目标尺寸.
func (s *ScaleProcessor) fitOutside(img image.Image) image.Image {
	srcW, srcH := img.Bounds().Dx(), img.Bounds().Dy()
	ratio := math.Max(float64(s.width)/float64(srcW), float64(s.height)/float64(srcH))
	newW := int(math.Round(float64(srcW) * ratio))
	newH := int(math.Round(float64(srcH) * ratio))
	logger.Debug("scale fit outside: fit image to %dx%d", newW, newH)
	return resize(img, newW, newH)
}

// fitInside 等比内接 (contain), 缩放后宽和高均不超过目标尺寸.
func (s *ScaleProcessor) fitInside(img image.Image) image.Image {
	srcW, srcH := img.Bounds().Dx(), img.Bounds().Dy()
	ratio := math.Min(float64(s.width)/float64(srcW), float64(s.height)/float64(srcH))
	newW := int(math.Round(float64(srcW) * ratio))
	newH := int(math.Round(float64(srcH) * ratio))
	logger.Debug("scale fit inside: fit image to %dx%d", newW, newH)
	return resize(img, newW, newH)
}

// fitOutsideCrop 等比外接后居中裁剪到目标尺寸.
func (s *ScaleProcessor) fitOutsideCrop(img image.Image) image.Image {
	srcW, srcH := img.Bounds().Dx(), img.Bounds().Dy()
	ratio := math.Max(float64(s.width)/float64(srcW), float64(s.height)/float64(srcH))
	scaledW := int(math.Round(float64(srcW) * ratio))
	scaledH := int(math.Round(float64(srcH) * ratio))

	var scaled = img
	if scaledW != srcW || scaledH != srcH {
		scaled = resize(img, scaledW, scaledH)
	}

	offX := (scaledW - s.width) / 2
	offY := (scaledH - s.height) / 2
	cropRect := image.Rect(offX, offY, offX+s.width, offY+s.height)

	if !cropRect.In(image.Rect(0, 0, scaledW, scaledH)) {
		logger.Warn("crop rect (%d, %d, %d, %d) outside of scaled image with width %d, height %d", offX, offY, offX+s.width, offY+s.height, scaledW, scaledH)
		return scaled
	}
	logger.Debug("scale fit outside crop: crop image to %dx%d", s.width, s.height)
	return crop(scaled, cropRect)
}

// lockSide 等比锁定单边, 宽或高必须有一个为 0, 按另一条边等比缩放.
func (s *ScaleProcessor) lockSide(img image.Image) image.Image {
	srcW, srcH := img.Bounds().Dx(), img.Bounds().Dy()
	if s.width == 0 && s.height > 0 {
		ratio := float64(s.height) / float64(srcH)
		newW := int(math.Round(float64(srcW) * ratio))
		logger.Debug("scale lock side: fit image to %dx%d", newW, s.height)
		return resize(img, newW, s.height)
	} else if s.height == 0 && s.width > 0 {
		ratio := float64(s.width) / float64(srcW)
		newH := int(math.Round(float64(srcH) * ratio))
		logger.Debug("scale lock side: fit image to %dx%d", s.width, newH)
		return resize(img, s.width, newH)
	} else {
		logger.Warn("scale lock side: width and height are both 0, do nothing")
		return img
	}
}

// resize 使用 CatmullRom 算法进行高质量缩放, 输出 *image.NRGBA.
func resize(src image.Image, dstW, dstH int) image.Image {
	if src.Bounds().Dx() == dstW && src.Bounds().Dy() == dstH {
		logger.Debug("resize failed: src image size is same as dst size, do nothing")
		return src
	}

	var dst xdraw.Image
	dstRect := image.Rect(0, 0, dstW, dstH)

	switch src.(type) {
	case *image.RGBA:
		dst = image.NewRGBA(dstRect)
	case *image.NRGBA:
		dst = image.NewNRGBA(dstRect)
	default:
		logger.Warn("resize failed: unknown image type, do nothing")
		return src
	}

	xdraw.CatmullRom.Scale(dst, dstRect, src, src.Bounds(), xdraw.Over, nil)
	logger.Debug("resize success, dst size: %dx%d", dstW, dstH)
	return dst
}

// crop 按指定矩形区域裁剪图像, 输出 image.Image.
func crop(src image.Image, rect image.Rectangle) image.Image {
	w, h := rect.Dx(), rect.Dy()
	var out xdraw.Image
	switch src.(type) {
	case *image.RGBA:
		out = image.NewRGBA(image.Rect(0, 0, w, h))
	case *image.NRGBA:
		out = image.NewNRGBA(image.Rect(0, 0, w, h))
	default:
		logger.Warn("crop warn: unknown image type, use NRGBA instead")
		out = image.NewNRGBA(image.Rect(0, 0, w, h))
	}
	xdraw.Draw(out, out.Bounds(), src, rect.Min, xdraw.Src)
	logger.Debug("crop success, dst size: %dx%d", w, h)
	return out
}
