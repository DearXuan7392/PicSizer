package utils

import (
	"image"
)

// HasAlphaPixel 遍历图像像素, 判断是否存在非 255 的 alpha 值.
// 入参图像已由上游管道保证为 *image.RGBA, *image.NRGBA 类型.
// 返回 true 表示至少存在一个透明像素 (alpha < 255), 透明通道承载有效信息.
func HasAlphaPixel(img image.Image) bool {
	if img == nil {
		return false
	}

	var pix []byte

	switch strideImg := img.(type) {
	case *image.RGBA:
		pix = strideImg.Pix
	case *image.NRGBA:
		pix = strideImg.Pix
	default:
		return false
	}

	if len(pix) == 0 {
		return false
	}

	n := len(pix)
	_ = pix[n-1]

	for i := 3; i < n; i += 4 {
		if pix[i] < 255 {
			return true
		}
	}

	return false
}
