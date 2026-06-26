package core

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/core/strings"
	"fmt"
)

// PicResult 表示单张图片的压缩结果.
// Ok 为 true 时表示压缩成功, CompressResult 指明具体结果类型.
type PicResult struct {
	Ok             bool
	CompressResult CompressResultEnum
	Message        string
}

// CompressResultEnum 表示压缩结果类型的枚举.
type CompressResultEnum int

const (
	ResultOk CompressResultEnum = iota
	ResultOutOfLimit
	ResultError
)

// GetOk 返回一个表示压缩成功的 PicResult.
func GetOk() *PicResult {
	return &PicResult{Ok: true, CompressResult: ResultOk}
}

// GetOutOfLimit 返回一个表示超出大小限制的 PicResult.
func GetOutOfLimit() *PicResult {
	return &PicResult{Ok: false, CompressResult: ResultOutOfLimit, Message: strs.ErrOutOfLimit}
}

// GetError 返回一个包含指定错误信息的 PicResult.
func GetError(message string) *PicResult {
	return &PicResult{Ok: false, CompressResult: ResultError, Message: message}
}

// GetErrorf 返回一个包含格式化错误信息的 PicResult.
func GetErrorf(format string, args ...interface{}) *PicResult {
	return &PicResult{Ok: false, CompressResult: ResultError, Message: fmt.Sprintf(format, args...)}
}

// PicItem 表示待压缩图片的项目信息, 包含文件路径、大小和压缩状态.
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
	ExitFlag      bool
)

// FormatFileSize 将字节数格式化为人类可读的字符串, 自动选择 B/KB/MB 单位.
func FormatFileSize(size int64) string {
	if size < 1024 {
		return fmt.Sprintf("%d B", size)
	}
	if size < 1024*1024 {
		return fmt.Sprintf("%.2f KB", float64(size)/1024.0)
	}
	return fmt.Sprintf("%.2f MB", float64(size)/(1024.0*1024.0))
}
