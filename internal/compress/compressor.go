package compress

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/log"
	"image"
	"os"
	"path/filepath"
	"time"

	"PicSizer/internal/core"
	strs "PicSizer/internal/core/strings"
	"PicSizer/internal/fileio"
	"PicSizer/internal/fileio/codec"
	"PicSizer/internal/preprocess"
)

// Compressor 各输出格式需实现的压缩器接口.
type Compressor interface {
	CompressByQuality(quality settingLoader.QualityLevel) *core.PicResult
	CompressByFileSize(limitBytes int64) *core.PicResult
}

var (
	logger = log.NewLogger("compress")
)

// Compress 统一压缩入口, 按输出扩展名选择压缩器.
// 流程: 加载图片 -> 预处理 -> 执行压缩.
func Compress(inputPath, outputPath string) *core.PicResult {
	start := time.Now()
	logger.Debug("compress start, input: %s, output: %s", inputPath, outputPath)

	imgData, err := codec.LoadImage(inputPath)
	if err != nil {
		logger.Error("load image failed, input: %s, err: %v", inputPath, err)
		return core.GetError(err.Error())
	}

	imgData = preprocess.Process(imgData)
	logger.Debug("preprocess done, input: %s, bounds: %v, type: %T", inputPath, imgData.Bounds(), imgData)

	ext := fileio.GetExtension(outputPath)
	var compressor Compressor

	switch ext {
	case ".jpg", ".jpeg":
		compressor = NewJPEGCompressor(imgData, outputPath)
	case ".png":
		compressor = NewPNGCompressor(imgData, outputPath)
	case ".webp":
		compressor = NewWebPCompressor(imgData, outputPath)
	case ".bmp":
		compressor = NewBMPCompressor(imgData, outputPath)
	default:
		logger.Error("unsupported output format, output: %s, ext: %s", outputPath, ext)
		return core.GetErrorf(strs.ErrFormatNotSupport, ext)
	}

	set := settingLoader.GetSetting()
	var result *core.PicResult

	// BMP 格式不支持压缩, 强制使用按质量压缩 + 最高画质
	if ext == ".bmp" {
		logger.Debug("bmp output, force compress by quality with best level, input: %s", inputPath)
		result = compressor.CompressByQuality(settingLoader.QualityLevelBest)
		return finishCompress(start, inputPath, outputPath, result)
	}

	switch set.CompressType {
	case settingLoader.CompressQuality:
		logger.Debug("compress by quality, input: %s, level: %s", inputPath, settingLoader.QualityLevelName(set.Quality))
		result = compressor.CompressByQuality(set.Quality)
	case settingLoader.CompressFileSize:
		var limitBytes int64
		switch set.SizeUnit {
		case settingLoader.UnitKB:
			limitBytes = set.LimitSize * 1024
		case settingLoader.UnitMB:
			limitBytes = set.LimitSize * 1024 * 1024
		default:
			limitBytes = set.LimitSize * 1024
		}
		logger.Debug("compress by file size, input: %s, limit: %d %s, limit bytes: %d",
			inputPath, set.LimitSize, settingLoader.SizeUnitName(set.SizeUnit), limitBytes)
		result = compressor.CompressByFileSize(limitBytes)
	default:
		logger.Error("unknown compress type, type: %d", set.CompressType)
		return core.GetError(strs.ErrNotImplemented)
	}

	return finishCompress(start, inputPath, outputPath, result)
}

func finishCompress(start time.Time, inputPath, outputPath string, result *core.PicResult) *core.PicResult {
	if !result.Ok {
		logger.Error("compress failed, input: %s, output: %s, result: %s, message: %s, elapsed: %v",
			inputPath, outputPath, resultName(result.CompressResult), result.Message, time.Since(start))
		return result
	}
	logger.Debug("compress done, input: %s, output: %s, elapsed: %v", inputPath, outputPath, time.Since(start))
	return result
}

func resultName(r core.CompressResultEnum) string {
	switch r {
	case core.ResultOk:
		return "ok"
	case core.ResultOutOfLimit:
		return "out-of-limit"
	default:
		return "error"
	}
}

type baseCompressor struct {
	img image.Image
	// maxQuality 编码器支持的画质上限, 用于校验按质量压缩的入参.
	maxQuality int
	// sizeUpper 按大小压缩时二分查找的画质上限, 可低于 maxQuality
	// (例如 PNG 的最高等级为无损编码, 不参与按大小压缩的搜索).
	sizeUpper int
}

