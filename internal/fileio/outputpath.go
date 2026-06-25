package fileio

import (
	"PicSizer/internal/core/setting"
	"path/filepath"
	"strings"

	"PicSizer/internal/core"
)

// GetOutputPath 根据输出设置生成输出文件路径
//
// 文件名模板规则:
//   - 模板中不再包含后缀, 输出格式后缀由程序根据 setting.Extension 自动追加.
//   - 模板中可使用的变量:
//     {id}    - 输出序号 (从 StartIndex 开始递增)
//     {index} - 同 {id}, 旧版兼容变量
//     {name}  - 原始文件名 (不含扩展名)
//
// 三种输出方式:
//   - OutputDirection  : 输出到统一目录, 使用模板生成文件名, 自动追加后缀
//   - OutputCoverOrigin: 直接覆盖源文件, 不使用模板
//   - OutputStructure  : 保留目录结构, 使用模板生成文件名, 自动追加后缀
func GetOutputPath(input string, index int) string {
	set := setting.GetSetting()

	switch set.OutputType {
	case setting.OutputDirection:
		// 输出到统一文件夹
		// 使用模板生成文件名, 末尾自动追加输出格式后缀
		output := applyFilenameTemplate(set.OutputFilename, input, index)
		output += getOutputExtension(input, set.Extension)
		return filepath.Join(core.OutputDirPath, output)

	case setting.OutputCoverOrigin:
		// 覆盖源文件, 需要按输出格式修改扩展名
		if set.Extension != setting.ExtOrigin {
			ext := setting.GetExtensionString(set.Extension)
			return input[:len(input)-len(filepath.Ext(input))] + ext
		}
		return input

	case setting.OutputStructure:
		// 保留文件夹结构
		// 1. 计算相对于公共目录的路径, 提取相对目录
		relativePath := strings.TrimPrefix(input, core.PublicDirPath+string(filepath.Separator))
		relDir := filepath.Dir(relativePath)
		// 2. 使用模板生成文件名 (不再使用源文件名, 仅保留目录结构)
		fileName := applyFilenameTemplate(set.OutputFilename, input, index)
		// 3. 追加输出格式后缀
		fileName += getOutputExtension(input, set.Extension)
		// 4. 拼接: 输出目录 + 相对目录 + 文件名
		finalPath := filepath.Join(core.OutputDirPath, relDir, fileName)
		// 确保目录存在
		dir := filepath.Dir(finalPath)
		EnsureDir(dir)
		return finalPath

	default:
		return ""
	}
}

// applyFilenameTemplate 应用文件名模板, 替换 {id} / {index} / {name} 变量
//
// 说明: 模板中不再包含后缀, 后缀由 getOutputExtension 单独追加.
// 保留 {ext} 替换为兼容旧版 (返回空串, 后续追加的后缀会覆盖其作用).
func applyFilenameTemplate(tpl, input string, index int) string {
	output := tpl
	output = strings.ReplaceAll(output, "{id}", itoa(index))
	output = strings.ReplaceAll(output, "{name}", GetFileNameWithoutExt(input))
	return output
}

// getOutputExtension 根据输出格式返回后缀字符串 (含点号)
//
// 行为:
//   - ExtOrigin: 返回源文件的后缀 (含点号), 用于"保持原格式"
//   - 其他     : 返回对应格式的固定后缀 (含点号)
func getOutputExtension(input string, extType setting.ExtensionType) string {
	if extType == setting.ExtOrigin {
		// 保持原格式: 复用源文件后缀
		// 当源文件无后缀时, 返回空串
		ext := filepath.Ext(input)
		if ext == "" {
			// 源文件无后缀时, 默认追加 .jpg, 避免生成无后缀文件
			return setting.GetExtensionString(setting.ExtJPEG)
		}
		return ext
	}
	return setting.GetExtensionString(extType)
}

// itoa 整数转字符串（简单实现）
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
