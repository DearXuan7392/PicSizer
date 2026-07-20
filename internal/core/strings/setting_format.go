package strs

// 输出格式
var (
	TextOutputFormat = "输出格式"
	TextFormatJPEG   = "JPEG (快速)"
	TextFormatPNG    = "PNG (缓慢)"
	TextFormatWebP   = "WebP (缓慢)"
	TextFormatBMP    = "BMP (快速)"
	TextFormatOrigin = "保持原格式"
	TextFilenameTpl  = "文件名模板"
	TextStartIndex   = "起始下标"

	TipExtension   = "当选择原格式时, 若图片实际格式与后缀名不同, 会以后缀名为准, 自动调整编码方式\nBMP 格式暂不支持压缩, 仅支持输出最佳画质"
	TipFilenameTpl = "文件名模板 (不含后缀), 替换原则如下:\n      {name} 被替换为原始文件名, 如 abc.jpg 中, {name} 被替换为 abc\n      {id} 被替换为程序自动分配的索引数字\n后缀会根据上方\"输出格式\"自动追加, 无需在模板中填写"
	TipStartIndex  = "输出文件的起始序号, 即文件名模板中 {id} 的起始值"
)
