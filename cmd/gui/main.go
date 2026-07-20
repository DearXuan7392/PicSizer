package main

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/log"
	"PicSizer/internal/ui"
	"os"
)

func main() {
	// 分离调试标志和文件路径
	var paths []string
	for _, arg := range os.Args[1:] {
		if arg == "-d" || arg == "--debug" {
			settingLoader.EnableDebug()
		} else {
			paths = append(paths, arg)
		}
	}
	log.InitLogger()
	ui.Run(paths)
}
