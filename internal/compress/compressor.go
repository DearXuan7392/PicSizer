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
		return core.GetErrorf(err.Error())
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
	default:
		return core.GetErrorf(strs.ErrFormatNotSupport, ext)
	}

	set := settingLoader.GetSetting()
	var result *core.PicResult

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
		logger.Error(result.Message)
	}
	return result
}

type baseCompressor struct {
	img        image.Image
	maxQuality int
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
// 通过二分查找在 1 到 maxQuality 之间寻找不超过 limitBytes 的最高质量.
func (c *baseCompressor) compressByFileSize(limitBytes int64, encode func(int) ([]byte, error), outputPath string) *core.PicResult {
	logger.Debug("compress by file size, limit: %d", limitBytes)
	left, right := 1, c.maxQuality

	sizeCache := make(map[int]int64)

	for left < right-1 {
		mid := (left + right) / 2

		size, ok := sizeCache[mid]
		if !ok {
			data, err := encode(mid)
			if err != nil {
				return core.GetErrorf(strs.ErrEncodeFailed, err)
			}
			size = int64(len(data))
			sizeCache[mid] = size
			logger.Debug("quality: %d, size: %d", mid, size)
		}

		if size <= limitBytes {
			left = mid
		} else {
			right = mid
		}
	}

	if _, ok := sizeCache[left]; !ok {
		data, err := encode(left)
		if err != nil {
			return core.GetErrorf(strs.ErrEncodeFailed, err)
		}
		sizeCache[left] = int64(len(data))
	}

	if sizeCache[left] <= limitBytes || settingLoader.GetSetting().AcceptExceed {
		data, err := encode(left)
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

	return core.GetOutOfLimit()
}
