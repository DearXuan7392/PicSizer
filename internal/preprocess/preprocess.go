// Package preprocess 提供图片压缩前的预处理流程.
// 加载图片后, 压缩前, 可通过本包对图像进行预处理 (例如透明通道处理、裁剪等).
// 包内通过统一的 Preprocessor 接口串联多个处理器, 新增处理逻辑只需实现接口并注册到 pipeline.
package preprocess

import (
	"PicSizer/internal/core/setting"
	"image"
)

// Preprocessor 预处理器接口.
// 不同的预处理策略 (例如透明度处理、裁剪、缩放等) 均可实现该接口,
// 入口 Process 会按注册顺序依次执行所有已启用的处理器.
type Preprocessor interface {
	// Name 返回预处理器名称, 用于日志或调试.
	Name() string
	// Enabled 返回当前预处理器是否需要执行.
	// 当用户在设置中选择"不做处理"或对应开关关闭时, 应返回 false.
	Enabled() bool
	// Process 对图像执行预处理, 返回处理后的 image.Image.
	// 若无需修改, 可直接返回原 codec.
	Process(img image.Image) image.Image
}

// newPreprocessors 根据当前全局配置构造所有预处理器实例.
// 顺序即为执行顺序, 越靠前的处理器越先作用在原图上.
// 后续新增的预处理器只需在此函数内追加即可, 无需改动 Process 主流程.
func newPreprocessors(setting setting.Setting) []Preprocessor {
	return []Preprocessor{
		newAlphaProcessor(setting.AlphaHandle),
		newScaleProcessor(setting.Scale, setting.ScaleWidth, setting.ScaleHeight),
	}
}

// Process 统一预处理入口, 按注册顺序依次执行所有已启用的预处理器.
// 返回处理后的图像, 任意一步不会因失败中断整体流程 (处理器内部应自行保证容错).
func Process(img image.Image) image.Image {
	if img == nil {
		return img
	}
	setting := setting.GetSetting()
	processors := newPreprocessors(setting)
	for _, p := range processors {
		if p == nil || !p.Enabled() {
			continue
		}
		img = p.Process(img)
	}
	return img
}
