package fileio

import (
	"PicSizer/internal/log"
	"os"
	"path/filepath"
	"strings"
)

var logger = log.NewLogger("fileio")

// GetExtension 返回文件后缀名 (含点号, 小写).
func GetExtension(path string) string {
	ext := filepath.Ext(path)
	return strings.ToLower(ext)
}

func GetFileNameWithoutExt(path string) string {
	return strings.TrimSuffix(filepath.Base(path), filepath.Ext(path))
}

func IsImageFile(path string) bool {
	ext := GetExtension(path)
	switch ext {
	case ".jpg", ".jpeg", ".png", ".bmp", ".webp":
		return true
	}
	return false
}

// GetSupportedExtensions 返回文件对话框使用的图片扩展名过滤器.
func GetSupportedExtensions() string {
	return "*.jpg;*.jpeg;*.png;*.bmp;*.webp"
}

// CollectImageFiles 递归遍历目录, 收集所有支持的图片文件路径.
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
	if err != nil {
		logger.Warn("collect image files aborted, dir: %s, err: %v", dir, err)
		return nil, err
	}
	logger.Debug("collect image files done, dir: %s, count: %d", dir, len(files))
	return files, nil
}

// GetPublicDirectory 为旧版本兼容保留, 推荐使用 GetCommonPrefix.
func GetPublicDirectory(paths []string) string {
	prefix, _ := GetCommonPrefix(paths)
	return prefix
}

// GetCommonPrefix 计算多个路径的公共父目录.
// 跨盘符时返回 ("", false); 仅一个路径时返回其所在目录.
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

func EnsureDir(dir string) error {
	return os.MkdirAll(dir, os.ModePerm)
}

func GetFileInfo(path string) (os.FileInfo, error) {
	return os.Stat(path)
}
