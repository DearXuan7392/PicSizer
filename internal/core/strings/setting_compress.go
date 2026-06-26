package strs

// 压缩设置
var (
	TextCompressSetting = "压缩设置"
	TextCompressMode    = "压缩模式"
	TextQualityMode     = "按质量压缩"
	TextFileSizeMode    = "按大小压缩"
	TextQuality         = "画质"
	TextQualityBest     = "最佳"
	TextQualityClear    = "清晰"
	TextQualityNormal   = "一般"
	TextQualityPoor     = "较差"
	TextLimitSize       = "限制大小"
	TextAcceptExceed    = "接受超出大小的图片"
	TextMaxThreads      = "最大线程数"
	TextTopMost         = "窗口置顶"

	TextUnitKB = "KB"
	TextUnitMB = "MB"

	TipCompressMode = "按质量压缩: \n      指定输出画质, 速度较快\n按大小压缩: \n      限制压缩后文件大小, 由程序自动计算最佳画质, 但更耗时"
	TipQuality      = "按质量压缩模式下, 设置输出图片的画质等级, 共 4 档: \n      最佳 (95) / 清晰 (80) / 一般 (40) / 较差 (10)\n等级越高画质越好, 但文件体积越大"
	TipLimitSize    = "按大小压缩模式下, 设置输出文件的最大体积, 最终大小不会超过这个值"
	TipAcceptExceed = "勾选后, 对于无法压缩到指定大小以内的图片, 仍然输出所能达到的体积最小的文件\n若取消勾选, 则无法压缩到指定大小以内的图片会直接报错"
	TipMaxThreads   = "设置并发压缩的线程数, 过多时会导致程序卡顿"
	TipThreadSlider = "拖拽滑块快速调整线程数"
	TipTopMost      = "勾选后窗口始终置顶"
)
