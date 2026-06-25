// Package cli 提供 PicSizer 的命令行模式入口.
//
// 本包负责解析文档 CLI.md 中声明的全部命令行参数, 将参数映射到 core.Setting,
// 并复用现有的 compress、fileio、server 等模块完成单文件或批量目录压缩.
// 不引入任何 GUI 依赖, 适合在脚本、右键菜单等纯控制台场景使用.
package cli

import (
	"PicSizer/internal/core/setting"
	"flag"
	"fmt"
	"os"
	"path/filepath"
	"strconv"
	"strings"
	"time"

	"PicSizer/internal/compress"
	"PicSizer/internal/core"
	"PicSizer/internal/fileio"
	"PicSizer/internal/server"

	"github.com/schollz/progressbar/v3"
)

// Mode 是否启用命令行模式, 由 -cli / -c 参数设置.
var Mode bool

// 命令行参数变量, 与 CLI.md 文档中的参数表一一对应.
var (
	inputPath    string
	outputPath   string
	threads      int
	showProgress bool
	compressType string
	// quality 画质等级字符串: best/clear/normal/poor 或 最佳/清晰/一般/较差
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
)

func init() {
	// 1. 运行模式与全局控制
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

	// 2. 压缩模式设置
	flag.StringVar(&compressType, "comp-type", "quality", "压缩模式: quality 或 size")
	flag.StringVar(&compressType, "ct", "quality", "压缩模式: quality 或 size")
	// 画质等级: best/clear/normal/poor 或 最佳/清晰/一般/较差, 默认清晰 (80)
	flag.StringVar(&quality, "quality", "clear", "画质等级: best/clear/normal/poor (最佳/清晰/一般/较差), 默认 clear")
	flag.StringVar(&quality, "q", "clear", "画质等级: best/clear/normal/poor (最佳/清晰/一般/较差), 默认 clear")
	flag.StringVar(&limitStr, "limit", "200kb", "限制文件大小, 例如 200kb、2mb、500")
	flag.StringVar(&limitStr, "l", "200kb", "限制文件大小, 例如 200kb、2mb、500")
	flag.BoolVar(&acceptExceed, "exceed", false, "是否接受超出限制的输出")
	flag.BoolVar(&acceptExceed, "e", false, "是否接受超出限制的输出")

	// 3. 输出与命名模板
	flag.StringVar(&outType, "out-type", "dir", "输出方式: dir / cover / struct")
	flag.StringVar(&outType, "ot", "dir", "输出方式: dir / cover / struct")
	flag.StringVar(&format, "format", "jpeg", "输出图像格式: jpeg / jpg / png / webp / origin")
	flag.StringVar(&format, "f", "jpeg", "输出图像格式: jpeg / jpg / png / webp / origin")
	flag.StringVar(&templateStr, "template", "{id}", "文件名模板 (不含后缀), 支持 {id} / {name}")
	flag.StringVar(&templateStr, "tpl", "{id}", "文件名模板 (不含后缀), 支持 {id} / {name}")
	flag.IntVar(&startIdx, "start-idx", 1, "命名模板中 {id} 的起始序号")
	flag.IntVar(&startIdx, "si", 1, "命名模板中 {id} 的起始序号")

	// 4. 高级图像预处理
	flag.StringVar(&alpha, "alpha", "keep", "透明通道处理: keep / smart / remove")
	flag.StringVar(&alpha, "a", "keep", "透明通道处理: keep / smart / remove")
	flag.StringVar(&scale, "scale", "none", "图像缩放模式: none / stretch / cover / contain / crop / lock")
	flag.StringVar(&scale, "s", "none", "图像缩放模式: none / stretch / cover / contain / crop / lock")
	flag.IntVar(&width, "width", 0, "缩放目标宽度 (像素), 0 表示不限制")
	flag.IntVar(&width, "w", 0, "缩放目标宽度 (像素), 0 表示不限制")
	flag.IntVar(&height, "height", 0, "缩放目标高度 (像素), 0 表示不限制")
	flag.IntVar(&height, "hg", 0, "缩放目标高度 (像素), 0 表示不限制")
}

// Run 执行命令行模式入口.
//
// 流程:
//  1. 校验输入路径
//  2. 将 CLI 参数转换为 core.Setting
//  3. 判断输入是文件还是目录, 分别调用单文件或批量压缩
func Run() {
	flag.Parse()

	// 初始化配置
	setting.InitSetting()

	if inputPath == "" {
		fmt.Println("错误: 请指定输入路径 (-i / --input)")
		fmt.Println("用法: PicSizer.exe -c -i <输入路径> [选项]")
		flag.PrintDefaults()
		os.Exit(1)
	}

	if err := applySettings(); err != nil {
		fmt.Printf("错误: %v\n", err)
		os.Exit(1)
	}

	info, err := fileio.GetFileInfo(inputPath)
	if err != nil {
		fmt.Printf("错误: 无法访问输入路径: %v\n", err)
		os.Exit(1)
	}

	if info.IsDir() {
		compressDirectory(inputPath, outputPath)
	} else {
		compressFile(inputPath, outputPath)
	}
}

