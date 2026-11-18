package fileio

import (
	"fmt"
	"os"
	"path/filepath"
	"strconv"
	"strings"
)

// DuplicatePath 表示一组落在同一输出路径上的项目.
type DuplicatePath struct {
	Path  string // 首次出现时的书写形式
	Count int    // 占用该路径的项目数量
}

// FindDuplicates 找出 paths 中重复的路径, 按首次出现顺序返回.
// Windows 文件系统不区分大小写, 因此比较前统一转为小写并清理路径.
func FindDuplicates(paths []string) []DuplicatePath {
	counts := make(map[string]int, len(paths))
	literals := make(map[string]string, len(paths))
	var order []string

	for _, p := range paths {
		key := pathKey(p)
		if _, ok := counts[key]; !ok {
			order = append(order, key)
			literals[key] = p
		}
		counts[key]++
	}

	var duplicates []DuplicatePath
	for _, key := range order {
		if counts[key] > 1 {
			duplicates = append(duplicates, DuplicatePath{Path: literals[key], Count: counts[key]})
		}
	}
	return duplicates
}

// MakeUniquePaths 为 paths 依次分配互不覆盖的路径.
// 首个占用者保留原路径 (与原有覆盖行为一致), 后续冲突按 "abc (2).jpg"、"abc (3).jpg" 递增,
// 且自动生成的名称会避开磁盘上已存在的同名文件.
func MakeUniquePaths(paths []string) []string {
	used := make(map[string]bool, len(paths))
	unique := make([]string, len(paths))

	for i, p := range paths {
		unique[i] = uniquePath(p, used)
		if unique[i] != p {
			logger.Debug("assign unique output path, from: %s, to: %s", p, unique[i])
		}
		used[pathKey(unique[i])] = true
	}
	return unique
}

// uniquePath 返回本批次内尚未被占用的路径.
// 被占用时自 " (2)" 起递增, 直到既不与本批冲突、也不覆盖磁盘已有文件.
func uniquePath(path string, used map[string]bool) string {
	if !used[pathKey(path)] {
		return path
	}

	dir, name := filepath.Split(path)
	stem, ext := splitCopyStem(name)

	for n := 2; ; n++ {
		candidate := filepath.Join(dir, fmt.Sprintf("%s (%d)%s", stem, n, ext))
		if !used[pathKey(candidate)] && !fileExists(candidate) {
			return candidate
		}
	}
}

// splitCopyStem 拆分文件名, 并剥离已有的 " (n)" 副本后缀,
// 使 "abc (2).jpg" 的下一份副本为 "abc (3).jpg" 而非 "abc (2) (2).jpg".
func splitCopyStem(name string) (stem, ext string) {
	ext = filepath.Ext(name)
	stem = strings.TrimSuffix(name, ext)

	if !strings.HasSuffix(stem, ")") {
		return stem, ext
	}
	if i := strings.LastIndex(stem, " ("); i >= 0 {
		if _, err := strconv.Atoi(stem[i+2 : len(stem)-1]); err == nil {
			stem = stem[:i]
		}
	}
	return stem, ext
}

func pathKey(path string) string {
	return strings.ToLower(filepath.Clean(path))
}

func fileExists(path string) bool {
	_, err := os.Stat(path)
	return err == nil
}
