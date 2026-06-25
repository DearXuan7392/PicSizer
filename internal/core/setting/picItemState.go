package setting

// PicItemState 表示图片项目处理状态的枚举类型。
type PicItemState int

const (
	StateWaiting PicItemState = iota
	StateCompressing
	StateSuccess
	StateOutOfLimit
	StateError
)
