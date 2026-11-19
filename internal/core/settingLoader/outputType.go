package settingLoader

type OutputType int

const (
	OutputCoverOrigin OutputType = iota // 覆盖源文件
	OutputStructure                     // 输出到目标目录并保留相对目录结构
	OutputDirection                     // 全部输出到统一目录
)
