package setting

// 图片项目状态
type PicItemState int

const (
	StateWaiting PicItemState = iota
	StateCompressing
	StateSuccess
	StateOutOfLimit
	StateError
)
