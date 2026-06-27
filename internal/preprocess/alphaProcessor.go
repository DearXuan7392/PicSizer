package preprocess

import (
	"PicSizer/internal/core/settingLoader"
	"image"

	"PicSizer/internal/utils"
)

// AlphaProcessor 透明通道预处理器.
// 根据 mode 字段区分处理策略: 保留、智能移除或全部移除.
type AlphaProcessor struct {
	mode settingLoader.AlphaHandleType
}

// newAlphaProcessor 创建透明通道预处理器实例.
func newAlphaProcessor(mode settingLoader.AlphaHandleType) *AlphaProcessor {
	return &AlphaProcessor{mode: mode}
}

// Name 返回预处理器名称.
func (a *AlphaProcessor) Name() string {
	return "AlphaProcessor"
}

// Enabled 判断是否需要执行透明通道处理.
// 保留模式 (AlphaKeep) 不做处理, 其他模式均视为启用.
func (a *AlphaProcessor) Enabled() bool {
	return a.mode != settingLoader.AlphaKeep
}

// Process 执行透明通道处理.
// 智能移除模式: 仅当透明通道冗余 (无任何非255像素) 时按白色背景合成移除通道.
// 全部移除模式: 不做检查, 直接按白色背景合成.
func (a *AlphaProcessor) Process(img image.Image) image.Image {
	logger.Debug("start preprocess: %s", a.Name())

	if img == nil {
		return nil
	}

	switch img.(type) {
	case *image.NRGBA, *image.NRGBA64, *image.NYCbCrA:
		break
	default:
		logger.Debug("no alpha channel in image, skip preprocess")
		return img
	}

	bounds := img.Bounds()

	switch a.mode {
	case settingLoader.AlphaSmartRemove:
		if utils.HasAlphaPixel(img) {
			logger.Debug("smart remove: image has alpha pixel, skip preprocess")
			return img
		}
		logger.Debug("smart remove: image has no alpha pixel, composite on white background")
		return utils.CompositeOnWhite(img, bounds)
	case settingLoader.AlphaRemove:
		logger.Debug("remove alpha: composite on white background")
		return utils.CompositeOnWhite(img, bounds)
	default:
		// 理论上不会走到这里
		logger.Debug("keep alpha: do nothing")
		return img
	}
}
