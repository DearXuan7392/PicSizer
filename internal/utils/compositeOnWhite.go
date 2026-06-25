// Package utils 提供项目内部使用的通用工具函数.
package utils

import (
	"image"
	"image/color"
	"image/draw"
)

// CompositeOnWhite 将图像按白色背景叠加, 返回 NRGBA 图像 (无透明通道).
//
// 合成步骤:
//  1. 创建一个与 bounds 同尺寸的 *image.NRGBA 画布, NRGBA 代表 Non-Alpha-Premultiplied RGBA,
//     非常适合用于处理或导出最终的不透明图像.
//  2. 使用 draw.Src 在画布上铺一层纯白底色 (R=G=B=255, A=255), 整个画布变为完全不透明.
//  3. 使用 draw.Over (叠加) 将原图绘制到白色画布上:
//     - 原图 A=0 的区域会完全透出底部的白色, 不会产生黑色透明像素.
//     - 0 < A < 255 的半透明边缘会与白色完美混合, 自然消除 PNG 半透明区域常见的黑色锯齿边.
//
// 合成公式等价于:
//   - out.r = src.r * (a/255) + 255 * (1 - a/255)
//   - out.g = src.g * (a/255) + 255 * (1 - a/255)
//   - out.b = src.b * (a/255) + 255 * (1 - a/255)
//   - out.a = 255
//
// 当 a == 255 时退化为 out = src, 此时相当于仅移除透明通道并保留 RGB 数据.
//
// 入参说明:
//   - codec    : 源图像, 任意 image.Image 均可, 由 image/draw 通过通用 At 访问像素.
//   - bounds : 源图像的有效像素范围, 同时也是输出画布尺寸, 通常传入 codec.Bounds().
//
// 返回值:
//   - 与 bounds 同尺寸的 *image.NRGBA, 像素全部为完全不透明 (a = 255).
func CompositeOnWhite(img image.Image, bounds image.Rectangle) *image.RGBA {
	// 1. 创建一个新的 NRGBA 画布, 大小由传入的 bounds 决定
	dstImg := image.NewRGBA(bounds)

	// 2. 首先在画布上绘制一层纯白色的固体背景
	// draw.Src 表示完全覆盖, 这一步让整个画布变成不透明的纯白
	whiteDst := &image.Uniform{color.RGBA{R: 255, G: 255, B: 255, A: 255}}
	draw.Draw(dstImg, dstImg.Bounds(), whiteDst, image.Point{}, draw.Src)

	// 3. 将原图通过 draw.Over (叠加) 模式绘制到白色画布上
	// 原图中 A=0 的地方会完全透出白色, 0 < A < 255 的半透明边缘会与白色完美混合 (消除锯齿黑边)
	// image.Point{} 表示从原图的左上角开始读取
	draw.Draw(dstImg, dstImg.Bounds(), img, bounds.Min, draw.Over)

	return dstImg
}