// applySettings 将解析后的 CLI 参数写入 core.CurrentSetting.
//
// 包含参数合法性校验, 例如质量值范围、缩放锁定单边条件等.
// 校验失败时返回错误, 由 Run 决定退出程序.
func applySettings() error {
	set := setting.GetSetting()

	// 压缩模式
	switch strings.ToLower(compressType) {
	case "quality":
		set.CompressType = setting.CompressQuality
	case "size":
		set.CompressType = setting.CompressFileSize
	default:
		return fmt.Errorf("不支持的压缩模式: %s (可选: quality / size)", compressType)
	}

	// 画质等级
	qualityLevel, err := setting.ParseQualityLevel(quality)
	if err != nil {
		return err
	}
	set.Quality = qualityLevel

	// 限制大小 (仅在按大小压缩时生效, 但仍会解析以便报错友好)
	if set.CompressType == setting.CompressFileSize {
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
		set.OutputType = setting.OutputDirection
	case "cover":
		set.OutputType = setting.OutputCoverOrigin
	case "struct":
		set.OutputType = setting.OutputStructure
	default:
		return fmt.Errorf("不支持的输出方式: %s (可选: dir / cover / struct)", outType)
	}

	// 输出格式
	switch strings.ToLower(format) {
	case "jpeg", "jpg":
		set.Extension = setting.ExtJPEG
	case "png":
		set.Extension = setting.ExtPNG
	case "webp":
		set.Extension = setting.ExtWebP
	case "origin":
		set.Extension = setting.ExtOrigin
	default:
		return fmt.Errorf("不支持的输出格式: %s (可选: jpeg / jpg / png / webp / origin)", format)
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
		set.AlphaHandle = setting.AlphaKeep
	case "smart":
		set.AlphaHandle = setting.AlphaSmartRemove
	case "remove":
		set.AlphaHandle = setting.AlphaRemove
	default:
		return fmt.Errorf("不支持的透明通道处理方式: %s (可选: keep / smart / remove)", alpha)
	}

	// 缩放模式
	switch strings.ToLower(scale) {
	case "none":
		set.Scale = setting.ScaleNone
	case "stretch":
		set.Scale = setting.ScaleStretch
	case "cover":
		set.Scale = setting.ScaleFitOutside
	case "contain":
		set.Scale = setting.ScaleFitInside
	case "crop":
		set.Scale = setting.ScaleFitOutsideCrop
	case "lock":
		set.Scale = setting.ScaleLockSide
	default:
		return fmt.Errorf("不支持的缩放模式: %s (可选: none / stretch / cover / contain / crop / lock)", scale)
	}

	// 缩放目标尺寸
	set.ScaleWidth = width
	set.ScaleHeight = height

	// 等比锁定单边校验
	if set.Scale == setting.ScaleLockSide {
		w0 := set.ScaleWidth == 0
		h0 := set.ScaleHeight == 0
		if (w0 && h0) || (!w0 && !h0) {
			return fmt.Errorf(core.ErrScaleLockSideInvalid)
		}
	}

	// 文件名模板重名保护 (非覆盖模式下给出警告, 但不阻止执行)
	if set.OutputType != setting.OutputCoverOrigin {
		hasID := strings.Contains(set.OutputFilename, "{id}")
		hasName := strings.Contains(set.OutputFilename, "{name}")
		if !hasID && !hasName {
			fmt.Println("警告: 文件名模板未包含 {id} 或 {name}, 多张图片可能因重名而被覆盖")
		}
	}

	setting.UpdateSetting(set)
	return nil
}

// parseLimit 解析限制大小字符串.
//
// 支持以下写法:
//   - "200kb" / "200KB" / "2mb" / "2MB"
//   - "500" (默认单位为 KB)
//
// 仅作为按大小压缩模式下的合法大小解析, 不再支持额外的单位参数.
func parseLimit(limitStr string) (int64, setting.SizeUnit, error) {
	s := strings.TrimSpace(limitStr)
	s = strings.ToLower(s)

	numeric := s
	var parsedUnit setting.SizeUnit = setting.UnitKB

	// 从 limit 字符串尾部提取单位
	if strings.HasSuffix(s, "mb") {
		parsedUnit = setting.UnitMB
		numeric = strings.TrimSuffix(s, "mb")
	} else if strings.HasSuffix(s, "kb") {
		parsedUnit = setting.UnitKB
		numeric = strings.TrimSuffix(s, "kb")
	}

	value, err := strconv.ParseInt(strings.TrimSpace(numeric), 10, 64)
	if err != nil {
		return 0, 0, fmt.Errorf("无法解析限制大小: %s", limitStr)
	}
	if value <= 0 {
		return 0, 0, fmt.Errorf("限制大小必须大于 0: %d", value)
	}

	return value, parsedUnit, nil
}

