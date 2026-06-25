package ui

import (
	"PicSizer/internal/core/setting"
	"sort"

	"PicSizer/internal/core"
	"PicSizer/internal/fileio"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
)

// 拖拽选择状态
type dragSelectState struct {
	isDragging bool
	startRow   int
	lastRow    int
}

// PicItemModel 图片列表数据模型
type PicItemModel struct {
	walk.TableModelBase
	items []*core.PicItem
	// onRowChanged 行变更后的可选回调 (供 PicListView 注入, 用于自动滚动跟随)
	// 回调接收已定位到的行号 row, 在 PublishRowChanged 之后同步调用.
	// 注意: 该回调运行在 PublishRowChangedByItem 的调用线程上 (worker 线程),
	// 若回调内部需操作 UI 控件, 应自行通过 walk.Synchronize 切到 UI 线程.
	onRowChanged func(row int)
}

// NewPicItemModel 创建图片列表模型
func NewPicItemModel() *PicItemModel {
	return &PicItemModel{}
}

// SetOnRowChanged 设置行变更回调
//
// 用途: PicListView 通过该回调实现 "状态变更自动滚动" 等基于行号的副作用.
// 传 nil 可清除回调.
func (m *PicItemModel) SetOnRowChanged(cb func(row int)) {
	m.onRowChanged = cb
}

// RowCount 返回行数
func (m *PicItemModel) RowCount() int {
	return len(m.items)
}

// Value 返回指定单元格的值
func (m *PicItemModel) Value(row, col int) interface{} {
	item := m.items[row]
	switch col {
	case 0:
		return item.FileName
	case 1:
		return core.FormatFileSize(item.OrigSize)
	case 2:
		if item.NewSize > 0 {
			return core.FormatFileSize(item.NewSize)
		}
		return ""
	case 3:
		return itemStateToString(item.State)
	}
	return ""
}

// StyleCell 按状态为单元格设置文本颜色 (walk 框架通过 StyleCell 字段识别)
//
// 颜色规则:
//   - 压缩中: 橙色 (255, 140, 0)
//   - 压缩成功: 绿色 (34, 139, 34)
//   - 压缩失败 (超出限制或错误): 红色 (200, 0, 0)
//   - 其它: 默认样式
func (m *PicItemModel) StyleCell(style *walk.CellStyle) {
	if style == nil {
		return
	}
	row := style.Row()
	if row < 0 || row >= len(m.items) {
		return
	}
	item := m.items[row]
	switch item.State {
	case setting.StateCompressing:
		// 橙色, 表示正在压缩
		style.TextColor = walk.RGB(255, 140, 0)
	case setting.StateSuccess:
		// 绿色, 表示压缩成功
		style.TextColor = walk.RGB(34, 139, 34)
	case setting.StateOutOfLimit, setting.StateError:
		// 红色, 表示压缩失败
		style.TextColor = walk.RGB(200, 0, 0)
	}
}

// PublishRowChangedByItem 根据 PicItem 指针定位行, 通知 UI 该行数据已变更 (用于刷新颜色)
//
// 由于压缩过程中只会逐行更新单条记录, 调用 PublishRowsReset 会清空选中状态, 影响交互体验.
// 因此提供按行精确刷新的方式: 通过指针匹配定位行号, 然后发布 RowChanged 事件,
// 让 TableView 仅重绘该行 (保留选中状态), 同时触发该行所有单元格的 StyleCell 回调.
//
// 若模型已注册 onRowChanged 回调 (见 SetOnRowChanged), 会在定位到行后同步调用,
// 便于 PicListView 实现 "状态变更时自动滚动到该行" 等副作用.
func (m *PicItemModel) PublishRowChangedByItem(item *core.PicItem) {
	if item == nil {
		return
	}
	for i, it := range m.items {
		if it == item {
			m.PublishRowChanged(i)
			if m.onRowChanged != nil {
				m.onRowChanged(i)
			}
			return
		}
	}
}

// AddItems 添加图片项目
func (m *PicItemModel) AddItems(items []*core.PicItem) {
	m.items = append(m.items, items...)
	m.PublishRowsReset()
}

// AddItem 添加单个图片项目
func (m *PicItemModel) AddItem(item *core.PicItem) {
	m.items = append(m.items, item)
	m.PublishRowsReset()
}

// RemoveSelected 移除选中的项目
func (m *PicItemModel) RemoveSelected(tv *walk.TableView) {
	indices := tv.SelectedIndexes()
	// 从后往前删除
	sort.Sort(sort.Reverse(sort.IntSlice(indices)))
	for _, idx := range indices {
		if idx >= 0 && idx < len(m.items) {
			m.items = append(m.items[:idx], m.items[idx+1:]...)
		}
	}
	m.PublishRowsReset()
}

// RemoveByState 移除指定状态的项目
func (m *PicItemModel) RemoveByState(state setting.PicItemState) {
	var kept []*core.PicItem
	for _, item := range m.items {
		if item.State != state {
			kept = append(kept, item)
		}
	}
	m.items = kept
	m.PublishRowsReset()
}

