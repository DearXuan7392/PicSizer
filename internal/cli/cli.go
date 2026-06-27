// Package cli 提供 PicSizer 的命令行模式入口.
// 负责解析命令行参数、映射到 core.Setting 并复用现有模块完成单文件或批量目录压缩.
package cli

import (
	"PicSizer/internal/core/settingLoader"
	"flag"
	"fmt"
	"os"
	"path/filepath"
	"strconv"
	"strings"
	"time"

	"PicSizer/internal/compress"
	"PicSizer/internal/core"
	strs "PicSizer/internal/core/strings"
	"PicSizer/internal/fileio"
	"PicSizer/internal/server"

	"github.com/schollz/progressbar/v3"
)

// Mode 表示是否启用命令行模式, 由 -cli / -c 参数设置.
var Mode bool

var (
	inputPath    string
	outputPath   string
	threads      int
	showProgress bool
	compressType string
	quality      string
	limitStr     string
	acceptExceed bool
	outType      string
	format       string
	templateStr  string
	startIdx     int
	alpha        string
	scale        string
	width        int
	height       int

	// 高级设置
	jpegQuality         int
	webpQuality         int
	pngPaletteAlgo      string
	pngKeepIndexedAlpha bool
	pngEnableDithering  bool
)

func init() {
	// 运行模式与全局控制
	flag.BoolVar(&Mode, "cli", false, "启用命令行模式")
	flag.BoolVar(&Mode, "c", false, "启用命令行模式")
	flag.StringVar(&inputPath, "input", "", "输入文件或目录路径")
	flag.StringVar(&inputPath, "i", "", "输入文件或目录路径")
	flag.StringVar(&outputPath, "output", "", "输出文件或目录路径")
	flag.StringVar(&outputPath, "o", "", "输出文件或目录路径")
	flag.IntVar(&threads, "threads", 2, "最大并发线程数")
	flag.IntVar(&threads, "t", 2, "最大并发线程数")
	flag.BoolVar(&showProgress, "progress", true, "是否在控制台显示进度条")
	flag.BoolVar(&showProgress, "p", true, "是否在控制台显示进度条")

	// 压缩模式设置
	flag.StringVar(&compressType, "comp-type", "quality", "压缩模式: quality 或 size")
	flag.StringVar(&compressType, "ct", "quality", "压缩模式: quality 或 size")
	// 画质等级: best/clear/normal/poor 默认清晰 (80)
	flag.StringVar(&quality, "quality", "clear", "画质等级: best/clear/normal/poor (最佳/清晰/一般/较差), 默认 clear")
	flag.StringVar(&quality, "q", "clear", "画质等级: best/clear/normal/poor (最佳/清晰/一般/较差), 默认 clear")
	flag.StringVar(&limitStr, "limit", "200kb", "限制文件大小, 例如 200kb、2mb、500")
	flag.StringVar(&limitStr, "l", "200kb", "限制文件大小, 例如 200kb、2mb、500")
	flag.BoolVar(&acceptExceed, "exceed", false, "是否接受超出限制的输出")
	flag.BoolVar(&acceptExceed, "e", false, "是否接受超出限制的输出")

	// 输出与命名模板
	flag.StringVar(&outType, "out-type", "dir", "输出方式: dir / cover / struct")
	flag.StringVar(&outType, "ot", "dir", "输出方式: dir / cover / struct")
	flag.StringVar(&format, "format", "jpeg", "输出图像格式: jpeg / jpg / png / webp / origin")
	flag.StringVar(&format, "f", "jpeg", "输出图像格式: jpeg / jpg / png / webp / origin")
	flag.StringVar(&templateStr, "template", "{id}", "文件名模板 (不含后缀), 支持 {id} / {name}")
	flag.StringVar(&templateStr, "tpl", "{id}", "文件名模板 (不含后缀), 支持 {id} / {name}")
	flag.IntVar(&startIdx, "start-idx", 1, "命名模板中 {id} 的起始序号")
	flag.IntVar(&startIdx, "si", 1, "命名模板中 {id} 的起始序号")

	// 高级图像预处理
	flag.StringVar(&alpha, "alpha", "keep", "透明通道处理: keep / smart / remove")
	flag.StringVar(&alpha, "a", "keep", "透明通道处理: keep / smart / remove")
	flag.StringVar(&scale, "scale", "none", "图像缩放模式: none / stretch / cover / contain / crop / lock")
	flag.StringVar(&scale, "s", "none", "图像缩放模式: none / stretch / cover / contain / crop / lock")
	flag.IntVar(&width, "width", 0, "缩放目标宽度 (像素), 0 表示不限制")
	flag.IntVar(&width, "w", 0, "缩放目标宽度 (像素), 0 表示不限制")
	flag.IntVar(&height, "height", 0, "缩放目标高度 (像素), 0 表示不限制")
	flag.IntVar(&height, "hg", 0, "缩放目标高度 (像素), 0 表示不限制")

	// 高级设置
	flag.IntVar(&jpegQuality, "jpeg-quality", 0, "JPEG 精细化画质 (0-100), 0 表示按全局画质等级处理")
	flag.IntVar(&jpegQuality, "jq", 0, "JPEG 精细化画质 (0-100), 0 表示按全局画质等级处理")
	flag.IntVar(&webpQuality, "webp-quality", 0, "WebP 精细化画质 (0-100), 0 表示按全局画质等级处理")
	flag.IntVar(&webpQuality, "wq", 0, "WebP 精细化画质 (0-100), 0 表示按全局画质等级处理")
	flag.StringVar(&pngPaletteAlgo, "png-palette", "mediancut", "PNG 调色盘算法: mediancut / kmeans")
	flag.StringVar(&pngPaletteAlgo, "pp", "mediancut", "PNG 调色盘算法: mediancut / kmeans")
	flag.BoolVar(&pngKeepIndexedAlpha, "png-keep-alpha", true, "PNG 索引格式压缩时是否预留透明像素")
	flag.BoolVar(&pngKeepIndexedAlpha, "pka", true, "PNG 索引格式压缩时是否预留透明像素")
	flag.BoolVar(&pngEnableDithering, "png-dither", true, "PNG 有损量化时是否启用抖动算法")
	flag.BoolVar(&pngEnableDithering, "pd", true, "PNG 有损量化时是否启用抖动算法")
}

