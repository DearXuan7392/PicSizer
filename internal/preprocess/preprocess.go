package preprocess

import (
	"PicSizer/internal/core/settingLoader"
	"image"
)

// Preprocessor 定义预处理器接口.
// 不同的预处理策略（透明度处理、缩放等）均可实现该接口,
// 入口 Process 会按注册顺序依次执行所有已启用的处理器.
type Preprocessor interface {
	Name() string
	Enabled() bool
	Process(img image.Image) image.Image
}

// newPreprocessors 根据当前全局配置构造所有预处理器实例.
// 顺序即为执行顺序, 新增预处理器只需在此函数内追加即可.
func newPreprocessors(setting settingLoader.Setting) []Preprocessor {
	return []Preprocessor{
		newAlphaProcessor(setting.AlphaHandle),
		newScaleProcessor(setting.Scale, setting.ScaleWidth, setting.ScaleHeight),
	}
}

// Process 是统一预处理入口, 按注册顺序依次执行所有已启用的预处理器.
// 返回处理后的图像, 任意一步不会因失败中断整体流程.
func Process(img image.Image) image.Image {
	if img == nil {
		return img
	}
	setting := settingLoader.GetSetting()
	processors := newPreprocessors(setting)
	for _, p := range processors {
		if p == nil || !p.Enabled() {
			continue
		}
		img = p.Process(img)
	}
	return img
}
