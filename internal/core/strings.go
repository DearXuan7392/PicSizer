package core

// AppName 和 AppVersion 分别表示应用程序的名称和版本号。
var (
	AppName    = "PicSizer"
	AppVersion = "v6.0.0"

	TitleMain     = AppName + " - 图片压缩工具"
	TitleSetting  = "压缩设置"
	TitleAbout    = "关于 " + AppName
	TitleProgress = "压缩进度"

	AboutTitle   = AppName
	AboutVersion = "版本: " + AppVersion
	AboutDesc    = "一款使用 Go 语言编写的高效图片压缩工具\n" +
		"支持 JPEG、PNG、WebP 格式\n" +
		"支持按图像质量或压缩后大小进行压缩"
)

// 通用文本
var (
	TextOK      = "确定"
	TextCancel  = "取消"
	TextClose   = "关闭"
	TextSave    = "保存"
	TextBrowse  = "浏览"
	TextStart   = "开始压缩"
	TextSetting = "设置"
)

// 菜单栏
var (
	TextFile      = "文件"
	TextAddFiles  = "添加文件"
	TextAddFolder = "打开文件夹"
	TextExit      = "退出"

	// 编辑
	TextEdit          = "编辑"
	TextRemove        = "移除"
	TextRemoveSelect  = "选中项"
	TextRemoveDone    = "已完成"
	TextRemoveError   = "错误项"
	TextRemoveAll     = "全部项"
	TextSelect        = "选择"
	TextSelectAll     = "全选"
	TextSelectReverse = "反选"

	TextHelp  = "帮助"
	TextAbout = "关于"
)

