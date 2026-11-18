package server

import (
	"PicSizer/internal/core/settingLoader"
	"PicSizer/internal/fileio/codec"
	"PicSizer/internal/log"
	"sync"

	"PicSizer/internal/compress"
	"PicSizer/internal/core"
	"PicSizer/internal/fileio"
)

// ProgressCallback 定义压缩进度回调函数类型.
// current 为成功数, errCount 为失败数, total 为总数.
type ProgressCallback func(current, errCount, total int)

// ItemChangedCallback 定义单项状态变更回调函数类型, 用于通知 UI 刷新行颜色.
type ItemChangedCallback func(item *core.PicItem)

// ThreadPool 管理一组 worker goroutine, 实现并发图片压缩.
// 支持 Start/Wait/Stop 生命周期控制, Start 非阻塞, Wait 阻塞等待完成.
type ThreadPool struct {
	totalNum      int
	currentNum    int
	errorNum      int
	indexOfPic    int
	items         []*core.PicItem
	outputPaths   []string
	mu            sync.Mutex
	wg            sync.WaitGroup
	onProgress    ProgressCallback
	onItemChanged ItemChangedCallback
	onComplete    func(success, errCount, total int)
}

var (
	logger   = log.NewLogger("pool")
	exitFlag = false
)

// NewThreadPool 创建一个线程池实例.
// outputPaths 与 items 一一对应, 为每个项目预先规划的输出路径.
func NewThreadPool(items []*core.PicItem, outputPaths []string, onProgress ProgressCallback, onItemChanged ItemChangedCallback, onComplete func(success, errCount, total int)) *ThreadPool {
	return &ThreadPool{
		items:         items,
		outputPaths:   outputPaths,
		onProgress:    onProgress,
		onItemChanged: onItemChanged,
		onComplete:    onComplete,
	}
}

// Start 启动多线程压缩 (非阻塞).
// 根据配置的 MaxThreads 创建对应数量的 worker goroutine.
// 调用方需在 Start 之后调用 Wait 阻塞等待所有任务完成.
func (tp *ThreadPool) Start() {
	set := settingLoader.GetSetting()
	codec.InitSetting(set)

	tp.totalNum = len(tp.items)
	tp.currentNum = 0
	tp.errorNum = 0
	tp.indexOfPic = 0
	exitFlag = false

	threadCount := set.MaxThreads
	if threadCount <= 0 {
		threadCount = 1
	}
	logger.Info("thread pool start, total: %d, threads: %d", tp.totalNum, threadCount)

	for i := 0; i < threadCount; i++ {
		tp.wg.Add(1)
		go func() {
			defer tp.wg.Done()
			tp.worker()
		}()
	}
}

// Wait 阻塞等待所有 worker 完成, 然后触发 onComplete 回调.
// 必须与 Start 配对使用.
func (tp *ThreadPool) Wait() {
	tp.wg.Wait()

	tp.mu.Lock()
	success, errCount, total := tp.currentNum, tp.errorNum, tp.totalNum
	cb := tp.onComplete
	tp.mu.Unlock()

	logger.Info("thread pool done, success: %d, failed: %d, total: %d", success, errCount, total)

	if cb != nil {
		cb(success, errCount, total)
	}
}

// popTask 从任务队列中取出下一个待处理的项目.
// 若队列为空、已处理完或触发了退出标志, 返回 nil.
func (tp *ThreadPool) popTask() (item *core.PicItem, output string) {
	tp.mu.Lock()

	// 1. 检查全局退出标志
	// 2. 检查是否所有任务都已取完 (Stop 清空队列后 len 为 0, 同样在此返回)
	if exitFlag || tp.indexOfPic >= len(tp.items) {
		tp.mu.Unlock()
		logger.Debug("no task left, index: %d, exit flag: %t", tp.indexOfPic, exitFlag)
		return nil, ""
	}

	item = tp.items[tp.indexOfPic]
	output = tp.outputPaths[tp.indexOfPic]
	tp.indexOfPic++
	tp.mu.Unlock()

	logger.Debug("pop task, index: %d, input: %s, output: %s", tp.indexOfPic-1, item.FullPath, output)
	return item, output
}

// worker 工作线程的循环体, 持续从队列取出任务并执行压缩.
func (tp *ThreadPool) worker() {
	for {
		// 从队列中取出任务
		item, output := tp.popTask()
		if item == nil {
			// 队列为空或收到停止信号, 线程退出
			return
		}

		// 更新状态为压缩中
		item.State = settingLoader.StateCompressing
		// 通知 UI 刷新颜色 (压缩中 → 橙色)
		tp.notifyItemChanged(item)

		// 执行压缩
		result := compress.Compress(item.FullPath, output)

		// 更新结果
		tp.updateResult(item, result, output)
	}
}

// notifyItemChanged 安全地触发单项状态变更回调 (用于 UI 行重绘).
func (tp *ThreadPool) notifyItemChanged(item *core.PicItem) {
	tp.mu.Lock()
	cb := tp.onItemChanged
	tp.mu.Unlock()

	if cb != nil {
		cb(item)
	}
}

// updateResult 更新压缩结果并通知进度回调.
func (tp *ThreadPool) updateResult(item *core.PicItem, result *core.PicResult, outputPath string) {
	tp.mu.Lock()
	if result.CompressResult == core.ResultOk {
		item.State = settingLoader.StateSuccess
		item.OutputPath = outputPath
		// 获取输出文件大小
		if info, err := fileio.GetFileInfo(outputPath); err == nil {
			item.NewSize = info.Size()
		}
		tp.currentNum++
	} else if result.CompressResult == core.ResultOutOfLimit {
		item.State = settingLoader.StateOutOfLimit
		item.Message = result.Message
		tp.errorNum++
	} else {
		item.State = settingLoader.StateError
		item.Message = result.Message
		tp.errorNum++
	}

	logger.Debug("update result, input: %s, state: %s, new size: %d, success: %d, failed: %d, total: %d",
		item.FullPath, settingLoader.PicItemStateName(item.State), item.NewSize,
		tp.currentNum, tp.errorNum, tp.totalNum)

	// 进度回调
	if tp.onProgress != nil {
		tp.onProgress(tp.currentNum, tp.errorNum, tp.totalNum)
	}
	tp.mu.Unlock()

	// 状态更新后, 在锁外通知 UI 刷新行颜色 (成功 → 绿色, 失败 → 红色)
	// 在锁外调用以避免与回调内部的 UI 同步操作产生持锁风险
	tp.notifyItemChanged(item)
}

// Stop 停止压缩流程.
// 设置退出标志并清空任务队列, 正在压缩的任务会自然完成, 后续未启动的任务不再执行.
func (tp *ThreadPool) Stop() {
	tp.mu.Lock()
	defer tp.mu.Unlock()

	logger.Info("thread pool stop requested, remaining: %d", len(tp.items)-tp.indexOfPic)

	exitFlag = true
	// 清空队列
	tp.items = nil
}

// GetStats 获取当前的实时统计数据.
func (tp *ThreadPool) GetStats() (current, errCount, total int) {
	tp.mu.Lock()
	defer tp.mu.Unlock()
	return tp.currentNum, tp.errorNum, tp.totalNum
}
