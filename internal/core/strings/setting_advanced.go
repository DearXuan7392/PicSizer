package strs

// 高级设置
var (
	TextAdvJPEGSettings     = "JPEG 设置"
	TextAdvPNGSettings      = "PNG 设置"
	TextAdvWebPSettings     = "WebP 设置"
	TextAdvFineQuality      = "精细化画质"
	TextPngKeepIndexedAlpha = "为索引编码预留透明像素"
	TextPngPaletteAlgo      = "调色盘生成算法"
	TextPngEnableDithering  = "启用抖动算法"
	TextDebugMode           = "开启调试"

	TipAdvFineQuality      = "为 0 时不启用. 如果非 0, 则按画质压缩时, 覆盖 jpeg 图片的画质. "
	TipAdvFineQualityWebP  = "为 0 时不启用. 如果非 0, 则按画质压缩时, 覆盖 webp 图片的画质. "
	TipPngKeepIndexedAlpha = "在压缩为 PNG 格式时, 有时会将图像转换为索引 (调色板) 编码. \n勾选后, 将在调色板中为透明像素预留位置, 可避免透明背景变色或产生毛边;\n若图片中存在全透明像素, 建议开启该选项;\n该功能无法避免半透明像素的改变. "
	TipPngPaletteAlgo      = "选择生成 PNG 索引编码图像时采用的算法:\n      中位切分 (Median Cut): 基于像素出现数量均匀分割色彩空间, 适合风景照, 人像, 或大面积自然渐变图像, 压缩速度较快;\n      均值聚类 (K-Means): 基于色彩距离生成调色盘, 能够更好地保护低频但高对比度颜色, 适合夜景, 动漫, Logo, 等色彩差距较大的图像, 压缩速度较慢."
	TipPngEnableDithering  = "勾选后开启 Floyd-Steinberg 抖动算法, 使用离散像素模拟图像中的渐变效果, 使图像更平滑, 但生成文件更大;\n取消勾选则颜色纯净且体积更小, 但可能出现色彩断层, 仅在图片棱角分明时关闭."
	TipDebugMode           = "启用调试后, 将在程序目录下生成 PicSizerLog.log 文件以记录日志, 并对性能造成一定影响;\n开启后程序会自动重启, 仅在必要时开启."

	TextPaletteMedianCut = "中位切分 (Median Cut)"
	TextPaletteKMeans    = "均值聚类 (K-Means)"
)
