package preprocess

import (
	"PicSizer/internal/core/setting"
	"image"

	"PicSizer/internal/utils"
)

// AlphaProcessor 透明通道预处理器.
// 根据 mode 字段区分处理策略: 保留、智能移除或全部移除.
type AlphaProcessor struct {
	mode setting.AlphaHandleType
}

// newAlphaProcessor 创建透明通道预处理器实例.
func newAlphaProcessor(mode setting.AlphaHandleType) *AlphaProcessor {
	return &AlphaProcessor{mode: mode}
}

// Name 返回预处理器名称.
func (a *AlphaProcessor) Name() string {
	return "AlphaProcessor"
}

// Enabled 判断是否需要执行透明通道处理.
// 保留模式（AlphaKeep）不做处理, 其他模式均视为启用.
func (a *AlphaProcessor) Enabled() bool {
	return a.mode != setting.AlphaKeep
}

// Process 执行透明通道处理.
// 智能移除模式: 仅当透明通道冗余（无任何非255像素）时按白色背景合成移除通道.
// 全部移除模式: 不做检查, 直接按白色背景合成.
func (a *AlphaProcessor) Process(img image.Image) image.Image {
	if img == nil {
		return img
	}

	switch img.(type) {
	case *image.NRGBA, *image.NRGBA64:
		break
	default:
		return img
	}

	bounds := img.Bounds()

	switch a.mode {
	case setting.AlphaSmartRemove:
		if utils.HasAlphaPixel(img) {
			return img
		}
		return utils.CompositeOnWhite(img, bounds)
	case setting.AlphaRemove:
		return utils.CompositeOnWhite(img, bounds)
	default:
		return img
	}
}