// Clear 清空列表
func (m *PicItemModel) Clear() {
	m.items = nil
	m.PublishRowsReset()
}

// GetItems 获取所有项目
func (m *PicItemModel) GetItems() []*core.PicItem {
	return m.items
}

// GetSelectedItems 获取选中的项目
func (m *PicItemModel) GetSelectedItems(tv *walk.TableView) []*core.PicItem {
	indices := tv.SelectedIndexes()
	var selected []*core.PicItem
	for _, idx := range indices {
		if idx >= 0 && idx < len(m.items) {
			selected = append(selected, m.items[idx])
		}
	}
	return selected
}

// SelectAll 全选
func (m *PicItemModel) SelectAll(tv *walk.TableView) {
	var indexes []int
	for i := 0; i < len(m.items); i++ {
		indexes = append(indexes, i)
	}
	tv.SetSelectedIndexes(indexes)
}

// SelectReverse 反选
func (m *PicItemModel) SelectReverse(tv *walk.TableView) {
	selected := make(map[int]bool)
	for _, idx := range tv.SelectedIndexes() {
		selected[idx] = true
	}
	var indexes []int
	for i := 0; i < len(m.items); i++ {
		if !selected[i] {
			indexes = append(indexes, i)
		}
	}
	tv.SetSelectedIndexes(indexes)
}

// ItemCount 获取项目总数
func (m *PicItemModel) ItemCount() int {
	return len(m.items)
}

// ResetAllToWaiting 将所有项目重置为等待状态
//
// 说明:
//   - 清空 NewSize / Message / OutputPath, 避免显示陈旧的压缩结果
//   - 将 State 统一设置为 StateWaiting, 颜色刷新由调用方通过 PublishRowChangedByItem 触发
//   - 内部遍历时直接修改指针指向的 PicItem 字段, 无需重新分配切片
//   - 用于"开始压缩"时统一重置列表, 即便之前已经成功完成的项目也一并重置
func (m *PicItemModel) ResetAllToWaiting() {
	for _, item := range m.items {
		if item == nil {
			continue
		}
		item.NewSize = 0
		item.Message = ""
		item.OutputPath = ""
		item.State = setting.StateWaiting
	}
}

// CountByState 统计指定状态的项目数量
//
// 用于"开始压缩"前判断是否存在已成功完成的项目, 必要时弹出覆盖警告.
func (m *PicItemModel) CountByState(state setting.PicItemState) int {
	count := 0
	for _, item := range m.items {
		if item != nil && item.State == state {
			count++
		}
	}
	return count
}

// itemStateToString 状态转字符串
func itemStateToString(state setting.PicItemState) string {
	switch state {
	case setting.StateWaiting:
		return core.StrStateWaiting
	case setting.StateCompressing:
		return core.StrStateCompressing
	case setting.StateSuccess:
		return core.StrStateSuccess
	case setting.StateOutOfLimit:
		return core.StrStateOutOfLimit
	case setting.StateError:
		return core.StrStateError
	default:
		return ""
	}
}

// PicListView 图片列表视图
type PicListView struct {
	*walk.TableView
	model        *PicItemModel
	dragState    *dragSelectState
	lastClickRow int
	onSelChanged func()
	// autoScrollDown 是否启用 "状态变更自动下滚跟随" 行为
	//   - true:  压缩过程中, 任一行状态变化 (StartCompressing / Success / Error / OutOfLimit)
	//            都会触发单向下滚, 让该行处于可见区域;
	//   - false: 关闭自动滚动, 用户的滚动操作不受影响 (默认值).
	// 仅在 MainForm.onStartCompress 开始时打开, onComplete 关闭, 避免影响用户后续手动滚动.
	autoScrollDown bool
	// maxAutoScrolledRow 已自动滚动到的最大行号 (-1 表示尚未滚动过)
	// 仅当 row > maxAutoScrolledRow 时才触发滚动, 实现 "滑块只能下移, 不能上移" 的行为:
	// 上面行的状态变化不会让滑块回滚, 避免用户定位到列表中下部时被反复拉回顶部.
	maxAutoScrolledRow int
}

// NewPicListView 创建图片列表视图
func NewPicListView() *PicListView {
	model := NewPicItemModel()
	p := &PicListView{
		model:              model,
		dragState:          &dragSelectState{},
		maxAutoScrolledRow: -1,
	}
	// 注入行变更回调, 模型在 PublishRowChangedByItem 定位到行后会回调 onModelRowChanged,
	// 由其实现 "状态变更时自动下滚跟随" 的副作用 (滑块只能下移).
	model.SetOnRowChanged(p.onModelRowChanged)
	return p
}

// ResetAutoScrollDown 启用自动下滚跟随, 并将最大已滚动行号重置为 -1
//
// 用途: MainForm 在 "开始压缩" 时调用, 表示新的一轮压缩开始, 应从顶部重新开始跟踪.
// 本方法会将 maxAutoScrolledRow 重置为 -1, 之后任意行 (包括 0) 的状态变化都会触发下滚.
func (p *PicListView) ResetAutoScrollDown() {
	p.maxAutoScrolledRow = -1
	p.autoScrollDown = true
}