// compressByQuality 各格式压缩器共享的质量压缩实现.
// encode 参数由具体格式压缩器提供, 执行实际编码.
func (c *baseCompressor) compressByQuality(quality int, encode func(int) ([]byte, error), outputPath string) *core.PicResult {
	logger.Debug("encode by quality, quality: %d, max quality: %d, output: %s", quality, c.maxQuality, outputPath)
	if quality < 1 || quality > c.maxQuality {
		logger.Error("quality out of range, quality: %d, max quality: %d", quality, c.maxQuality)
		return core.GetError(strs.ErrArgOutOfRange)
	}

	data, err := encode(quality)
	if err != nil {
		logger.Error("encode image failed, quality: %d, output: %s, err: %v", quality, outputPath, err)
		return core.GetErrorf(strs.ErrEncodeFailed, err)
	}
	logger.Debug("encode image done, quality: %d, size: %d", quality, len(data))

	return writeOutput(outputPath, data)
}

// compressByFileSize 各格式压缩器共享的按大小压缩实现.
// 在 [1, sizeUpper] 闭区间内二分查找编码体积不超过 limitBytes 的最高画质.
func (c *baseCompressor) compressByFileSize(limitBytes int64, encode func(int) ([]byte, error), outputPath string) *core.PicResult {
	logger.Debug("binary search start, limit: %d, lo: 1, hi: %d, output: %s", limitBytes, c.sizeUpper, outputPath)

	sizeCache := make(map[int]int64)
	var bestData []byte

	// measure 返回指定画质的编码体积. 未命中缓存时同时返回编码结果, 命中时返回 nil.
	measure := func(quality int) (int64, []byte, error) {
		if size, ok := sizeCache[quality]; ok {
			logger.Debug("size cache hit, quality: %d, size: %d", quality, size)
			return size, nil, nil
		}
		data, err := encode(quality)
		if err != nil {
			logger.Error("encode image failed during search, quality: %d, output: %s, err: %v", quality, outputPath, err)
			return 0, nil, err
		}
		size := int64(len(data))
		sizeCache[quality] = size
		logger.Debug("measure quality, quality: %d, size: %d, limit: %d, fit: %t", quality, size, limitBytes, size <= limitBytes)
		return size, data, nil
	}

	lo, hi := 1, c.sizeUpper
	for lo < hi {
		mid := (lo + hi + 1) / 2

		size, data, err := measure(mid)
		if err != nil {
			return core.GetErrorf(strs.ErrEncodeFailed, err)
		}

		if size <= limitBytes {
			lo = mid
			if data != nil {
				bestData = data
			}
		} else {
			hi = mid - 1
		}
		logger.Debug("binary search step, lo: %d, hi: %d, mid: %d, size: %d, fit: %t", lo, hi, mid, size, size <= limitBytes)
	}

	// 循环结束后 lo 可能从未被测量过 (区间直接收敛到下限), 补测一次.
	size, ok := sizeCache[lo]
	if !ok {
		logger.Debug("binary search result not measured, measure lo: %d", lo)
		measured, data, err := measure(lo)
		if err != nil {
			return core.GetErrorf(strs.ErrEncodeFailed, err)
		}
		size, bestData = measured, data
	}
	logger.Debug("binary search done, quality: %d, size: %d, limit: %d", lo, size, limitBytes)

	if size > limitBytes && !settingLoader.GetSetting().AcceptExceed {
		logger.Warn("limit not reachable, smallest quality: %d, size: %d, limit: %d, accept exceed: false", lo, size, limitBytes)
		return core.GetOutOfLimit()
	}

	if bestData == nil {
		logger.Debug("no encoded data kept during search, encode again, quality: %d", lo)
		data, err := encode(lo)
		if err != nil {
			logger.Error("encode image failed, quality: %d, output: %s, err: %v", lo, outputPath, err)
			return core.GetErrorf(strs.ErrEncodeFailed, err)
		}
		bestData = data
	}

	return writeOutput(outputPath, bestData)
}

func writeOutput(outputPath string, data []byte) *core.PicResult {
	dir := filepath.Dir(outputPath)
	if err := fileio.EnsureDir(dir); err != nil {
		logger.Error("create output dir failed, dir: %s, err: %v", dir, err)
		return core.GetErrorf(strs.ErrMkdirFailed, err)
	}

	logger.Debug("write output file, path: %s, size: %d", outputPath, len(data))
	if err := os.WriteFile(outputPath, data, 0644); err != nil {
		logger.Error("write output file failed, path: %s, size: %d, err: %v", outputPath, len(data), err)
		return core.GetErrorf(strs.ErrWriteFileFailed, err)
	}

	return core.GetOk()
}
