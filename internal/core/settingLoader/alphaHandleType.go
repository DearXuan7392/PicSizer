package settingLoader

// AlphaHandleType 表示透明通道处理方式的枚举类型。
type AlphaHandleType int

const (
	AlphaKeep        AlphaHandleType = iota // 保留透明通道
	AlphaSmartRemove                        // 智能移除 (判断是否存在透明像素)
	AlphaRemove                             // 全部移除 (按白色背景叠加计算)
)
