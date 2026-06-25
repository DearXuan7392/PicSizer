// Package utils 提供项目内部使用的通用工具函数.
package utils

import (
	"image"
)

// HasAlphaPixel 遍历图像像素判断是否存在非 255 的 alpha 值.
// 此时入参图像已经通过 normalize 管道严格限制为 *image.RGBA 或 *image.NRGBA.
//
// 入参为 nil 时直接返回 false, 调用方无需额外判空.
//
// 返回值:
//   - true  : 至少存在一个非完全不透明的像素, 透明通道承载了有效信息.
//   - false : 所有像素 alpha 均为 255, 透明通道冗余 (或图像为空).
func HasAlphaPixel(img image.Image) bool {
	if img == nil {
		return false
	}

	var pix []byte

	// 1. 严格分流：由于上游管道保证，这里只会命中这两个分支之一
	switch strideImg := img.(type) {
	case *image.RGBA:
		pix = strideImg.Pix
	case *image.NRGBA:
		pix = strideImg.Pix
	default:
		// 理论上永远不会走到这里，但为了代码严谨性做个安全兜底
		return false
	}

	if len(pix) == 0 {
		return false
	}

	n := len(pix)

	// 2. ⚡ 边界检查消除 (BCE)：向编译器自证安全，直接抽离循环内的边界判断指令
	_ = pix[n-1]

	// 3. 🚀 极致连续内存扫描：步长为 4，只看 A 通道
	for i := 3; i < n; i += 4 {
		if pix[i] < 255 {
			return true // 发现透明像素，提前退出
		}
	}

	return false
}