// StopAutoScrollDown 关闭自动下滚跟随
//
// 用途: MainForm 在压缩结束 (全部完成 / 取消) 时调用, 关闭自动滚动以恢复用户对滚动条的手动控制.
// 关闭后, 即便列表中的行状态再变化, 也不会触发自动滚动.
func (p *PicListView) StopAutoScrollDown() {
	p.autoScrollDown = false
}

// onModelRowChanged 处理模型层行变更事件: 触发单向下滚跟随
//
// 行为:
//   - 仅在 autoScrollDown 启用时执行, 避免影响用户手动滚动;
//   - 滑块只能下移: 维护 maxAutoScrolledRow, 仅当 row > maxAutoScrolledRow 时才滚动;
//     即便更上方行的状态变化, 也不会触发上移;
//   - 滚动通过 walk.TableView.EnsureItemVisible 实现, 自动选择最小滚动距离
//     (若目标行已在可见区域内则不滚动);
//   - walk.EnsureItemVisible 必须运行在 UI 线程, 因此通过 TableView.Synchronize
//     把滚动操作切到 UI 消息循环. row 通过局部变量传入, 避免闭包捕获外部变量导致数据竞争.
func (p *PicListView) onModelRowChanged(row int) {
	if !p.autoScrollDown {
		return
	}
	if row <= p.maxAutoScrolledRow {
		return
	}
	p.maxAutoScrolledRow = row
	if p.TableView == nil {
		return
	}
	// 缓存到局部变量, 避免 Synchronize 闭包在并发执行时读取到变更后的 row
	target := row
	p.TableView.Synchronize(func() {
		if p.TableView != nil {
			p.TableView.EnsureItemVisible(target)
		}
	})
}

// PicListViewWidget 返回列表视图控件定义
//
// StyleCell 字段是 walk 框架用于按行/列自定义单元格样式的入口,
// 该字段类型为 func(style *walk.CellStyle), 由 walk 在每次重绘时回调.
func (p *PicListView) PicListViewWidget() declarative.TableView {
	return declarative.TableView{
		AssignTo:         &p.TableView,
		Model:            p.model,
		AlternatingRowBG: true,
		MultiSelection:   true,
		Columns: []declarative.TableViewColumn{
			{Title: core.ColFilename, Width: 200},
			{Title: core.ColOrigSize, Width: 80},
			{Title: core.ColNewSize, Width: 80},
			{Title: core.ColStatus, Width: 80},
		},
		StyleCell:                p.model.StyleCell,
		OnKeyDown:                p.onKeyDown,
		OnSelectedIndexesChanged: p.onSelectionChanged,
	}
}

// GetModel 获取数据模型
func (p *PicListView) GetModel() *PicItemModel {
	return p.model
}

// AddPicturesFromPaths 从路径列表添加图片（自动去重）
func (p *PicListView) AddPicturesFromPaths(paths []string) {
	// 构建已存在路径集合
	existingPaths := make(map[string]bool)
	for _, item := range p.model.items {
		existingPaths[item.FullPath] = true
	}

	var items []*core.PicItem
	for _, path := range paths {
		// 跳过已存在的路径
		if existingPaths[path] {
			continue
		}
		if !fileio.IsImageFile(path) {
			continue
		}
		info, err := fileio.GetFileInfo(path)
		if err != nil {
			continue
		}
		items = append(items, &core.PicItem{
			FullPath: path,
			FileName: fileio.GetFileNameWithoutExt(path) + fileio.GetExtension(path),
			OrigSize: info.Size(),
			State:    setting.StateWaiting,
		})
		existingPaths[path] = true
	}
	if len(items) > 0 {
		p.model.AddItems(items)
	}
}

// AddPicturesFromDirectory 从目录添加图片
func (p *PicListView) AddPicturesFromDirectory(dir string) {
	files, err := fileio.CollectImageFiles(dir)
	if err != nil {
		return
	}
	p.AddPicturesFromPaths(files)
}

// onKeyDown 键盘事件处理
//
// 支持的快捷键:
//   - Ctrl+A: 全选所有项目
//   - Delete: 移除当前选中的项目 (未选中任何项目时不执行任何操作)
func (p *PicListView) onKeyDown(key walk.Key) {
	if p.TableView == nil {
		return
	}
	switch key {
	case walk.KeyA:
		// Ctrl+A 全选
		p.model.SelectAll(p.TableView)
	case walk.KeyDelete:
		// Delete 键快速删除选中的项目
		p.model.RemoveSelected(p.TableView)
		// 删除后通过 onSelChanged 回调刷新底部"已选/总数"标签
		if p.onSelChanged != nil {
			p.onSelChanged()
		}
	}
}

// onSelectionChanged 选择变化事件处理
func (p *PicListView) onSelectionChanged() {
	if p.onSelChanged != nil {
		p.onSelChanged()
	}
}
