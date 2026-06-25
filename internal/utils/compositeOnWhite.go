package utils

import (
	"image"
	"image/color"
	"image/draw"
)

// CompositeOnWhite 将图像按白色背景叠加合成, 返回不含透明通道的 *image.RGBA.
// 创建白色画布, 用 draw.Src 铺白底, 再用 draw.Over 叠加上原图.
// 原图 A=0 的区域透出白色, 0<A<255 的半透明区域与白色混合.
func CompositeOnWhite(img image.Image, bounds image.Rectangle) *image.RGBA {
	dstImg := image.NewRGBA(bounds)

	whiteDst := &image.Uniform{color.RGBA{R: 255, G: 255, B: 255, A: 255}}
	draw.Draw(dstImg, dstImg.Bounds(), whiteDst, image.Point{}, draw.Src)

	draw.Draw(dstImg, dstImg.Bounds(), img, bounds.Min, draw.Over)

	return dstImg
}
