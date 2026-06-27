package main

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/log"
	"PicSizer/internal/ui"
	"os"
)

func main() {
	for _, arg := range os.Args[1:] {
		if arg == "-d" || arg == "--debug" {
			settingLoader.EnableDebug()
		}
	}
	log.InitLogger()
	ui.Run()
}
