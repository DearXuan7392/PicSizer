package strs

// 图像预处理
var (
	TextAlphaHandle      = "透明通道处理"
	AlphaKeepText        = "保留"
	AlphaSmartRemoveText = "智能移除"
	AlphaRemoveText      = "全部移除"

	// 缩放方式
	TextScale         = "缩放方式"
	TextScaleWidth    = "宽"
	TextScaleHeight   = "高"
	ScaleNoneText     = "无操作"
	ScaleStretchText  = "强制拉伸"
	ScaleFitOutText   = "等比外接"
	ScaleFitInText    = "等比内接"
	ScaleFitOutCrop   = "等比外接+裁剪"
	ScaleLockSideText = "等比锁定单边"

	TipScale       = "缩放方式: \n\n      无操作: 按原图尺寸输出\n\n      强制拉伸: 破坏原图比例, 强制拉伸到指定大小\n\n      等比外接: 保持原图比例, 缩放至指定尺寸的最小外接矩形. 即宽和高都 大于等于 指定尺寸的最小矩形\n\n      等比内接: 保持原图比例, 缩放至指定尺寸的最大内接矩形. 即宽和高都 小于等于 指定尺寸的最大矩形\n\n      等比外接+裁剪: 保持原图比例, 缩放至指定尺寸的最小外接矩形后, 居中裁剪\n\n      等比锁定单边: 保持原图比例, 将其中一条边缩放至指定大小, 另一条边自适应调整. 输入 \"0\" 表示自动调整此边长"
	TipScaleWidth  = "目标宽度 (像素)\n\"0\" 表示自动调整, 必须与缩放方式配合使用"
	TipScaleHeight = "目标高度 (像素)\n\"0\" 表示自动调整, 必须与缩放方式配合使用"
	TipAlphaHandle = "透明通道处理方式: \n      保留: 输出图片保留透明度通道\n      智能移除: 当存在透明像素时保留透明度通道, 仅在图片全部为不透明像素时移除透明度通道\n      全部移除: 使用白色底色叠加, 并移除透明度通道. 半透明的图片会变为白底不透明图片"
)
