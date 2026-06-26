package fileio

import (
	"os"
	"path/filepath"
	"strings"
)

// GetExtension 获取文件的后缀名 (含点号, 小写字母).
func GetExtension(path string) string {
	ext := filepath.Ext(path)
	return strings.ToLower(ext)
}

// GetFileNameWithoutExt 获取文件名中不包含扩展名的部分.
func GetFileNameWithoutExt(path string) string {
	return strings.TrimSuffix(filepath.Base(path), filepath.Ext(path))
}

// IsImageFile 判断指定路径是否为程序支持的图片文件.
func IsImageFile(path string) bool {
	ext := GetExtension(path)
	switch ext {
	case ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".tiff", ".tif":
		return true
	}
	return false
}

// GetSupportedExtensions 返回支持的文件扩展名过滤器字符串, 用于文件对话框.
func GetSupportedExtensions() string {
	return "*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp;*.tiff;*.tif"
}

// CollectImageFiles 递归遍历指定目录, 收集所有支持的图片文件路径.
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

// GetPublicDirectory 获取多个文件路径的公共父目录.
// 该函数为兼容旧版本保留, 推荐使用 GetCommonPrefix.
func GetPublicDirectory(paths []string) string {
	prefix, _ := GetCommonPrefix(paths)
	return prefix
}

// GetCommonPrefix 计算多个文件路径的公共父目录.
// 跨盘符时返回 ("", false). 仅一个文件时返回其所在目录.
func GetCommonPrefix(paths []string) (prefix string, ok bool) {
	if len(paths) == 0 {
		return "", false
	}
	if len(paths) == 1 {
		return filepath.Dir(paths[0]), true
	}

	root := filepath.Dir(paths[0])

	for _, path := range paths[1:] {
		dir := filepath.Dir(path)

		if filepath.VolumeName(root) != filepath.VolumeName(dir) {
			return "", false
		}

		for {
			if dir == root {
				break
			}
			if strings.HasPrefix(dir, root+string(filepath.Separator)) {
				break
			}
			parent := filepath.Dir(root)
			if parent == root {
				return "", false
			}
			root = parent
		}
	}
	return root, true
}

// EnsureDir 确保指定目录存在, 若不存在则递归创建.
func EnsureDir(dir string) error {
	return os.MkdirAll(dir, os.ModePerm)
}

// GetFileInfo 获取指定文件路径的文件信息.
func GetFileInfo(path string) (os.FileInfo, error) {
	return os.Stat(path)
}
