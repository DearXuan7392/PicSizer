package compress

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/log"
	"image"
	"os"
	"path/filepath"

	"PicSizer/internal/core"
	strs "PicSizer/internal/core/strings"
	"PicSizer/internal/fileio"
	"PicSizer/internal/fileio/codec"
	"PicSizer/internal/preprocess"
)

// Compressor 定义统一的压缩器接口.
// 各图片格式 (JPEG/PNG/WebP) 需实现该接口.
type Compressor interface {
	CompressByQuality(quality settingLoader.QualityLevel) *core.PicResult
	CompressByFileSize(limitBytes int64) *core.PicResult
}

var (
	logger = log.NewLogger("compress")
)

// Compress 是统一压缩入口, 根据输入路径和输出路径执行完整压缩流程.
// 流程包括: 加载图片 -> 预处理 -> 选择压缩器 -> 执行压缩.
func Compress(inputPath, outputPath string) *core.PicResult {
	imgData, err := codec.LoadImage(inputPath)
	if err != nil {
		return core.GetError(err.Error())
	}

	imgData = preprocess.Process(imgData)

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
		return core.GetErrorf(strs.ErrFormatNotSupport, ext)
	}

	set := settingLoader.GetSetting()
	var result *core.PicResult

	// BMP 格式不支持压缩, 强制使用按质量压缩 + 最高画质
	if ext == ".bmp" {
		result = compressor.CompressByQuality(settingLoader.QualityLevelBest)
		if !result.Ok {
			logger.Error("%s", result.Message)
		}
		return result
	}

	switch set.CompressType {
	case settingLoader.CompressQuality:
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
		result = compressor.CompressByFileSize(limitBytes)
	default:
		return core.GetError(strs.ErrNotImplemented)
	}

	if !result.Ok {
		logger.Error("%s", result.Message)
	}
	return result
}

type baseCompressor struct {
	img image.Image
	// maxQuality 编码器支持的画质上限, 用于校验按质量压缩的入参.
	maxQuality int
	// sizeUpper 按大小压缩时二分查找的画质上限, 可低于 maxQuality
	// (例如 PNG 的最优一级为无损编码, 不参与按大小压缩的搜索).
	sizeUpper int
}

// compressByQuality 是各格式压缩器共享的质量压缩通用实现.
// encode 参数由具体格式压缩器提供, 用于执行实际的编码操作.
func (c *baseCompressor) compressByQuality(quality int, encode func(int) ([]byte, error), outputPath string) *core.PicResult {
	logger.Debug("compress by quality, quality: %d", quality)
	if quality < 1 || quality > c.maxQuality {
		return core.GetError(strs.ErrArgOutOfRange)
	}

	data, err := encode(quality)
	if err != nil {
		return core.GetErrorf(strs.ErrEncodeFailed, err)
	}

	dir := filepath.Dir(outputPath)
	if err := fileio.EnsureDir(dir); err != nil {
		return core.GetErrorf(strs.ErrMkdirFailed, err)
	}

	err = os.WriteFile(outputPath, data, 0644)
	if err != nil {
		return core.GetErrorf(strs.ErrWriteFileFailed, err)
	}

	return core.GetOk()
}

// compressByFileSize 是各格式压缩器共享的二分查找大小压缩通用实现.
// 在 [1, sizeUpper] 区间内查找编码体积不超过 limitBytes 的最大画质, 上下限均可取到.
func (c *baseCompressor) compressByFileSize(limitBytes int64, encode func(int) ([]byte, error), outputPath string) *core.PicResult {
	logger.Debug("compress by file size, limit: %d, upper: %d", limitBytes, c.sizeUpper)

	sizeCache := make(map[int]int64)
	var bestData []byte

	// measure 返回指定画质的编码体积, 首次计算时同时返回编码结果, 命中缓存时返回 nil.
	measure := func(quality int) (int64, []byte, error) {
		if size, ok := sizeCache[quality]; ok {
			return size, nil, nil
		}
		data, err := encode(quality)
		if err != nil {
			return 0, nil, err
		}
		size := int64(len(data))
		sizeCache[quality] = size
		logger.Debug("quality: %d, size: %d", quality, size)
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
	}

	// lo 可能从未被搜索到 (区间直接收敛到下限), 此时需要补测一次
	size, ok := sizeCache[lo]
	if !ok {
		measured, data, err := measure(lo)
		if err != nil {
			return core.GetErrorf(strs.ErrEncodeFailed, err)
		}
		size, bestData = measured, data
	}

	if size > limitBytes && !settingLoader.GetSetting().AcceptExceed {
		return core.GetOutOfLimit()
	}

	if bestData == nil {
		data, err := encode(lo)
		if err != nil {
			return core.GetErrorf(strs.ErrEncodeFailed, err)
		}
		bestData = data
	}

	dir := filepath.Dir(outputPath)
	if err := fileio.EnsureDir(dir); err != nil {
		return core.GetErrorf(strs.ErrMkdirFailed, err)
	}

	if err := os.WriteFile(outputPath, bestData, 0644); err != nil {
		return core.GetErrorf(strs.ErrWriteFileFailed, err)
	}

	return core.GetOk()
}