var (
	// 输出方式
	TextOutputMode   = "输出方式"
	TextCoverOrigin  = "覆盖源文件"
	TextOutputDir    = "输出到目录"
	TextOutputStruct = "保留目录结构"
	TextOutputPath   = "输出目录"

	// 压缩设置
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

	// 输出格式
	TextOutputFormat = "输出格式"
	TextFormatJPEG   = "JPEG (快速)"
	TextFormatPNG    = "PNG (缓慢)"
	TextFormatWebP   = "WebP (缓慢)"
	TextFormatOrigin = "保持原格式"
	TextFilenameTpl  = "文件名模板"
	TextStartIndex   = "起始下标"

	// 状态文本
	StrStateWaiting     = "等待中"
	StrStateCompressing = "压缩中"
	StrStateSuccess     = "成功"
	StrStateOutOfLimit  = "超出限制"
	StrStateError       = "错误"

	// 进度窗口文本
	StrWaitingForFinish = "正在等待当前任务结束..."

	// 列名
	ColFilename = "文件名"
	ColOrigSize = "原大小"
	ColNewSize  = "新大小"
	ColStatus   = "状态"

	// 重复任务提示
	WarnCompressRunning  = "已有压缩任务正在进行，请等待完成或取消后再开始新的任务"
	WarnOverwriteDoneFmt = "列表中有 %d 张已完成 (成功) 的图片，重新压缩将覆盖之前的输出文件。\n可通过 \"菜单栏\" - \"" + TextEdit + "\" - \"" + TextRemove + "\" - \"" + TextRemoveSelect + "\" 来移除已完成的任务。\n\n是否仍要继续?"

	// 错误信息
	ErrCompressError    = "压缩过程中发生错误"
	ErrOutOfLimit       = "无法压缩到指定大小"
	ErrNotImplemented   = "未实现的功能"
	ErrArgOutOfRange    = "参数超出范围"
	ErrFileNotFound     = "文件不存在"
	ErrBrokenFile       = "文件损坏"
	ErrNoPictures       = "没有待压缩的图片"
	ErrOutputDirInvalid = "输出目录有误"
	ErrUnsupportedExt   = "不支持的格式"
	ErrNoCommonPrefix   = "无法找到共同前缀，请重新选择图片，或将图片分多次压缩\n(常见原因: 图片分布在多个不同的盘符下)"

	// 对话框文本
	MsgClearList      = "是否清空列表，包括未完成的项目?"
	MsgCompressFinish = "压缩已结束"
	MsgCompressResult = "总共: %d 张\n压缩完成: %d 张\n未完成: %d 张"

	// 设置保存时的校验提示
	ErrScaleLockSideInvalid = "缩放方式为\"等比锁定单边\"时，宽和高必须有一项为 0，另一项不为 0"
	WarnFilenameTplMissing  = "文件名模板未包含 {id} 或 {name}，多张图片可能因重名而被覆盖。\n是否仍要保存当前设置?"

	// 设置界面提示文本
	TipCompressMode = "按质量压缩：\n      指定输出画质，速度较快\n按大小压缩：\n      限制压缩后文件大小，由程序自动计算最佳画质，但更耗时"
	TipQuality      = "按质量压缩模式下，设置输出图片的画质等级，共 4 档：\n      最佳 (95) / 清晰 (80) / 一般 (40) / 较差 (10)\n等级越高画质越好，但文件体积越大"
	TipLimitSize    = "按大小压缩模式下，设置输出文件的最大体积，最终大小不会超过这个值"
	TipAcceptExceed = "勾选后，对于无法压缩到指定大小以内的图片，仍然输出所能达到的体积最小的文件\n若取消勾选，则无法压缩到指定大小以内的图片会直接报错"
	TipOutputType   = "输出到目录：\n      将图片全部输出到单个文件夹内\n覆盖源文件：\n      直接覆盖源文件，需做好备份\n保留目录结构：\n      将文件按原始的相对位置输出到新的文件夹中"
	TipExtension    = "当选择原格式时，若图片实际格式与后缀名不同，会以后缀名为准，自动调整编码方式"
	TipFilenameTpl  = "文件名模板 (不含后缀)，替换原则如下:\n      {name} 被替换为原始文件名，如 abc.jpg 中，{name} 被替换为 abc\n      {id} 被替换为程序自动分配的索引数字\n后缀会根据上方\"输出格式\"自动追加，无需在模板中填写"
	TipStartIndex   = "输出文件的起始序号，即文件名模板中 {id} 的起始值"
	TipMaxThreads   = "设置并发压缩的线程数，过多时会导致程序卡顿"
	TipThreadSlider = "拖拽滑块快速调整线程数"
	TipTopMost      = "勾选后窗口始终置顶"
	TipScale        = "缩放方式：\n\n      无操作：按原图尺寸输出\n\n      强制拉伸：破坏原图比例，强制拉伸到指定大小\n\n      等比外接：保持原图比例，缩放至指定尺寸的最小外接矩形。即宽和高都 大于等于 指定尺寸的最小矩形\n\n      等比内接：保持原图比例，缩放至指定尺寸的最大内接矩形。即宽和高都 小于等于 指定尺寸的最大矩形\n\n      等比外接+裁剪：保持原图比例，缩放至指定尺寸的最小外接矩形后，居中裁剪\n\n      等比锁定单边：保持原图比例，将其中一条边缩放至指定大小，另一条边自适应调整。输入 \"0\" 表示自动调整此边长"
	TipScaleWidth   = "目标宽度 (像素)\n\"0\" 表示自动调整，必须与缩放方式配合使用"
	TipScaleHeight  = "目标高度 (像素)\n\"0\" 表示自动调整，必须与缩放方式配合使用"
	TipAlphaHandle  = "透明通道处理方式：\n      保留：输出图片保留透明度通道\n      智能移除：当存在透明像素时保留透明度通道，仅在图片全部为不透明像素时移除透明度通道\n      全部移除：使用白色底色叠加，并移除透明度通道。半透明的图片会变为白底不透明图片"

	// 分页设置界面
	TextGeneralSetting    = "常规设置"
	TextAdvancedSetting   = "高级设置"
	TextPreprocessSetting = "图像预处理"

	// 图像预处理
	TextAlphaHandle      = "透明通道处理"
	AlphaKeepText        = "保留"
	AlphaSmartRemoveText = "智能移除"
	AlphaRemoveText      = "全部移除"

	// 缩放方式
	TextScale         = "缩放方式"
	TextScaleWidth    = "宽"
	TextScaleHeight   = "高"
	ScaleNoneText     = "无操作"
	ScaleStretchText  = "强制拉伸"
	ScaleFitOutText   = "等比外接"
	ScaleFitInText    = "等比内接"
	ScaleFitOutCrop   = "等比外接+裁剪"
	ScaleLockSideText = "等比锁定单边"

	// 高级设置 (各格式精细化画质)
	TextAdvJPEGSettings   = "JPEG 设置"
	TextAdvPNGSettings    = "PNG 设置"
	TextAdvWebPSettings   = "WebP 设置"
	TextAdvFineQuality    = "精细化画质"
	TipAdvFineQuality     = "为 0 时不启用。如果非 0，则按画质压缩时，覆盖 jpeg 图片的画质。"
	TipAdvFineQualityWebP = "为 0 时不启用。如果非 0，则按画质压缩时，覆盖 webp 图片的画质。"

	// PNG 高级设置
	TextPngKeepIndexedAlpha = "为索引格式预留透明像素"
	TipPngKeepIndexedAlpha  = "PNG 在量化压缩时会将图像转换为索引色 (调色板) 格式。\n勾选后, 在调色板中为透明像素预留位置, 可避免透明背景变色或产生毛边;\n取消勾选则不再额外保留透明位, 可能获得更小的文件体积, 但透明像素可能被替换为其他颜色。"
)
