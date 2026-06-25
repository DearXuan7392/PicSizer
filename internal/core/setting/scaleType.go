package setting

// ScaleType 表示缩放方式的枚举类型。
// 控制图像在压缩前是否进行尺寸调整，共 6 种模式。
type ScaleType int

const (
	ScaleNone           ScaleType = iota // 无操作, 不缩放
	ScaleStretch                         // 强制拉伸: 无视宽高比, 强制缩放到指定尺寸
	ScaleFitOutside                      // 等比外接: 缩放后宽高均 >= 目标尺寸 (cover)
	ScaleFitInside                       // 等比内接: 缩放后宽高均 <= 目标尺寸 (contain)
	ScaleFitOutsideCrop                  // 等比外接+裁剪: 先 cover 缩放再中心裁剪
	ScaleLockSide                        // 等比锁定单边: 宽高其一为 0, 按另一边固定缩放
)
