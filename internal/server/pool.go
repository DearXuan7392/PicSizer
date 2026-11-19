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

// ThreadPool 管理一组 worker goroutine 并发压缩图片.
// 生命周期: Start 创建 worker (非阻塞), Wait 阻塞等待全部完成, Stop 中止未开始的任务.
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
	// exitFlag 包级退出标志. 程序同一时刻只运行一个压缩批次,
	// 由 Start 重置、Stop 置位, worker 借此感知取消.
	exitFlag = false
)

// NewThreadPool 创建线程池. outputPaths 与 items 一一对应, 为每项预规划的输出路径.
func NewThreadPool(items []*core.PicItem, outputPaths []string, onProgress ProgressCallback, onItemChanged ItemChangedCallback, onComplete func(success, errCount, total int)) *ThreadPool {
	return &ThreadPool{
		items:         items,
		outputPaths:   outputPaths,
		onProgress:    onProgress,
		onItemChanged: onItemChanged,
		onComplete:    onComplete,
	}
}

// Start 启动压缩 (非阻塞), 按 MaxThreads 创建 worker goroutine.
// 调用方需随后调用 Wait 等待完成并触发 onComplete.
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

// Wait 阻塞等待所有 worker 完成, 随后触发 onComplete 回调.
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

// popTask 取出下一个待处理项目.
// 队列为空、已取完或触发退出标志时返回 nil.
func (tp *ThreadPool) popTask() (item *core.PicItem, output string) {
	tp.mu.Lock()

	// Stop 通过置 items=nil 清空队列, 该场景同样收敛到 indexOfPic >= len(items)
	// (len 为 0), 因此只需一次判断.
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

func (tp *ThreadPool) worker() {
	for {
		item, output := tp.popTask()
		if item == nil {
			return
		}

		item.State = settingLoader.StateCompressing
		tp.notifyItemChanged(item)

		result := compress.Compress(item.FullPath, output)

		tp.updateResult(item, result, output)
	}
}

// notifyItemChanged 先在锁内取出回调引用, 再在锁外执行.
// 回调内部的 UI 同步 (Synchronize) 会等待 UI 线程,
// 若 UI 线程此刻正尝试进入本池的锁 (如 Stop), 持锁调用会死锁.
func (tp *ThreadPool) notifyItemChanged(item *core.PicItem) {
	tp.mu.Lock()
	cb := tp.onItemChanged
	tp.mu.Unlock()

	if cb != nil {
		cb(item)
	}
}

// updateResult 记录单项压缩结果并触发进度回调.
func (tp *ThreadPool) updateResult(item *core.PicItem, result *core.PicResult, outputPath string) {
	tp.mu.Lock()
	if result.CompressResult == core.ResultOk {
		item.State = settingLoader.StateSuccess
		item.OutputPath = outputPath
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

	if tp.onProgress != nil {
		tp.onProgress(tp.currentNum, tp.errorNum, tp.totalNum)
	}
	tp.mu.Unlock()

	// notifyItemChanged 在锁外执行, 理由见其函数注释.
	tp.notifyItemChanged(item)
}

// Stop 中止压缩流程: 置退出标志并清空队列.
// 正在压缩的任务会自然完成, 未开始的任务不再执行.
func (tp *ThreadPool) Stop() {
	tp.mu.Lock()
	defer tp.mu.Unlock()

	logger.Info("thread pool stop requested, remaining: %d", len(tp.items)-tp.indexOfPic)

	exitFlag = true
	tp.items = nil
}

func (tp *ThreadPool) GetStats() (current, errCount, total int) {
	tp.mu.Lock()
	defer tp.mu.Unlock()
	return tp.currentNum, tp.errorNum, tp.totalNum
}
