package setting

// 输出类型
type OutputType int

const (
	OutputCoverOrigin OutputType = iota // 覆盖源文件
	OutputStructure                     // 输出到文件夹并保留结构
	OutputDirection                     // 输出到统一文件夹
)
