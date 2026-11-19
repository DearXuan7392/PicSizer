package fileio

import (
	"PicSizer/internal/core/settingLoader"
	"path/filepath"
	"strings"

	"PicSizer/internal/core"
)

// GetOutputPath 根据配置的输出方式生成输出文件路径.
// 文件名模板支持 {id} (序号) 与 {name} (原始文件名) 两个变量.
func GetOutputPath(input string, index int) string {
	set := settingLoader.GetSetting()

	switch set.OutputType {
	case settingLoader.OutputDirection:
		output := applyFilenameTemplate(set.OutputFilename, input, index)
		output += getOutputExtension(input, set.Extension)
		return filepath.Join(core.OutputDirPath, output)

	case settingLoader.OutputCoverOrigin:
		if set.Extension != settingLoader.ExtOrigin {
			ext := settingLoader.GetExtensionString(set.Extension)
			return input[:len(input)-len(filepath.Ext(input))] + ext
		}
		return input

	case settingLoader.OutputStructure:
		relativePath := strings.TrimPrefix(input, core.PublicDirPath+string(filepath.Separator))
		relDir := filepath.Dir(relativePath)
		fileName := applyFilenameTemplate(set.OutputFilename, input, index)
		fileName += getOutputExtension(input, set.Extension)
		return filepath.Join(core.OutputDirPath, relDir, fileName)

	default:
		return ""
	}
}

// applyFilenameTemplate 将模板变量替换为实际值:
// {id} 替换为序号, {name} 替换为原始文件名 (不含扩展名).
func applyFilenameTemplate(tpl, input string, index int) string {
	output := tpl
	output = strings.ReplaceAll(output, "{id}", itoa(index))
	output = strings.ReplaceAll(output, "{name}", GetFileNameWithoutExt(input))
	return output
}

// getOutputExtension 返回输出文件后缀.
// ExtOrigin 时沿用源文件后缀 (无后缀时视为 jpg), 其他格式返回固定后缀.
func getOutputExtension(input string, extType settingLoader.ExtensionType) string {
	if extType == settingLoader.ExtOrigin {
		ext := filepath.Ext(input)
		if ext == "" {
			return settingLoader.GetExtensionString(settingLoader.ExtJPEG)
		}
		return ext
	}
	return settingLoader.GetExtensionString(extType)
}

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
