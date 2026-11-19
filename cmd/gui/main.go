package main

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/log"
	"PicSizer/internal/ui"
	"os"
)

func main() {
	var paths []string
	for _, arg := range os.Args[1:] {
		if arg == "-d" || arg == "--debug" {
			settingLoader.EnableDebug()
		} else {
			paths = append(paths, arg)
		}
	}
	log.InitLogger()

	logger := log.NewLogger("main")
	logger.Info("process start, debug: %t, args: %v", settingLoader.IsDebug(), os.Args)

	ui.Run(paths)

	logger.Info("process exit")
	log.Close()
}