// compressFile 压缩单个文件.
//
// 根据输出方式构造最终输出路径, 然后调用 compress.Compress.
func compressFile(input, output string) {
	finalOutput := buildSingleOutputPath(input, output)

	fmt.Printf("压缩: %s -> %s\n", input, finalOutput)

	result := compress.Compress(input, finalOutput)
	switch result.CompressResult {
	case core.ResultOk:
		outInfo, err := fileio.GetFileInfo(finalOutput)
		if err == nil {
			fmt.Printf("成功! 输出大小: %s\n", core.FormatFileSize(outInfo.Size()))
		} else {
			fmt.Println("成功!")
		}
	case core.ResultOutOfLimit:
		fmt.Printf("失败: %s\n", result.Message)
		os.Exit(1)
	default:
		fmt.Printf("失败: %s\n", result.Message)
		os.Exit(1)
	}
}

// buildSingleOutputPath 根据全局输出方式为单文件构造最终输出路径.
//
//   - cover  : 覆盖源文件, 按输出格式修改扩展名
//   - dir    : 输出到统一目录, 使用模板生成文件名
//   - struct : 保留源文件所在目录结构, 使用模板生成文件名
func buildSingleOutputPath(input, output string) string {
	set := setting.GetSetting()

	switch set.OutputType {
	case setting.OutputCoverOrigin:
		if set.Extension != setting.ExtOrigin {
			ext := setting.GetExtensionString(set.Extension)
			return input[:len(input)-len(filepath.Ext(input))] + ext
		}
		return input

	case setting.OutputDirection:
		if output == "" {
			fmt.Println("错误: 输出到目录模式 (-ot dir) 必须指定输出路径 (-o / --output)")
			os.Exit(1)
		}
		core.OutputDirPath = output
		return fileio.GetOutputPath(input, set.StartIndex)

	case setting.OutputStructure:
		if output == "" {
			fmt.Println("错误: 保留目录结构模式 (-ot struct) 必须指定输出路径 (-o / --output)")
			os.Exit(1)
		}
		core.OutputDirPath = output
		core.PublicDirPath = filepath.Dir(input)
		return fileio.GetOutputPath(input, set.StartIndex)
	}

	return ""
}

// compressDirectory 批量压缩目录.
//
// 收集目录下所有图片文件, 创建线程池并发压缩.
// 根据 -p / --progress 参数决定是否在控制台显示进度条, 压缩结束后在控制台打印汇总结果.
func compressDirectory(inputDir, outputDir string) {
	files, err := fileio.CollectImageFiles(inputDir)
	if err != nil {
		fmt.Printf("错误: 无法读取目录: %v\n", err)
		os.Exit(1)
	}

	if len(files) == 0 {
		fmt.Println("没有找到图片文件")
		os.Exit(1)
	}

	fmt.Printf("找到 %d 个图片文件\n", len(files))

	set := setting.GetSetting()

	// 设置输出相关全局变量, 供 fileio.GetOutputPath 使用
	switch set.OutputType {
	case setting.OutputCoverOrigin:
		// 覆盖源文件, 无需输出目录
	case setting.OutputDirection, setting.OutputStructure:
		if outputDir == "" {
			fmt.Println("错误: 当前输出方式必须指定输出路径 (-o / --output)")
			os.Exit(1)
		}
		core.OutputDirPath = outputDir
		if set.OutputType == setting.OutputStructure {
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
			State:    setting.StateWaiting,
		})
	}

	// 创建进度条 (仅在 showProgress 为 true 时使用)
	// Description 包含: 待压缩数量, 已完成, 剩余, 已用时间, 预计剩余时间
	// 关闭 progressbar 默认的 ETA / Elapsed 显示, 改为在描述中由我们统一输出
	var bar *progressbar.ProgressBar
	startTime := time.Now()
	if showProgress {
		bar = progressbar.NewOptions(len(items),
			progressbar.OptionSetDescription(fmt.Sprintf("压缩中 (共 %d 张)", len(items))),
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
				"压缩中 | 总数 %d | 已完成 %d | 剩余 %d | 用时 %s | 预计剩余 %s",
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
			fmt.Println("========== 压缩结果 ==========")
			fmt.Printf("总计:      %d 张\n", total)
			fmt.Printf("成功:      %d 张\n", success)
			fmt.Printf("失败/超限: %d 张\n", errCount)
			fmt.Printf("总用时:    %s\n", formatDuration(time.Since(startTime)))
			fmt.Println("==============================")
		},
	)

	pool.Start()

	// 阻塞等待所有 worker 完成, 由 Wait 内部统一触发 onComplete 回调
	// (进度条结束 + 控制台打印汇总结果).
	// Start 与 Wait 分离的好处: 调用方可在 Start 与 Wait 之间插入其他逻辑
	// (例如: 注册信号处理、轮询状态、定时汇报), 同时也避免 Start 内嵌 wg.Wait()
	// 导致调用方无法在该线程中做其它事.
	pool.Wait()
	errCount := getErrorCount(pool)

	if errCount == 0 {
		fmt.Println("Success")
	} else {
		fmt.Println("Failed")
	}
}

// formatDuration 格式化时间为简短的 hh:mm:ss / mm:ss 字符串.
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

// getErrorCount 从线程池获取当前错误数量.
//
// 用于批量压缩结束后判断是否以非零状态码退出.
func getErrorCount(pool *server.ThreadPool) int {
	_, errCount, _ := pool.GetStats()
	return errCount
}
