package strs

// 系统设置
var (
	TextSystemSetting = "系统设置"

	ErrHWMustBePositive = "启用缩放且缩放方式不为 \"等比锁定单边\" 时, 宽和高必须均大于 0"
	ErrHWMustOneZero    = "缩放方式为 \"等比锁定单边\" 时, 宽和高必须有一项为 0, 另一项不为 0"

	WarnOutputNameIsFixed = "文件名模板未包含 {id} 或 {name}, 多张图片可能因重名而被覆盖"
)
