package codec

import (
	"errors"
	"image"
	"os"
	"sync"
)

// codecRegistry 编解码器注册表
var (
	registry   []ImageCodec
	registryMu sync.RWMutex
)

// RegisterCodec 注册图片编解码器
// 编解码器按注册顺序进行类型判断, 先注册的优先匹配
func RegisterCodec(codec ImageCodec) {
	registryMu.Lock()
	defer registryMu.Unlock()
	registry = append(registry, codec)
}

// GetCodec 根据图片数据获取匹配的编解码器
// 遍历注册表, 返回第一个匹配的编解码器
func GetCodec(data []byte) ImageCodec {
	registryMu.RLock()
	defer registryMu.RUnlock()

	for _, codec := range registry {
		if codec.IsType(data) {
			return codec
		}
	}
	return nil
}

// init 初始化时注册所有支持的图片格式编解码器
func init() {
	// 按优先级注册: WebP 优先 (避免 RIFF 头被误判), 然后 PNG, 最后 JPEG
	RegisterCodec(&webpCodec{})
	RegisterCodec(&pngCodec{})
	RegisterCodec(&jpegCodec{})
}

// LoadImage 加载图片文件, 返回 image.Image
// 自动检测图片格式并使用对应的编解码器进行解码
func LoadImage(imgPath string) (image.Image, error) {
	data, err := os.ReadFile(imgPath)
	if err != nil {
		return nil, err
	}

	// 获取匹配的编解码器
	codec := GetCodec(data)
	if codec == nil {
		return nil, errors.New("不支持的图片格式")
	}

	// 解码图片
	img, err := codec.Decode(data)
	if err != nil {
		return nil, err
	}

	return img, nil
}
