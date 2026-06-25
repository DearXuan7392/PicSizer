// Package utils 提供项目内部使用的通用工具函数.
package utils

import (
	"image"
)

// HasAlphaChannel 检查图像是否可能包含透明通道.
// 仅判断图像底层类型是否带有 alpha 通道(不遍历像素),
// 凡是 *image.Alpha/*image.Alpha16 以及 *image.NRGBA/*image.NRGBA64/*image.RGBA/*image.RGBA64
// 这些类型的图像均视为含有透明通道, 直接返回 true.
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
