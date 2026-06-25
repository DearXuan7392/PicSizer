package utils

import (
	"image"
)

// HasAlphaChannel 通过图像类型判断是否包含透明通道。
// 仅根据底层类型判断，不遍历像素。*image.NRGBA 视为含透明通道，
// *image.RGBA 和 *image.YCbCr 视为不含透明通道，其他类型 panic。
func HasAlphaChannel(img image.Image) bool {
	switch img.(type) {
	case *image.NRGBA:
		return true
	case *image.RGBA, *image.YCbCr:
		return false
	default:
		panic("codec type not support")
	}
}
