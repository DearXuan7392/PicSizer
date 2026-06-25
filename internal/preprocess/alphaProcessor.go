package preprocess

import (
	"PicSizer/internal/core/setting"
	"image"

	"PicSizer/internal/utils"
)

// AlphaProcessor 透明通道预处理器.
// 通过 mode 字段区分处理策略:
//   - AlphaKeep        : 不处理 (Enabled 返回 false, 不会进入处理流程).
//   - AlphaSmartRemove : 智能移除, 仅当透明通道冗余 (无任何非 255 像素) 时按白色背景合成移除通道;
//     若存在透明像素, 说明通道承载有效信息, 保留原图.
//   - AlphaRemove      : 全部移除, 直接按白色背景合成.
type AlphaProcessor struct {
	mode setting.AlphaHandleType
}

// newAlphaProcessor 创建透明通道预处理器.
// 工厂方法统一返回非 nil 实例, 是否启用由 Enabled() 内部判断.
func newAlphaProcessor(mode setting.AlphaHandleType) *AlphaProcessor {
	return &AlphaProcessor{mode: mode}
}

// Name 返回预处理器名称.
func (a *AlphaProcessor) Name() string {
	return "AlphaProcessor"
}

// Enabled 判断是否需要执行透明通道处理.
// 保留模式 (AlphaKeep) 不做处理, 其他模式均视为启用.
func (a *AlphaProcessor) Enabled() bool {
	return a.mode != setting.AlphaKeep
}

// Process 执行透明通道处理.
//   - 智能移除: 仅当透明通道冗余 (无任何非 255 像素) 时按白色背景合成移除通道;
//     若存在透明像素则保留原图, 通道不处理.
//   - 全部移除: 不做检查, 直接按白色背景合成.
//
// 已是 *image.NRGBA 格式的图像无需再处理, 直接返回.
func (a *AlphaProcessor) Process(img image.Image) image.Image {
	if img == nil {
		return img
	}

	switch img.(type) {
	case *image.NRGBA, *image.NRGBA64:
		// 含有 A 通道, 继续处理
		break
	default:
		// 不含 A 通道, 直接返回
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
		// 全部移除: 按白色背景合成
		return utils.CompositeOnWhite(img, bounds)
	default:
		// 未知或保留模式理论上不会走到这里, 兜底返回原图
		return img
	}
}
