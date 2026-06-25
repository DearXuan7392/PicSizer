package fileio

import (
	"PicSizer/internal/core/setting"
	"path/filepath"
	"strings"

	"PicSizer/internal/core"
)

// GetOutputPath 根据配置的输出方式生成最终的输出文件路径。
// 支持三种输出方式：目录模式、覆盖模式、保留结构模式。
// 文件名模板支持 {id}（序号）和 {name}（原始文件名）两种变量。
func GetOutputPath(input string, index int) string {
	set := setting.GetSetting()

	switch set.OutputType {
	case setting.OutputDirection:
		output := applyFilenameTemplate(set.OutputFilename, input, index)
		output += getOutputExtension(input, set.Extension)
		return filepath.Join(core.OutputDirPath, output)

	case setting.OutputCoverOrigin:
		if set.Extension != setting.ExtOrigin {
			ext := setting.GetExtensionString(set.Extension)
			return input[:len(input)-len(filepath.Ext(input))] + ext
		}
		return input

	case setting.OutputStructure:
		relativePath := strings.TrimPrefix(input, core.PublicDirPath+string(filepath.Separator))
		relDir := filepath.Dir(relativePath)
		fileName := applyFilenameTemplate(set.OutputFilename, input, index)
		fileName += getOutputExtension(input, set.Extension)
		finalPath := filepath.Join(core.OutputDirPath, relDir, fileName)
		dir := filepath.Dir(finalPath)
		EnsureDir(dir)
		return finalPath

	default:
		return ""
	}
}

// applyFilenameTemplate 将文件名模板中的变量替换为实际值。
// {id} 替换为序号，{name} 替换为原始文件名（不含扩展名）。
func applyFilenameTemplate(tpl, input string, index int) string {
	output := tpl
	output = strings.ReplaceAll(output, "{id}", itoa(index))
	output = strings.ReplaceAll(output, "{name}", GetFileNameWithoutExt(input))
	return output
}

// getOutputExtension 根据输出格式类型返回文件后缀（含点号）。
// ExtOrigin 时返回源文件后缀，其他格式返回对应格式的固定后缀。
func getOutputExtension(input string, extType setting.ExtensionType) string {
	if extType == setting.ExtOrigin {
		ext := filepath.Ext(input)
		if ext == "" {
			return setting.GetExtensionString(setting.ExtJPEG)
		}
		return ext
	}
	return setting.GetExtensionString(extType)
}

// itoa 将整数转换为字符串的简单实现。
func itoa(n int) string {
	if n == 0 {
		return "0"
	}
	var digits []byte
	for n > 0 {
		digits = append([]byte{byte('0' + n%10)}, digits...)
		n /= 10
	}
	return string(digits)
}
