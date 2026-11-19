package preprocess

import (
	"PicSizer/internal/core/settingLoader"
	"image"

	"PicSizer/internal/utils"
)

type AlphaProcessor struct {
	mode settingLoader.AlphaHandleType
}

func newAlphaProcessor(mode settingLoader.AlphaHandleType) *AlphaProcessor {
	return &AlphaProcessor{mode: mode}
}

func (a *AlphaProcessor) Name() string {
	return "AlphaProcessor"
}

func (a *AlphaProcessor) Enabled() bool {
	return a.mode != settingLoader.AlphaKeep
}

// Process 执行透明通道处理, 均通过白底合成实现.
// 智能移除: 仅当图中没有任何透明像素时才移除通道; 全部移除: 不检查直接合成.
func (a *AlphaProcessor) Process(img image.Image) image.Image {
	logger.Debug("alpha processor start, mode: %s", settingLoader.AlphaHandleName(a.mode))

	if img == nil {
		return nil
	}

	switch img.(type) {
	case *image.NRGBA, *image.NRGBA64, *image.NYCbCrA:
		break
	default:
		logger.Debug("alpha processor skip, type: %T has no alpha channel", img)
		return img
	}

	bounds := img.Bounds()

	switch a.mode {
	case settingLoader.AlphaSmartRemove:
		if utils.HasAlphaPixel(img) {
			logger.Debug("alpha processor keep, mode: smart-remove, image has alpha pixel")
			return img
		}
		logger.Debug("alpha processor composite on white, mode: smart-remove, image has no alpha pixel")
		return utils.CompositeOnWhite(img, bounds)
	case settingLoader.AlphaRemove:
		logger.Debug("alpha processor composite on white, mode: remove")
		return utils.CompositeOnWhite(img, bounds)
	default:
		logger.Debug("alpha processor keep, mode: %s", settingLoader.AlphaHandleName(a.mode))
		return img
	}
}
