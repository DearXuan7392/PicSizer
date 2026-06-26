package settingLoader

import (
	"fmt"
	"strings"

	"PicSizer/internal/core/strings"
)

// QualityLevel 表示按质量压缩时的画质等级.
// 程序以 4 个等级让用户选择, 内部映射到具体的画质数值 95/80/40/10.
type QualityLevel int

const (
	QualityLevelBest   QualityLevel = iota // 最佳 (画质 95)
	QualityLevelClear                      // 清晰 (画质 80)
	QualityLevelNormal                     // 一般 (画质 40)
	QualityLevelPoor                       // 较差 (画质 10)
)

var qualityLevelValues = [...]int{
	95, // QualityLevelBest
	80, // QualityLevelClear
	40, // QualityLevelNormal
	10, // QualityLevelPoor
}

// QualityLevelValue 将画质等级转换为 1-100 的具体画质数值.
// 该数值直接传递给 JPEG/WebP 编码器. PNG 编码器内部使用另一套等级.
func QualityLevelValue(level QualityLevel) int {
	idx := int(level)
	if idx < 0 || idx >= len(qualityLevelValues) {
		panic(fmt.Sprintf(strs.CodecErrQualityRange, level))
	}
	return qualityLevelValues[idx]
}

// ParseQualityLevel 根据字符串解析对应的画质等级.
// 支持 "best"/"clear"/"normal"/"poor".
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
		return QualityLevelClear, fmt.Errorf(strs.QualityParseErrFmt, s)
	}
}
