package core

import (
	"PicSizer/internal/core/setting"
	"fmt"
)

// 数据模型模块

// PicResult 压缩结果
type PicResult struct {
	Ok             bool
	CompressResult CompressResultEnum
	Message        string
}

// CompressResultEnum 压缩结果枚举
type CompressResultEnum int

const (
	ResultOk         CompressResultEnum = iota // 成功
	ResultOutOfLimit                           // 超出限制
	ResultError                                // 错误
)

// GetOk 返回成功结果
func GetOk() *PicResult {
	return &PicResult{Ok: true, CompressResult: ResultOk}
}

// GetOutOfLimit 返回超出限制结果
func GetOutOfLimit() *PicResult {
	return &PicResult{Ok: false, CompressResult: ResultOutOfLimit, Message: ErrOutOfLimit}
}

// GetError 返回错误结果
func GetError(message string) *PicResult {
	return &PicResult{Ok: false, CompressResult: ResultError, Message: message}
}

// GetErrorf 返回格式化错误结果
func GetErrorf(format string, args ...interface{}) *PicResult {
	return &PicResult{Ok: false, CompressResult: ResultError, Message: fmt.Sprintf(format, args...)}
}

// PicItem 图片项目信息
type PicItem struct {
	FullPath   string               // 完整路径
	FileName   string               // 文件名
	OrigSize   int64                // 原始大小（字节）
	NewSize    int64                // 新大小（字节）
	State      setting.PicItemState // 状态
	Message    string               // 错误信息
	OutputPath string               // 输出路径
}

// 全局运行时变量
var (
	OutputDirPath string // 输出目录
	PublicDirPath string // 公共目录（保留结构时使用）
	ExitFlag      bool   // 是否退出压缩
)

// FormatFileSize 格式化文件大小
func FormatFileSize(size int64) string {
	if size < 1024 {
		return fmt.Sprintf("%d B", size)
	}
	if size < 1024*1024 {
		return fmt.Sprintf("%.2f KB", float64(size)/1024.0)
	}
	return fmt.Sprintf("%.2f MB", float64(size)/(1024.0*1024.0))
}