// Run 执行命令行模式入口.
// 解析命令行参数、校验输入路径、将参数映射到 Setting 后执行单文件或批量压缩.
func Run() {
	flag.Parse()

	settingLoader.InitSetting()

	if inputPath == "" {
		fmt.Println(strs.CLIErrNoInputPath)
		fmt.Println(strs.CLIUsage)
		flag.PrintDefaults()
		os.Exit(1)
	}

	if err := applySettings(); err != nil {
		fmt.Printf("错误: %v\n", err)
		os.Exit(1)
	}

	info, err := fileio.GetFileInfo(inputPath)
	if err != nil {
		fmt.Printf(strs.CLIErrAccessInput+"\n", err)
		os.Exit(1)
	}

	if info.IsDir() {
		compressDirectory(inputPath, outputPath)
	} else {
		compressFile(inputPath, outputPath)
	}
}

// applySettings 将解析后的 CLI 参数写入全局配置, 包含参数合法性校验.
// 校验失败时返回错误, 由 Run 决定是否退出程序.
func applySettings() error {
	set := settingLoader.GetSetting()

	// 压缩模式
	switch strings.ToLower(compressType) {
	case "quality":
		set.CompressType = settingLoader.CompressQuality
	case "size":
		set.CompressType = settingLoader.CompressFileSize
	default:
		return fmt.Errorf(strs.CLIErrCompressType, compressType)
	}

	// 画质等级
	qualityLevel, err := settingLoader.ParseQualityLevel(quality)
	if err != nil {
		return err
	}
	set.Quality = qualityLevel

	// 限制大小 (仅在按大小压缩时生效, 但仍会解析以便报错友好)
	if set.CompressType == settingLoader.CompressFileSize {
		limit, unit, err := parseLimit(limitStr)
		if err != nil {
			return err
		}
		set.LimitSize = limit
		set.SizeUnit = unit
	}
	set.AcceptExceed = acceptExceed

	// 输出方式
	switch strings.ToLower(outType) {
	case "dir":
		set.OutputType = settingLoader.OutputDirection
	case "cover":
		set.OutputType = settingLoader.OutputCoverOrigin
	case "struct":
		set.OutputType = settingLoader.OutputStructure
	default:
		return fmt.Errorf(strs.CLIErrOutputType, outType)
	}

	// 输出格式
	switch strings.ToLower(format) {
	case "jpeg", "jpg":
		set.Extension = settingLoader.ExtJPEG
	case "png":
		set.Extension = settingLoader.ExtPNG
	case "webp":
		set.Extension = settingLoader.ExtWebP
	case "origin":
		set.Extension = settingLoader.ExtOrigin
	default:
		return fmt.Errorf(strs.CLIErrFormat, format)
	}

	// 文件名模板与起始序号
	set.OutputFilename = templateStr
	set.StartIndex = startIdx

	// 线程数
	if threads < 1 {
		threads = 1
	}
	set.MaxThreads = threads

	// 透明通道处理
	switch strings.ToLower(alpha) {
	case "keep":
		set.AlphaHandle = settingLoader.AlphaKeep
	case "smart":
		set.AlphaHandle = settingLoader.AlphaSmartRemove
	case "remove":
		set.AlphaHandle = settingLoader.AlphaRemove
	default:
		return fmt.Errorf(strs.CLIErrAlpha, alpha)
	}

	// 缩放模式
	switch strings.ToLower(scale) {
	case "none":
		set.Scale = settingLoader.ScaleNone
	case "stretch":
		set.Scale = settingLoader.ScaleStretch
	case "cover":
		set.Scale = settingLoader.ScaleFitOutside
	case "contain":
		set.Scale = settingLoader.ScaleFitInside
	case "crop":
		set.Scale = settingLoader.ScaleFitOutsideCrop
	case "lock":
		set.Scale = settingLoader.ScaleLockSide
	default:
		return fmt.Errorf(strs.CLIErrScale, scale)
	}

	// 缩放目标尺寸
	set.ScaleWidth = width
	set.ScaleHeight = height

	// 高级设置: JPEG 精细化画质
	if jpegQuality < 0 || jpegQuality > 100 {
		return fmt.Errorf(strs.CLIErrJpegQuality, jpegQuality)
	}
	set.AdvancedJpegQuality = jpegQuality

	// 高级设置: WebP 精细化画质
	if webpQuality < 0 || webpQuality > 100 {
		return fmt.Errorf(strs.CLIErrWebpQuality, webpQuality)
	}
	set.AdvancedWebPQuality = webpQuality

	// 高级设置: PNG 调色盘算法
	switch strings.ToLower(pngPaletteAlgo) {
	case "mediancut":
		set.AdvancedPngPaletteAlgo = settingLoader.PaletteMedianCut
	case "kmeans":
		set.AdvancedPngPaletteAlgo = settingLoader.PaletteKMeans
	default:
		return fmt.Errorf(strs.CLIErrPngPalette, pngPaletteAlgo)
	}

	// 高级设置: PNG 索引透明像素预留
	set.AdvancedPngKeepIndexedAlpha = pngKeepIndexedAlpha

	// 高级设置: PNG 抖动算法
	set.AdvancedPngEnableDithering = pngEnableDithering

	// 使用 Setting 自带的校验方法检查错误
	if errors := set.CheckErrors(); len(errors) > 0 {
		return fmt.Errorf("%s", strings.Join(errors, "; "))
	}

	// 使用 Setting 自带的校验方法检查警告 (非覆盖模式下给出警告, 但不阻止执行)
	if warnings := set.CheckWarnings(); len(warnings) > 0 {
		for _, w := range warnings {
			fmt.Println(strs.CLIWarnPrefix + w)
		}
	}

	settingLoader.UpdateSetting(set)
	return nil
}

