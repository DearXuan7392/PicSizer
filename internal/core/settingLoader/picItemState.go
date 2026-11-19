package settingLoader

type PicItemState int

const (
	StateWaiting PicItemState = iota
	StateCompressing
	StateSuccess
	StateOutOfLimit
	StateError
)
