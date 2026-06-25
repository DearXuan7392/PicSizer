package server

import (
	"PicSizer/internal/core/setting"
	"sync"

	"PicSizer/internal/compress"
	"PicSizer/internal/core"
	"PicSizer/internal/fileio"
)

// ProgressCallback 进度回调函数类型
type ProgressCallback func(current, errCount, total int)

// ItemChangedCallback 单项状态变更回调函数类型 (用于刷新 UI 行颜色)
type ItemChangedCallback func(item *core.PicItem)

// ThreadPool 线程池
type ThreadPool struct {
	totalNum      int
	currentNum    int
	errorNum      int
	indexOfPic    int
	indexOfOut    int
	items         []*core.PicItem // 待压缩图片队列
	mu            sync.Mutex
	wg            sync.WaitGroup // 用于阻塞等待所有 worker 退出
	onProgress    ProgressCallback
	onItemChanged ItemChangedCallback
	onComplete    func(success, errCount, total int)
}

// NewThreadPool 创建线程池
func NewThreadPool(items []*core.PicItem, onProgress ProgressCallback, onItemChanged ItemChangedCallback, onComplete func(success, errCount, total int)) *ThreadPool {
	return &ThreadPool{
		items:         items,
		onProgress:    onProgress,
		onItemChanged: onItemChanged,
		onComplete:    onComplete,
	}
}

// Start 启动多线程压缩 (非阻塞).
//
// 本方法仅完成以下工作:
//   - 1. 初始化计数器, 重置 core.ExitFlag.
//   - 2. 按 setting.MaxThreads 创建对应数量的 worker goroutine.
//
// 本方法**不会等待**所有 worker 完成, 也不会触发 onComplete 回调.
// 调用方必须在 Start 之后显式调用 Wait 阻塞等待, 否则 worker 可能仍在运行时
// 调用方就已经退出函数作用域, 导致压缩被异常中断.
func (tp *ThreadPool) Start() {
	setting := setting.GetSetting()

	tp.mu.Lock()
	tp.totalNum = len(tp.items)
	tp.currentNum = 0
	tp.errorNum = 0
	tp.indexOfPic = 0
	tp.indexOfOut = setting.StartIndex
	core.ExitFlag = false
	tp.mu.Unlock()

	// 创建工作线程
	threadCount := setting.MaxThreads
	if threadCount <= 0 {
		threadCount = 1
	}

	for i := 0; i < threadCount; i++ {
		tp.wg.Add(1)
		go func() {
			defer tp.wg.Done()
			tp.worker()
		}()
	}
}

// Wait 阻塞等待所有 worker 完成, 然后触发 onComplete 回调.
//
// 本方法必须与 Start 配对使用: Start 启动 worker 之后, 调用方通常需要阻塞等待
// 全部 worker 退出 (例如 CLI 模式下要等到进度条结束并打印汇总, GUI 模式下要等到
// 用户在窗体内继续点击/操作). 阻塞期间可由其他线程通过 Stop 触发提前退出.
//
// 完成后会在锁内读取 currentNum / errorNum / totalNum 并调用 onComplete,
// 保证回调拿到的统计数据是同一时刻的一致快照.
func (tp *ThreadPool) Wait() {
	tp.wg.Wait()

	// 读取最终统计, 与 onComplete 内的判定保持一致, 避免在锁外读取造成的撕裂值
	tp.mu.Lock()
	success, errCount, total := tp.currentNum, tp.errorNum, tp.totalNum
	cb := tp.onComplete
	tp.mu.Unlock()

	if cb != nil {
		cb(success, errCount, total)
	}
}

// popTask 定义显式的取出函数，由子线程并发调用
// 内部加锁保证线程安全，若队列为空、已处理完或触发退出，则返回 nil, "", ""
func (tp *ThreadPool) popTask() (item *core.PicItem, input string, output string) {
	tp.mu.Lock()

	// 1. 检查全局退出标志
	// 2. 检查队列是否已被清空 (Stop 调用)
	// 3. 检查是否所有任务都已取完
	if core.ExitFlag || tp.items == nil || tp.indexOfPic >= len(tp.items) {
		tp.mu.Unlock()
		return nil, "", ""
	}

	itemIndex := tp.indexOfPic
	outputIndex := tp.indexOfOut
	tp.mu.Unlock()
	tp.indexOfPic++
	tp.indexOfOut++

	item = tp.items[itemIndex]
	input = item.FullPath
	output = fileio.GetOutputPath(input, outputIndex)
	return item, input, output
}

// worker 工作线程
func (tp *ThreadPool) worker() {
	for {
		// 从队列中取出任务 (锁在 popTask 内部安全释放)
		item, input, output := tp.popTask()
		if item == nil {
			return // 队列为空或收到停止信号，线程退出
		}

		// 更新状态为压缩中
		item.State = setting.StateCompressing
		// 通知 UI 刷新颜色 (压缩中 → 橙色)
		tp.notifyItemChanged(item)

		// 执行压缩 (阻塞调用，在锁外执行，不阻塞其他工作线程取任务)
		result := compress.Compress(input, output)

		// 更新结果
		tp.updateResult(item, result, output)
	}
}

// notifyItemChanged 安全地触发单项状态变更回调 (用于 UI 行重绘)
//
// 说明:
//   - 回调运行在 worker 线程中, 内部不应持有 tp.mu 锁.
//   - 回调内部通常仅触发 UI 同步刷新 (Synchronize + PublishRowsChanged), 不会有阻塞操作.
func (tp *ThreadPool) notifyItemChanged(item *core.PicItem) {
	tp.mu.Lock()
	cb := tp.onItemChanged
	tp.mu.Unlock()

	if cb != nil {
		cb(item)
	}
}

// updateResult 更新压缩结果
func (tp *ThreadPool) updateResult(item *core.PicItem, result *core.PicResult, outputPath string) {
	tp.mu.Lock()
	if result.CompressResult == core.ResultOk {
		item.State = setting.StateSuccess
		item.OutputPath = outputPath
		// 获取输出文件大小
		if info, err := fileio.GetFileInfo(outputPath); err == nil {
			item.NewSize = info.Size()
		}
		tp.currentNum++
	} else if result.CompressResult == core.ResultOutOfLimit {
		item.State = setting.StateOutOfLimit
		item.Message = result.Message
		tp.errorNum++
	} else {
		item.State = setting.StateError
		item.Message = result.Message
		tp.errorNum++
	}

	// 进度回调 (留在锁内调用, 与原行为保持一致)
	if tp.onProgress != nil {
		tp.onProgress(tp.currentNum, tp.errorNum, tp.totalNum)
	}
	tp.mu.Unlock()

	// 状态更新后, 在锁外通知 UI 刷新行颜色 (成功 → 绿色, 失败 → 红色)
	// 在锁外调用以避免与回调内部的 UI 同步操作产生持锁风险
	tp.notifyItemChanged(item)
}

// GetStats 获取统计信息
func (tp *ThreadPool) GetStats() (current, errCount, total int) {
	tp.mu.Lock()
	defer tp.mu.Unlock()
	return tp.currentNum, tp.errorNum, tp.totalNum
}

// Stop 停止压缩
func (tp *ThreadPool) Stop() {
	tp.mu.Lock()
	defer tp.mu.Unlock()

	core.ExitFlag = true
	// 直接清空队列，这样后续 popTask 会直接返回 nil，子线程立刻安全退出
	tp.items = nil
}