// parseLimit 解析限制大小字符串, 支持 "200kb"、"2mb" 或纯数字 (默认 KB) 格式.
func parseLimit(limitStr string) (int64, settingLoader.SizeUnit, error) {
	s := strings.TrimSpace(limitStr)
	s = strings.ToLower(s)

	numeric := s
	var parsedUnit settingLoader.SizeUnit = settingLoader.UnitKB

	if strings.HasSuffix(s, "mb") {
		parsedUnit = settingLoader.UnitMB
		numeric = strings.TrimSuffix(s, "mb")
	} else if strings.HasSuffix(s, "kb") {
		parsedUnit = settingLoader.UnitKB
		numeric = strings.TrimSuffix(s, "kb")
	}

	value, err := strconv.ParseInt(strings.TrimSpace(numeric), 10, 64)
	if err != nil {
		return 0, 0, fmt.Errorf(strs.CLIErrParseLimit, limitStr)
	}
	if value <= 0 {
		return 0, 0, fmt.Errorf(strs.CLIErrLimitMustPositive, value)
	}

	return value, parsedUnit, nil
}

// compressFile 压缩单个文件, 根据输出方式构造输出路径后调用 compress.Compress.
// 压缩完成后在控制台输出结果信息.
func compressFile(input, output string) {
	finalOutput := buildSingleOutputPath(input, output)

	fmt.Printf(strs.CLICompressFromTo+"\n", input, finalOutput)

	result := compress.Compress(input, finalOutput)
	switch result.CompressResult {
	case core.ResultOk:
		outInfo, err := fileio.GetFileInfo(finalOutput)
		if err == nil {
			fmt.Printf(strs.CLICompressSuccessFmt+"\n", core.FormatFileSize(outInfo.Size()))
		} else {
			fmt.Println(strs.CLICompressSuccess)
		}
	case core.ResultOutOfLimit:
		fmt.Printf(strs.CLICompressFailedFmt+"\n", result.Message)
		os.Exit(1)
	default:
		fmt.Printf(strs.CLICompressFailedFmt+"\n", result.Message)
		os.Exit(1)
	}
}

