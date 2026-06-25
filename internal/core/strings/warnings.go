package strs

// 警告提示
var (
	WarnCompressRunning    = "已有压缩任务正在进行, 请等待完成或取消后再开始新的任务"
	WarnOverwriteDoneFmt   = "列表中有 %d 张已完成 (成功) 的图片, 重新压缩将覆盖之前的输出文件. \n可通过 \"菜单栏\" - \"" + TextEdit + "\" - \"" + TextRemove + "\" - \"" + TextRemoveSelect + "\" 来移除已完成的任务. \n\n是否仍要继续?"
	WarnFilenameTplMissing = "文件名模板未包含 {id} 或 {name}, 多张图片可能因重名而被覆盖. \n是否仍要保存当前设置?"
)
