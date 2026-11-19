package core

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/core/strings"
	"fmt"
)

// PicResult 单张图片的压缩结果.
// Ok 为 true 时表示压缩成功, CompressResult 指明具体结果类型.
type PicResult struct {
	Ok             bool
	CompressResult CompressResultEnum
	Message        string
}

type CompressResultEnum int

const (
	ResultOk CompressResultEnum = iota
	ResultOutOfLimit
	ResultError
)

func GetOk() *PicResult {
	return &PicResult{Ok: true, CompressResult: ResultOk}
}

func GetOutOfLimit() *PicResult {
	return &PicResult{Ok: false, CompressResult: ResultOutOfLimit, Message: strs.ErrOutOfLimit}
}

func GetError(message string) *PicResult {
	return &PicResult{Ok: false, CompressResult: ResultError, Message: message}
}

func GetErrorf(format string, args ...interface{}) *PicResult {
	return &PicResult{Ok: false, CompressResult: ResultError, Message: fmt.Sprintf(format, args...)}
}

type PicItem struct {
	FullPath   string
	FileName   string
	OrigSize   int64
	NewSize    int64
	State      settingLoader.PicItemState
	Message    string
	OutputPath string
}

var (
	OutputDirPath string
	PublicDirPath string
)

// FormatFileSize 将字节数格式化为人类可读的字符串 (B/KB/MB).
func FormatFileSize(size int64) string {
	if size < 1024 {
		return fmt.Sprintf("%d B", size)
	}
	if size < 1024*1024 {
		return fmt.Sprintf("%.2f KB", float64(size)/1024.0)
	}
	return fmt.Sprintf("%.2f MB", float64(size)/(1024.0*1024.0))
}
