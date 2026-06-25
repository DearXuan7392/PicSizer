package setting

import (
	"fmt"
	"strings"
)

// QualityLevel 按质量压缩的画质等级.
//
// 为了让用户更直观地选择压缩质量, 避免直接面对 1-100 的数字,
// 程序以 4 个等级表示画质, 内部再映射到具体的画质数值.
//
//   - QualityLevelBest   : 最佳, 对应画质 95
//   - QualityLevelClear  : 清晰, 对应画质 80 (默认值)
//   - QualityLevelNormal : 一般, 对应画质 40
//   - QualityLevelPoor   : 较差, 对应画质 10
type QualityLevel int

const (
	QualityLevelBest   QualityLevel = iota // 最佳 (画质 95)
	QualityLevelClear                      // 清晰 (画质 80)
	QualityLevelNormal                     // 一般 (画质 40)
	QualityLevelPoor                       // 较差 (画质 10)
)

// qualityLevelValues 4 个画质等级到具体画质值的映射表.
//
// 下标与 QualityLevel 常量值一一对应; 新增等级时只需追加, 不要改动顺序.
var qualityLevelValues = [...]int{
	95, // QualityLevelBest
	80, // QualityLevelClear
	40, // QualityLevelNormal
	10, // QualityLevelPoor
}

// QualityLevelValue 将画质等级转换为具体的画质数值.
//
// 该数值是真正传递给 JPEG/WebP 编码器的质量参数 (范围 1-100).
// PNG 编码器内部使用另一套等级 (1-4), 由编码器自行处理.
func QualityLevelValue(level QualityLevel) int {
	idx := int(level)
	if idx < 0 || idx >= len(qualityLevelValues) {
		panic(fmt.Sprintf("QualityLevel value out of range: %d", level))
	}
	return qualityLevelValues[idx]
}

// ParseQualityLevel 根据字符串解析对应的画质等级.
//
// 接受 "best" / "clear" / "normal" / "poor" 以及中文 "最佳" / "清晰" / "一般" / "较差".
// 解析失败时返回错误, 由调用方决定是否使用默认值.
func ParseQualityLevel(s string) (QualityLevel, error) {
	switch strings.ToLower(strings.TrimSpace(s)) {
	case "best", "最佳":
		return QualityLevelBest, nil
	case "clear", "清晰":
		return QualityLevelClear, nil
	case "normal", "一般":
		return QualityLevelNormal, nil
	case "poor", "较差":
		return QualityLevelPoor, nil
	default:
		return QualityLevelClear, fmt.Errorf("不支持的画质等级: %s (可选: best/clear/normal/poor 或 最佳/清晰/一般/较差)", s)
	}
}
