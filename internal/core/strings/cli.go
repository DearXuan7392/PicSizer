package strs

// CLI 命令行模式相关文本
var (
	CLIErrNoInputPath     = "错误: 请指定输入路径 (-i / --input)"
	CLIUsage              = "用法: PicSizer.exe -c -i <输入路径> [选项]"
	CLIErrAccessInput     = "错误: 无法访问输入路径: %v"
	CLIErrNoImageFiles    = "没有找到图片文件"
	CLIFoundImageFiles    = "找到 %d 个图片文件"
	CLIErrNeedOutputPath  = "错误: 当前输出方式必须指定输出路径 (-o / --output)"
	CLICompressing        = "压缩中 (共 %d 张)"
	CLIProgressFormat     = "压缩中 | 总数 %d | 已完成 %d | 剩余 %d | 用时 %s | 预计剩余 %s"
	CLIResultHeader       = "========== 压缩结果 =========="
	CLIResultTotal        = "总计:      %d 张"
	CLIResultSuccess      = "成功:      %d 张"
	CLIResultFailed       = "失败/超限: %d 张"
	CLIResultDuration     = "总用时:    %s"
	CLIResultFooter       = "=============================="
	CLISuccess            = "Success"
	CLIFailed             = "Failed"
	CLICompressSuccess    = "成功!"
	CLICompressSuccessFmt = "成功! 输出大小: %s"
	CLICompressFailedFmt  = "失败: %s"
	CLICompressFromTo     = "压缩: %s -> %s"

	// CLI 参数校验错误
	CLIErrCompressType         = "不支持的压缩模式: %s (可选: quality / size)"
	CLIErrOutputType           = "不支持的输出方式: %s (可选: dir / cover / struct)"
	CLIErrFormat               = "不支持的输出格式: %s (可选: jpeg / jpg / png / webp / origin)"
	CLIErrAlpha                = "不支持的透明通道处理方式: %s (可选: keep / smart / remove)"
	CLIErrScale                = "不支持的缩放模式: %s (可选: none / stretch / cover / contain / crop / lock)"
	CLIErrParseLimit           = "无法解析限制大小: %s"
	CLIErrLimitMustPositive    = "限制大小必须大于 0: %d"
	CLIErrDirModeNeedOutput    = "错误: 输出到目录模式 (-ot dir) 必须指定输出路径 (-o / --output)"
	CLIErrStructModeNeedOutput = "错误: 保留目录结构模式 (-ot struct) 必须指定输出路径 (-o / --output)"
	CLIErrReadDir              = "错误: 无法读取目录: %v"
	CLIErrJpegQuality          = "JPEG 精细化画质必须在 0-100 之间: %d"
	CLIErrWebpQuality          = "WebP 精细化画质必须在 0-100 之间: %d"
	CLIErrPngPalette           = "不支持的 PNG 调色盘算法: %s (可选: mediancut / kmeans)"

	// CLI 文件名模板警告
	CLIWarnFilenameTplMissing = "警告: 文件名模板未包含 {id} 或 {name}, 多张图片可能因重名而被覆盖"

	// CLI 警告前缀
	CLIWarnPrefix = "警告: "
)
