// Package log 提供调试模式下使用的异步日志.
//
// 日志行格式固定为:
//
//	2006-01-02 15:04:05.000 [TID:42] [INFO ] [compress] message (compressor.go:31)
//
// 字段含义依次为: 时间, 产生日志的 goroutine id (等价于本程序的线程 id),
// 级别 (定宽 5), 模块名, 消息, 调用位置 (文件:行号).
//
// 消息统一使用英文, 采用 "action, key: value, key: value" 形式, 不带结尾句号.
//
// 设计要点:
//   - 未启用调试时所有写入在入口处直接返回, 不产生格式化与调用栈开销.
//   - 生产者非阻塞投递, 队列满时丢弃并计数, 避免日志反向阻塞压缩 worker.
//   - 单消费者协程按批写入带缓冲的文件, 队列排空时立即冲刷.
//   - Close 关闭消费者并落盘, 可重复调用; 未启用时为空操作.
package log

import (
	"PicSizer/internal/core/settingLoader"
	strs "PicSizer/internal/core/strings"
	"PicSizer/internal/dialog"
	"bufio"
	"fmt"
	"os"
	"path/filepath"
	"runtime"
	"strconv"
	"strings"
	"sync"
	"sync/atomic"
	"time"
)

// Level 日志级别, 数值越小越详细.
type Level int

const (
	LevelDebug Level = iota
	LevelInfo
	LevelWarn
	LevelError
)

// String 返回级别名称, 定宽 5 个字符以保证日志列对齐.
func (l Level) String() string {
	switch l {
	case LevelDebug:
		return "DEBUG"
	case LevelInfo:
		return "INFO "
	case LevelWarn:
		return "WARN "
	case LevelError:
		return "ERROR"
	default:
		return "?????"
	}
}

const (
	queueSize    = 8192
	writeBufSize = 64 * 1024
)

type record struct {
	ts     time.Time
	level  Level
	tid    int64
	module string
	msg    string
	caller string
}

// Logger 一个模块的日志入口, 由各包在包级变量中注册.
type Logger struct {
	module string
}

var (
	enabled atomic.Bool
	stopped atomic.Bool
	dropped atomic.Uint64

	queue    chan record
	stopCh   chan struct{}
	doneCh   chan struct{}
	stopOnce sync.Once
	writer   *bufio.Writer
	logFile  *os.File
)

// InitLogger 初始化全局日志. 仅在调试模式下真正生效, 否则后续所有写入均为空操作.
func InitLogger() {
	if !settingLoader.IsDebug() {
		return
	}

	logPath := fmt.Sprintf("PicSizerLog_%s.log", time.Now().Format("20060102_150405"))
	if err := os.MkdirAll(filepath.Dir(logPath), 0755); err != nil {
		dialog.ShowErrorWithParent(0, strs.ErrCannotCreateLogFile)
		os.Exit(1)
	}

	file, err := os.OpenFile(logPath, os.O_CREATE|os.O_WRONLY|os.O_APPEND, 0644)
	if err != nil {
		dialog.ShowErrorWithParent(0, strs.ErrCannotOpenLogFile)
		os.Exit(1)
	}

	logFile = file
	writer = bufio.NewWriterSize(file, writeBufSize)
	queue = make(chan record, queueSize)
	stopCh = make(chan struct{})
	doneCh = make(chan struct{})

	enabled.Store(true)
	go consume()

	NewLogger("log").Info("logger start, path: %s, app: %s %s", logPath, strs.AppName, strs.AppVersion)
}

// Enabled 返回日志是否已启用, 供调用方决定是否构造昂贵的日志参数.
func Enabled() bool {
	return enabled.Load()
}

// Close 停止消费者并落盘. 可重复调用; 日志未启用时立即返回.
func Close() {
	if !enabled.Load() {
		return
	}
	stopOnce.Do(func() {
		stopped.Store(true)
		close(stopCh)
		<-doneCh
		_ = writer.Flush()
		_ = logFile.Close()
	})
}

func NewLogger(module string) *Logger {
	return &Logger{module: module}
}

// consume 单消费者协程: 依次取出队列内容写入文件, 收到停止信号后排空队列再退出.
func consume() {
	defer close(doneCh)
	for {
		select {
		case r := <-queue:
			writeRecord(r)
		case <-stopCh:
			for {
				select {
				case r := <-queue:
					writeRecord(r)
				default:
					return
				}
			}
		}
	}
}

// writeRecord 格式化并写入一条日志, 队列排空时立即冲刷缓冲区.
func writeRecord(r record) {
	line := fmt.Sprintf("%s [TID:%d] [%s] [%s] %s (%s)\n",
		r.ts.Format("2006-01-02 15:04:05.000"), r.tid, r.level.String(), r.module, r.msg, r.caller)
	_, _ = writer.WriteString(line)

	if dropped.Load() > 0 {
		n := dropped.Swap(0)
		overflow := fmt.Sprintf("%s [TID:%d] [WARN ] [log] queue overflow, dropped: %d (%s)\n",
			time.Now().Format("2006-01-02 15:04:05.000"), r.tid, n, "logger.go")
		_, _ = writer.WriteString(overflow)
	}

	if len(queue) == 0 {
		_ = writer.Flush()
	}
}

// getTID 返回当前 goroutine 的 id, 用于在并发场景下区分日志来源.
func getTID() int64 {
	var buf [64]byte
	n := runtime.Stack(buf[:], false)
	field := strings.Fields(strings.TrimPrefix(string(buf[:n]), "goroutine "))
	if len(field) == 0 {
		return 0
	}
	id, err := strconv.ParseInt(field[0], 10, 64)
	if err != nil {
		return 0
	}
	return id
}

// push 组装日志记录并投递到队列. 队列满时丢弃, 不阻塞调用方.
func (l *Logger) push(level Level, format string, v ...interface{}) {
	if !enabled.Load() || stopped.Load() {
		return
	}

	caller := "unknown"
	if _, file, line, ok := runtime.Caller(2); ok {
		caller = fmt.Sprintf("%s:%d", filepath.Base(file), line)
	}

	r := record{
		ts:     time.Now(),
		level:  level,
		tid:    getTID(),
		module: l.module,
		msg:    fmt.Sprintf(format, v...),
		caller: caller,
	}

	select {
	case queue <- r:
	default:
		dropped.Add(1)
	}
}

// Debug 输出 debug 级别日志, 用于记录内部流程与中间状态.
func (l *Logger) Debug(format string, v ...interface{}) {
	l.push(LevelDebug, format, v...)
}

// Info 输出 info 级别日志, 用于记录用户操作与阶段性结果.
func (l *Logger) Info(format string, v ...interface{}) {
	l.push(LevelInfo, format, v...)
}

// Warn 输出 warn 级别日志, 用于记录可恢复的异常与降级行为.
func (l *Logger) Warn(format string, v ...interface{}) {
	l.push(LevelWarn, format, v...)
}

// Error 输出 error 级别日志, 用于记录失败原因.
func (l *Logger) Error(format string, v ...interface{}) {
	l.push(LevelError, format, v...)
}