// buildSingleOutputPath 根据输出方式为单文件构造最终输出路径.
// cover 模式覆盖源文件并修改扩展名；dir 和 struct 模式使用模板生成文件名.
func buildSingleOutputPath(input, output string) string {
	set := settingLoader.GetSetting()

	switch set.OutputType {
	case settingLoader.OutputCoverOrigin:
		if set.Extension != settingLoader.ExtOrigin {
			ext := settingLoader.GetExtensionString(set.Extension)
			return input[:len(input)-len(filepath.Ext(input))] + ext
		}
		return input

	case settingLoader.OutputDirection:
		if output == "" {
			fmt.Println(strs.CLIErrDirModeNeedOutput)
			os.Exit(1)
		}
		core.OutputDirPath = output
		return fileio.GetOutputPath(input, set.StartIndex)

	case settingLoader.OutputStructure:
		if output == "" {
			fmt.Println(strs.CLIErrStructModeNeedOutput)
			os.Exit(1)
		}
		core.OutputDirPath = output
		core.PublicDirPath = filepath.Dir(input)
		return fileio.GetOutputPath(input, set.StartIndex)
	}

	return ""
}

// compressDirectory 批量压缩目录中的所有图片文件.
// 使用线程池并发压缩, 通过 progressbar 在控制台显示进度, 压缩结束后打印汇总结果.
func compressDirectory(inputDir, outputDir string) {
	files, err := fileio.CollectImageFiles(inputDir)
	if err != nil {
		fmt.Printf(strs.CLIErrReadDir+"\n", err)
		os.Exit(1)
	}

	if len(files) == 0 {
		fmt.Println(strs.CLIErrNoImageFiles)
		os.Exit(1)
	}

	fmt.Printf(strs.CLIFoundImageFiles+"\n", len(files))

	set := settingLoader.GetSetting()

	// 设置输出相关全局变量, 供 fileio.GetOutputPath 使用
	switch set.OutputType {
	case settingLoader.OutputCoverOrigin:
		// 覆盖源文件, 无需输出目录
	case settingLoader.OutputDirection, settingLoader.OutputStructure:
		if outputDir == "" {
			fmt.Println(strs.CLIErrNeedOutputPath)
			os.Exit(1)
		}
		core.OutputDirPath = outputDir
		if set.OutputType == settingLoader.OutputStructure {
			core.PublicDirPath = inputDir
		}
	}

	// 构造待压缩项目列表
	var items []*core.PicItem
	for _, f := range files {
		info, _ := fileio.GetFileInfo(f)
		items = append(items, &core.PicItem{
			FullPath: f,
			FileName: filepath.Base(f),
			OrigSize: info.Size(),
			State:    settingLoader.StateWaiting,
		})
	}

	// 创建进度条
	var bar *progressbar.ProgressBar
	startTime := time.Now()
	if showProgress {
		bar = progressbar.NewOptions(len(items),
			progressbar.OptionSetDescription(fmt.Sprintf(strs.CLICompressing, len(items))),
			progressbar.OptionSetWidth(40),
			progressbar.OptionShowCount(),
			progressbar.OptionShowIts(),
			progressbar.OptionSetPredictTime(false),
			progressbar.OptionSetElapsedTime(false),
			progressbar.OptionThrottle(200*time.Millisecond),
			progressbar.OptionClearOnFinish(),
		)
	}

	// 创建线程池, CLI 模式使用 progressbar 驱动进度
	pool := server.NewThreadPool(items,
		func(current, errCount, total int) {
			if bar == nil {
				return
			}
			// 已完成数 = 成功数 + 失败数
			done := current + errCount
			// 防止回调早于 bar 创建或已 Clear 后再被调用
			if done > total {
				done = total
			}
			_ = bar.Set64(int64(done))

			// 动态更新描述, 同步展示剩余数量与时间预估
			elapsed := time.Since(startTime)
			remaining := total - done
			avgPerItem := time.Duration(0)
			if done > 0 {
				avgPerItem = elapsed / time.Duration(done)
			}
			eta := avgPerItem * time.Duration(remaining)
			bar.Describe(fmt.Sprintf(
				strs.CLIProgressFormat,
				total, done, remaining, formatDuration(elapsed), formatDuration(eta),
			))
		},
		nil,
		func(success, errCount, total int) {
			if bar != nil {
				_ = bar.Finish()
			}
			// 压缩完成后在控制台打印汇总结果
			fmt.Println()
			fmt.Println(strs.CLIResultHeader)
			fmt.Printf(strs.CLIResultTotal+"\n", total)
			fmt.Printf(strs.CLIResultSuccess+"\n", success)
			fmt.Printf(strs.CLIResultFailed+"\n", errCount)
			fmt.Printf(strs.CLIResultDuration+"\n", formatDuration(time.Since(startTime)))
			fmt.Println(strs.CLIResultFooter)
		},
	)

	pool.Start()
	pool.Wait()
	errCount := getErrorCount(pool)

	if errCount == 0 {
		fmt.Println(strs.CLISuccess)
	} else {
		fmt.Println(strs.CLIFailed)
	}
}

// formatDuration 将时间间隔格式化为 hh:mm:ss 或 mm:ss 字符串.
func formatDuration(d time.Duration) string {
	if d < 0 {
		d = 0
	}
	total := int(d.Round(time.Second).Seconds())
	h := total / 3600
	m := (total % 3600) / 60
	s := total % 60
	if h > 0 {
		return fmt.Sprintf("%02d:%02d:%02d", h, m, s)
	}
	return fmt.Sprintf("%02d:%02d", m, s)
}

// getErrorCount 从线程池获取当前错误数量, 用于判断是否以非零状态码退出.
func getErrorCount(pool *server.ThreadPool) int {
	_, errCount, _ := pool.GetStats()
	return errCount
}
