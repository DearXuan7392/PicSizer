package preprocess

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/log"
	"image"
)

// Preprocessor 预处理器接口. 入口 Process 按注册顺序依次执行已启用的处理器.
type Preprocessor interface {
	Name() string
	Enabled() bool
	Process(img image.Image) image.Image
}

var (
	logger = log.NewLogger("preprocess")
)

// newPreprocessors 构造当前配置下的所有预处理器.
// 切片顺序即执行顺序, 新增预处理器只需在此追加.
func newPreprocessors(setting settingLoader.Setting) []Preprocessor {
	return []Preprocessor{
		newAlphaProcessor(setting.AlphaHandle),
		newScaleProcessor(setting.Scale, setting.ScaleWidth, setting.ScaleHeight),
	}
}

// Process 统一预处理入口, 各处理器失败时自行降级, 不会中断整体流程.
func Process(img image.Image) image.Image {
	if img == nil {
		logger.Error("preprocess skipped, image is nil")
		return nil
	}
	setting := settingLoader.GetSetting()
	logger.Debug("preprocess start, bounds: %v, type: %T", img.Bounds(), img)

	processors := newPreprocessors(setting)
	for _, p := range processors {
		if p == nil || !p.Enabled() {
			continue
		}
		logger.Debug("preprocess step, name: %s", p.Name())
		img = p.Process(img)
	}

	logger.Debug("preprocess done, bounds: %v, type: %T", img.Bounds(), img)
	return img
}
