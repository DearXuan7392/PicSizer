package main

import (
	"PicSizer/internal/ui"
	"os"
)

func main() {
	for _, arg := range os.Args[1:] {
		if arg == "-d" || arg == "--debug" {

		}
	}
	ui.Run()
}
