package compress

import (
	"PicSizer/internal/core/setting"
	"image"
	"os"
	"path/filepath"

	"PicSizer/internal/core"
	"PicSizer/internal/fileio"
	"PicSizer/internal/fileio/codec"
	"PicSizer/internal/preprocess"
)

// Compressor 统一压缩接口
type Compressor interface {
	// 按质量压缩
	CompressByQuality(quality setting.QualityLevel) *core.PicResult
	// 按文件大小压缩（二分查找最优质量）
	CompressByFileSize(limitBytes int64) *core.PicResult
}

// Compress 统一压缩入口，根据输出格式选择对应的压缩器
func Compress(inputPath, outputPath string) *core.PicResult {
	// 1. 加载图片
	imgData, err := codec.LoadImage(inputPath)
	if err != nil {
		return core.GetErrorf("加载图片失败: %v", err)
	}

	// 2. 预处理流程 (在压缩前对图像进行处理, 例如透明通道处理)
	imgData = preprocess.Process(imgData)

	// 3. 根据输出扩展名选择压缩器
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
		return core.GetErrorf("\"%s\"格式不受支持", ext)
	}

	// 5. 根据压缩类型执行压缩
	set := setting.GetSetting()
	var result *core.PicResult

	switch set.CompressType {
	case setting.CompressQuality:
		result = compressor.CompressByQuality(set.Quality)
	case setting.CompressFileSize:
		// 根据单位转换为字节
		var limitBytes int64
		switch set.SizeUnit {
		case setting.UnitKB:
			limitBytes = set.LimitSize * 1024
		case setting.UnitMB:
			limitBytes = set.LimitSize * 1024 * 1024
		default:
			limitBytes = set.LimitSize * 1024
		}
		result = compressor.CompressByFileSize(limitBytes)
	default:
		return core.GetError(core.ErrNotImplemented)
	}

	return result
}

// baseCompressor 基础压缩器
type baseCompressor struct {
	img        image.Image
	maxQuality int
}

// compressByQuality 按质量压缩的通用实现
func (c *baseCompressor) compressByQuality(quality int, encode func(int) ([]byte, error), outputPath string) *core.PicResult {
	if quality < 1 || quality > c.maxQuality {
		return core.GetError(core.ErrArgOutOfRange)
	}

	data, err := encode(quality)
	if err != nil {
		return core.GetErrorf("编码失败: %v", err)
	}

	// 确保输出目录存在
	dir := filepath.Dir(outputPath)
	if err := fileio.EnsureDir(dir); err != nil {
		return core.GetErrorf("创建目录失败: %v", err)
	}

	// 写入文件
	err = os.WriteFile(outputPath, data, 0644)
	if err != nil {
		return core.GetErrorf("写入文件失败: %v", err)
	}

	return core.GetOk()
}

// compressByFileSize 按文件大小压缩的通用实现（二分查找）
func (c *baseCompressor) compressByFileSize(limitBytes int64, encode func(int) ([]byte, error), outputPath string) *core.PicResult {
	left, right := 1, c.maxQuality

	// 缓存已测试的质量对应的文件大小
	sizeCache := make(map[int]int64)

	// 二分查找最优质量
	for left < right-1 {
		mid := (left + right) / 2

		// 获取当前质量下的文件大小
		size, ok := sizeCache[mid]
		if !ok {
			data, err := encode(mid)
			if err != nil {
				return core.GetErrorf("编码失败: %v", err)
			}
			size = int64(len(data))
			sizeCache[mid] = size
		}

		if size <= limitBytes {
			left = mid
		} else {
			right = mid
		}
	}

	// 获取 left 质量下的大小
	if _, ok := sizeCache[left]; !ok {
		data, err := encode(left)
		if err != nil {
			return core.GetErrorf("编码失败: %v", err)
		}
		sizeCache[left] = int64(len(data))
	}

	// 如果符合要求或接受超出，则输出
	if sizeCache[left] <= limitBytes || setting.GetSetting().AcceptExceed {
		data, err := encode(left)
		if err != nil {
			return core.GetErrorf("编码失败: %v", err)
		}

		dir := filepath.Dir(outputPath)
		if err := fileio.EnsureDir(dir); err != nil {
			return core.GetErrorf("创建目录失败: %v", err)
		}

		err = os.WriteFile(outputPath, data, 0644)
		if err != nil {
			return core.GetErrorf("写入文件失败: %v", err)
		}

		return core.GetOk()
	}

	return core.GetOutOfLimit()
}
