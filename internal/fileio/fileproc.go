package fileio

import (
	"os"
	"path/filepath"
	"strings"
)

// GetExtension 获取文件后缀名（包括点号，小写字母）
func GetExtension(path string) string {
	ext := filepath.Ext(path)
	return strings.ToLower(ext)
}

// GetFileNameWithoutExt 获取不带扩展名的文件名
func GetFileNameWithoutExt(path string) string {
	return strings.TrimSuffix(filepath.Base(path), filepath.Ext(path))
}

// IsImageFile 判断是否为支持的图片文件
func IsImageFile(path string) bool {
	ext := GetExtension(path)
	switch ext {
	case ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".tiff", ".tif":
		return true
	}
	return false
}

// GetSupportedExtensions 获取支持的图片扩展名过滤器字符串
func GetSupportedExtensions() string {
	return "*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp;*.tiff;*.tif"
}

// CollectImageFiles 从目录中收集所有图片文件
func CollectImageFiles(dir string) ([]string, error) {
	var files []string
	err := filepath.Walk(dir, func(path string, info os.FileInfo, err error) error {
		if err != nil {
			return err
		}
		if !info.IsDir() && IsImageFile(path) {
			files = append(files, path)
		}
		return nil
	})
	return files, err
}

// GetPublicDirectory 获取多个文件的公共目录
//
// 说明: 该函数为兼容旧版本保留, 实际推荐使用 GetCommonPrefix,
// 因为 GetPublicDirectory 在跨盘符 (Windows) 等无法找到共同前缀时
// 仍会返回一个非空字符串 (如盘符根目录), 不便于调用方判断.
// 新代码应直接调用 GetCommonPrefix.
func GetPublicDirectory(paths []string) string {
	prefix, _ := GetCommonPrefix(paths)
	return prefix
}

// GetCommonPrefix 计算多个路径的公共父目录
//
// 行为:
//   - paths 为空时, 返回 "" 与 false.
//   - paths 仅有一个元素时, 返回该文件所在目录与 true.
//   - 多元素时, 返回所有路径的公共父目录; 若无法找到共同前缀
//     (例如跨盘符, C:\ 与 D:\), 返回 "" 与 false.
//
// 示例:
//   - ["/home/a.jpg", "/home/a/2.png"] -> ("/home", true)
//   - ["C:\\a\\b.jpg", "C:\\a\\c.png"] -> ("C:\\a", true)
//   - ["C:\\a.jpg", "D:\\a.png"]       -> ("", false)
//
// 返回值 (prefix, ok):
//   - prefix: 共同父目录; 找不到时为空字符串
//   - ok:    是否成功找到共同前缀
func GetCommonPrefix(paths []string) (prefix string, ok bool) {
	if len(paths) == 0 {
		return "", false
	}
	if len(paths) == 1 {
		// 只有一个文件时, 公共前缀为该文件所在目录
		return filepath.Dir(paths[0]), true
	}

	// 初始 root 取第一个文件所在目录
	root := filepath.Dir(paths[0])

	for _, path := range paths[1:] {
		dir := filepath.Dir(path)

		// 1. 检查卷标: 跨盘符时 (如 C: 与 D:) 直接判定为无共同前缀
		if filepath.VolumeName(root) != filepath.VolumeName(dir) {
			return "", false
		}

		// 2. 自底向上缩减 root, 直到 root 是 dir 的祖先目录
		for {
			// root 与 dir 完全相同, 已是祖先目录
			if dir == root {
				break
			}
			// dir 以 root + 路径分隔符 开头, root 是 dir 的祖先
			if strings.HasPrefix(dir, root+string(filepath.Separator)) {
				break
			}
			// 已到达卷标根 (如 "/" 或 "C:\\"), 仍不是 dir 的祖先, 视为无共同前缀
			parent := filepath.Dir(root)
			if parent == root {
				return "", false
			}
			root = parent
		}
	}
	return root, true
}

// EnsureDir 确保目录存在，如不存在则创建
func EnsureDir(dir string) error {
	return os.MkdirAll(dir, os.ModePerm)
}

// GetFileInfo 获取文件信息
func GetFileInfo(path string) (os.FileInfo, error) {
	return os.Stat(path)
}
