package settingLoader

type AlphaHandleType int

const (
	AlphaKeep        AlphaHandleType = iota // 保留透明通道
	AlphaSmartRemove                        // 智能移除 (无透明像素时才移除)
	AlphaRemove                             // 全部移除 (按白色背景叠加计算)
)
