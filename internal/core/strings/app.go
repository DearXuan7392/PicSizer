package strs

// AppName 和 AppVersion 分别表示应用程序的名称和版本号.
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
