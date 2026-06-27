package utils

import (
	"image"
	"image/draw"
)

func ConvertToRGBA(src image.Image) image.Image {
	if rgbaImg, ok := src.(*image.RGBA); ok {
		return rgbaImg
	}

	img := image.NewRGBA(src.Bounds())
	draw.Draw(img, img.Bounds(), src, image.Point{}, draw.Src)
	return img
}

func ConvertToNRGBA(src image.Image) image.Image {
	if rgbaImg, ok := src.(*image.NRGBA); ok {
		return rgbaImg
	}

	img := image.NewNRGBA(src.Bounds())
	draw.Draw(img, img.Bounds(), src, image.Point{}, draw.Src)
	return img
}
