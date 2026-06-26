package logger

import (
	"PicSizer/internal/core/settingLoader"
	strs "PicSizer/internal/core/strings"
	"PicSizer/internal/dialog"
	"fmt"
	"os"
	"path/filepath"
	"runtime"
	"strconv"
	"strings"
	"time"
)

// LogLevel 定义日志级别
type LogLevel string

const (
	logPath = "./PicSizerLog.log"

	LevelInfo  LogLevel = "INFO"
	LevelWarn  LogLevel = "WARN"
	LevelError LogLevel = "ERROR"
)

var (
	logEnable = false
)

// logMessage 队列中传输的日志结构体
type logMessage struct {
	time      time.Time
	level     LogLevel
	goid      int64
	className string
	msg       string
}

// AsyncLogger 异步日志器类
type AsyncLogger struct {
	className string // 注册的类名/模块名
}

// 全局单例管理后台写入
var (
	logQueue     = make(chan logMessage, 10000) // 日志缓冲区队列
	logFile      *os.File
	stopConsumer = make(chan struct{})
)

// InitLogger 初始化全局日志配置 (静态函数/单例初始化)
func InitLogger() {
	logEnable = settingLoader.IsDebug()
	if logEnable {
		logEnable = true

		dir := filepath.Dir(logPath)
		if err := os.MkdirAll(dir, 0755); err != nil {
			dialog.ShowErrorWithParent(0, strs.ErrCannotCreateLogFile)
			os.Exit(1)
		}

		file, err := os.OpenFile(logPath, os.O_CREATE|os.O_WRONLY|os.O_APPEND, 0644)
		if err != nil {
			dialog.ShowErrorWithParent(0, strs.ErrCannotOpenLogFile)
			os.Exit(1)
		}
		logFile = file

		// 启动消费者后台协程
		go startConsumer()
	}
}

func Get() {

}

// CloseLogger 关闭日志，等待队列消费完
func CloseLogger() {
	close(logQueue) // 关闭通道，消费者读完后会自动退出
	<-stopConsumer  // 等待消费者完全退出
	if logFile != nil {
		logFile.Close()
	}
}

// NewLogger 注册一个新的 Logger (每段代码/结构体自己注册)
func NewLogger(className string) *AsyncLogger {
	return &AsyncLogger{className: className}
}

// 后台消费者：将队列内容拼接写入文件
func startConsumer() {
	defer close(stopConsumer)
	for item := range logQueue {
		// 格式化：时间 [线程ID] [类名] [级别] 日志内容
		timeStr := item.time.Format("2006-01-02 15:04:05.000")
		line := fmt.Sprintf("%s [Goid:%d] [%s] [%s] %s\n",
			timeStr, item.goid, item.className, item.level, item.msg)

		// 写入文件 (如果未初始化则打印到控制台)
		if logFile != nil {
			_, _ = logFile.WriteString(line)
		} else {
			fmt.Print(line)
		}
	}
}

// 辅助函数：获取当前 Goroutine 的 ID (模拟线程ID)
func getGID() int64 {
	var buf [64]byte
	n := runtime.Stack(buf[:], false)
	idField := strings.Fields(strings.TrimPrefix(string(buf[:n]), "goroutine "))[0]
	id, err := strconv.ParseInt(idField, 10, 64)
	if err != nil {
		return 0
	}
	return id
}

// 生产者核心逻辑
func (l *AsyncLogger) pushLog(level LogLevel, format string, v ...interface{}) {
	msg := fmt.Sprintf(format, v...)
	logQueue <- logMessage{
		time:      time.Now(),
		level:     level,
		goid:      getGID(),
		className: l.className,
		msg:       msg,
	}
}

// Info 打印 info 级别日志
func (l *AsyncLogger) Info(format string, v ...interface{}) {
	l.pushLog(LevelInfo, format, v...)
}

// Warn 打印 warn 级别日志
func (l *AsyncLogger) Warn(format string, v ...interface{}) {
	l.pushLog(LevelWarn, format, v...)
}

// Error 打印 error 级别日志
func (l *AsyncLogger) Error(format string, v ...interface{}) {
	l.pushLog(LevelError, format, v...)
}
