package setting

// PaletteAlgoType 表示 PNG 调色盘生成算法的枚举类型.
type PaletteAlgoType int

const (
	PaletteMedianCut PaletteAlgoType = iota // 中位切分 (Median Cut)
	PaletteKMeans                           // 均值聚类 (K-Means)
)
