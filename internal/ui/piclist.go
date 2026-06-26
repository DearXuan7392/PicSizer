package ui

import (
	"PicSizer/internal/core/settingLoader"
	"sort"
	"syscall"

	"PicSizer/internal/core"
	strs "PicSizer/internal/core/strings"
	"PicSizer/internal/fileio"

	"github.com/lxn/walk"
	"github.com/lxn/walk/declarative"
	"github.com/lxn/win"
)

type dragSelectState struct {
	isDragging bool
	startRow   int
	lastRow    int
}

// PicItemModel 实现 walk.TableModelBase, 作为图片列表的数据模型.
// 管理 PicItem 列表, 提供行、列数据访问和状态控制能力.
type PicItemModel struct {
	walk.TableModelBase
	items        []*core.PicItem
	onRowChanged func(row int)
}

// NewPicItemModel 创建 PicItemModel 实例.
func NewPicItemModel() *PicItemModel {
	return &PicItemModel{}
}

// SetOnRowChanged 设置行变更回调, 用于实现自动滚动跟随等副作用.
func (m *PicItemModel) SetOnRowChanged(cb func(row int)) {
	m.onRowChanged = cb
}

// RowCount 返回列表中的项目总数.
func (m *PicItemModel) RowCount() int {
	return len(m.items)
}

// Value 返回指定行列的单元格显示值.
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
	case settingLoader.StateCompressing:
		// 橙色, 表示正在压缩
		style.TextColor = walk.RGB(255, 140, 0)
	case settingLoader.StateSuccess:
		// 绿色, 表示压缩成功
		style.TextColor = walk.RGB(34, 139, 34)
	case settingLoader.StateOutOfLimit, settingLoader.StateError:
		// 红色, 表示压缩失败
		style.TextColor = walk.RGB(200, 0, 0)
	}
}

// PublishRowChangedByItem 根据 PicItem 指针定位行, 精确通知 UI 该行数据已变更.
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

// AddItems 批量添加图片项目到列表.
func (m *PicItemModel) AddItems(items []*core.PicItem) {
	m.items = append(m.items, items...)
	m.PublishRowsReset()
}

// AddItem 添加单个图片项目到列表.
func (m *PicItemModel) AddItem(item *core.PicItem) {
	m.items = append(m.items, item)
	m.PublishRowsReset()
}

// RemoveSelected 移除当前选中的项目.
func (m *PicItemModel) RemoveSelected(tv *walk.TableView) {
	indices := tv.SelectedIndexes()
	sort.Sort(sort.Reverse(sort.IntSlice(indices)))
	for _, idx := range indices {
		if idx >= 0 && idx < len(m.items) {
			m.items = append(m.items[:idx], m.items[idx+1:]...)
		}
	}
	m.PublishRowsReset()
}

// RemoveByState 移除列表中指定状态的所有项目.
func (m *PicItemModel) RemoveByState(state settingLoader.PicItemState) {
	var kept []*core.PicItem
	for _, item := range m.items {
		if item.State != state {
			kept = append(kept, item)
		}
	}
	m.items = kept
	m.PublishRowsReset()
}

// Clear 清空列表中的所有项目.
func (m *PicItemModel) Clear() {
	m.items = nil
	m.PublishRowsReset()
}

// GetItems 返回列表中的所有项目.
func (m *PicItemModel) GetItems() []*core.PicItem {
	return m.items
}

// GetSelectedItems 返回当前选中的项目列表.
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

// SelectAll 选中列表中的所有项目.
func (m *PicItemModel) SelectAll(tv *walk.TableView) {
	var indexes []int
	for i := 0; i < len(m.items); i++ {
		indexes = append(indexes, i)
	}
	tv.SetSelectedIndexes(indexes)
}

// SelectReverse 反选列表中的项目.
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

// ItemCount 返回列表中的项目总数.
func (m *PicItemModel) ItemCount() int {
	return len(m.items)
}

// ResetAllToWaiting 将所有项目重置为等待状态, 清空压缩结果字段.
func (m *PicItemModel) ResetAllToWaiting() {
	for _, item := range m.items {
		if item == nil {
			continue
		}
		item.NewSize = 0
		item.Message = ""
		item.OutputPath = ""
		item.State = settingLoader.StateWaiting
	}
}

// CountByState 统计列表中指定状态的项目数量.
func (m *PicItemModel) CountByState(state settingLoader.PicItemState) int {
	count := 0
	for _, item := range m.items {
		if item != nil && item.State == state {
			count++
		}
	}
	return count
}

func itemStateToString(state settingLoader.PicItemState) string {
	switch state {
	case settingLoader.StateWaiting:
		return strs.StrStateWaiting
	case settingLoader.StateCompressing:
		return strs.StrStateCompressing
	case settingLoader.StateSuccess:
		return strs.StrStateSuccess
	case settingLoader.StateOutOfLimit:
		return strs.StrStateOutOfLimit
	case settingLoader.StateError:
		return strs.StrStateError
	default:
		return ""
	}
}

// PicListView 封装列表视图控件, 提供图片添加、状态管理和自动滚动等功能.
type PicListView struct {
	*walk.TableView
	model              *PicItemModel
	dragState          *dragSelectState
	lastClickRow       int
	onSelChanged       func()
	autoScrollDown     bool
	maxAutoScrolledRow int
	appIcon            *walk.Icon

	// 右键菜单项 (用于动态启用/禁用)
	actionOpenOrig     *walk.Action
	actionShowExplorer *walk.Action
	actionOpenComp     *walk.Action
	actionShowComp     *walk.Action
	actionViewError    *walk.Action
}

// NewPicListView 创建 PicListView 实例.
func NewPicListView(appIcon *walk.Icon) *PicListView {
	model := NewPicItemModel()
	p := &PicListView{
		model:              model,
		dragState:          &dragSelectState{},
		maxAutoScrolledRow: -1,
		appIcon:            appIcon,
	}
	model.SetOnRowChanged(p.onModelRowChanged)
	return p
}

// ResetAutoScrollDown 启用自动下滚跟随, 重置最大已滚动行号为 -1.
func (p *PicListView) ResetAutoScrollDown() {
	p.maxAutoScrolledRow = -1
	p.autoScrollDown = true
}

// StopAutoScrollDown 关闭自动下滚跟随.
func (p *PicListView) StopAutoScrollDown() {
	p.autoScrollDown = false
}

// onModelRowChanged 处理模型层行变更事件, 实现单向下滚跟随.
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
	target := row
	p.TableView.Synchronize(func() {
		if p.TableView != nil {
			p.TableView.EnsureItemVisible(target)
		}
	})
}

// PicListViewWidget 返回列表视图的声明式控件定义.
func (p *PicListView) PicListViewWidget() declarative.TableView {
	return declarative.TableView{
		AssignTo:         &p.TableView,
		Model:            p.model,
		AlternatingRowBG: true,
		MultiSelection:   true,
		Columns: []declarative.TableViewColumn{
			{Title: strs.ColFilename, Width: 200},
			{Title: strs.ColOrigSize, Width: 80},
			{Title: strs.ColNewSize, Width: 80},
			{Title: strs.ColStatus, Width: 80},
		},
		StyleCell:                p.model.StyleCell,
		OnKeyDown:                p.onKeyDown,
		OnMouseDown:              p.onMouseDown,
		OnSelectedIndexesChanged: p.onSelectionChanged,
		ContextMenuItems: []declarative.MenuItem{
			declarative.Action{
				AssignTo:    &p.actionOpenOrig,
				Text:        strs.CtxOpenOriginal,
				OnTriggered: p.onOpenOriginal,
			},
			declarative.Action{
				AssignTo:    &p.actionShowExplorer,
				Text:        strs.CtxShowInExplorer,
				OnTriggered: p.onShowInExplorer,
			},
			declarative.Separator{},
			declarative.Action{
				AssignTo:    &p.actionOpenComp,
				Text:        strs.CtxOpenCompressed,
				OnTriggered: p.onOpenCompressed,
			},
			declarative.Action{
				AssignTo:    &p.actionShowComp,
				Text:        strs.CtxShowCompInExplorer,
				OnTriggered: p.onShowCompInExplorer,
			},
			declarative.Separator{},
			declarative.Action{
				AssignTo:    &p.actionViewError,
				Text:        strs.CtxViewError,
				OnTriggered: p.onViewError,
			},
		},
	}
}

// GetModel 返回内部的数据模型.
func (p *PicListView) GetModel() *PicItemModel {
	return p.model
}

// AddPicturesFromPaths 从文件路径列表添加图片 (自动去重).
func (p *PicListView) AddPicturesFromPaths(paths []string) {
	existingPaths := make(map[string]bool)
	for _, item := range p.model.items {
		existingPaths[item.FullPath] = true
	}

	var items []*core.PicItem
	for _, path := range paths {
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
			State:    settingLoader.StateWaiting,
		})
		existingPaths[path] = true
	}
	if len(items) > 0 {
		p.model.AddItems(items)
	}
}

// AddPicturesFromDirectory 从目录递归添加所有图片文件到列表.
func (p *PicListView) AddPicturesFromDirectory(dir string) {
	files, err := fileio.CollectImageFiles(dir)
	if err != nil {
		return
	}
	p.AddPicturesFromPaths(files)
}

// onMouseDown 处理鼠标按下事件, 右键时尝试选中鼠标所在位置的项目.
func (p *PicListView) onMouseDown(x, y int, button walk.MouseButton) {
	if p.TableView == nil {
		return
	}
	if button != walk.RightButton {
		return
	}

	// 获取鼠标位置对应的行索引
	row := p.TableView.IndexAt(x, y)
	if row >= 0 && row < p.model.ItemCount() {
		// 选中该行 (单选)
		p.TableView.SetSelectedIndexes([]int{row})
	} else {
		// 点击空白区域, 清空选中
		p.TableView.SetSelectedIndexes(nil)
	}
}

// onKeyDown 处理键盘事件, 支持 Ctrl+A 全选和 Delete 删除.
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
		// 删除后更新右键菜单状态
		p.updateContextMenuState()
	}
}

// onSelectionChanged 响应选择变化事件, 同时更新右键菜单项的启用状态.
func (p *PicListView) onSelectionChanged() {
	if p.onSelChanged != nil {
		p.onSelChanged()
	}
	p.updateContextMenuState()
}

// updateContextMenuState 根据当前选中项的属性, 更新右键菜单项的启用/禁用状态.
func (p *PicListView) updateContextMenuState() {
	if p.TableView == nil {
		return
	}
	selected := p.model.GetSelectedItems(p.TableView)

	// 未选中任何项目时, 所有菜单项禁用
	hasSelection := len(selected) > 0
	hasCompressed := false
	hasError := false

	for _, item := range selected {
		if item.State == settingLoader.StateSuccess && item.OutputPath != "" {
			hasCompressed = true
		}
		if item.State == settingLoader.StateError || item.State == settingLoader.StateOutOfLimit {
			if item.Message != "" {
				hasError = true
			}
		}
	}

	if p.actionOpenOrig != nil {
		p.actionOpenOrig.SetEnabled(hasSelection)
	}
	if p.actionShowExplorer != nil {
		p.actionShowExplorer.SetEnabled(hasSelection)
	}
	if p.actionOpenComp != nil {
		p.actionOpenComp.SetEnabled(hasCompressed)
	}
	if p.actionShowComp != nil {
		p.actionShowComp.SetEnabled(hasCompressed)
	}
	if p.actionViewError != nil {
		p.actionViewError.SetEnabled(hasError)
	}
}

// getSingleSelectedItem 获取当前选中的唯一项目, 如果未选中或选中多个则返回 nil.
func (p *PicListView) getSingleSelectedItem() *core.PicItem {
	if p.TableView == nil {
		return nil
	}
	selected := p.model.GetSelectedItems(p.TableView)
	if len(selected) != 1 {
		return nil
	}
	return selected[0]
}

// shellOpen 调用 Windows ShellExecute 打开文件或执行操作.
func shellOpen(operation, file string) {
	verbPtr, _ := syscall.UTF16PtrFromString(operation)
	filePtr, _ := syscall.UTF16PtrFromString(file)
	win.ShellExecute(0, verbPtr, filePtr, nil, nil, win.SW_SHOWNORMAL)
}

// shellOpenFolderAndSelect 在资源管理器中打开文件夹并选中指定文件.
func shellOpenFolderAndSelect(filePath string) {
	// 使用 explorer /select 命令
	verbPtr, _ := syscall.UTF16PtrFromString("open")
	filePtr, _ := syscall.UTF16PtrFromString("explorer")
	argsPtr, _ := syscall.UTF16PtrFromString("/select,\"" + filePath + "\"")
	win.ShellExecute(0, verbPtr, filePtr, argsPtr, nil, win.SW_SHOWNORMAL)
}

// onOpenOriginal 右键菜单: 打开原图.
func (p *PicListView) onOpenOriginal() {
	item := p.getSingleSelectedItem()
	if item == nil {
		return
	}
	shellOpen("open", item.FullPath)
}

// onShowInExplorer 右键菜单: 在资源管理器中显示原图.
func (p *PicListView) onShowInExplorer() {
	item := p.getSingleSelectedItem()
	if item == nil {
		return
	}
	shellOpenFolderAndSelect(item.FullPath)
}

// onOpenCompressed 右键菜单: 打开压缩后图片.
func (p *PicListView) onOpenCompressed() {
	item := p.getSingleSelectedItem()
	if item == nil || item.OutputPath == "" {
		return
	}
	shellOpen("open", item.OutputPath)
}

// onShowCompInExplorer 右键菜单: 在资源管理器中显示压缩后图片.
func (p *PicListView) onShowCompInExplorer() {
	item := p.getSingleSelectedItem()
	if item == nil || item.OutputPath == "" {
		return
	}
	shellOpenFolderAndSelect(item.OutputPath)
}

// getParentForm 向上遍历控件树, 找到所属的 Form 窗口.
func getParentForm(w walk.Widget) walk.Form {
	for w != nil {
		if f, ok := w.(walk.Form); ok {
			return f
		}
		c := w.Parent()
		if c == nil {
			return nil
		}
		if f, ok := c.(walk.Form); ok {
			return f
		}
		// 尝试将 Container 转为 Widget 继续向上遍历
		if w2, ok := c.(walk.Widget); ok {
			w = w2
		} else {
			return nil
		}
	}
	return nil
}

// onViewError 右键菜单: 查看错误信息, 弹窗显示错误详情并支持复制.
func (p *PicListView) onViewError() {
	item := p.getSingleSelectedItem()
	if item == nil || item.Message == "" {
		return
	}

	parentForm := getParentForm(p.TableView)
	if parentForm == nil {
		return
	}

	// 创建自定义错误详情对话框
	var dlg *walk.Dialog
	var textEdit *walk.TextEdit
	var btnCopy *walk.PushButton
	var btnClose *walk.PushButton

	err := declarative.Dialog{
		AssignTo:  &dlg,
		Title:     strs.CtxErrorDetailTitle,
		Icon:      p.appIcon,
		FixedSize: true,
		MinSize:   declarative.Size{Width: 480, Height: 260},
		MaxSize:   declarative.Size{Width: 480, Height: 260},
		Size:      declarative.Size{Width: 480, Height: 260},
		Layout:    declarative.VBox{Spacing: 10, Margins: declarative.Margins{Left: 15, Top: 15, Right: 15, Bottom: 15}},
		Children: []declarative.Widget{
			declarative.Label{
				Text: item.FileName,
				Font: declarative.Font{PointSize: 10, Bold: true},
			},
			declarative.TextEdit{
				AssignTo: &textEdit,
				Text:     item.Message,
				ReadOnly: true,
				MinSize:  declarative.Size{Width: 0, Height: 120},
				MaxSize:  declarative.Size{Width: 0, Height: 120},
			},
			declarative.Composite{
				Layout: declarative.HBox{Spacing: 10, MarginsZero: true},
				Children: []declarative.Widget{
					declarative.HSpacer{},
					declarative.PushButton{
						AssignTo: &btnCopy,
						Text:     strs.CtxCopyError,
						OnClicked: func() {
							if err := walk.Clipboard().SetText(item.Message); err == nil {
								btnCopy.SetText(strs.TextOK)
							}
						},
					},
					declarative.PushButton{
						AssignTo: &btnClose,
						Text:     strs.TextClose,
						OnClicked: func() {
							dlg.Close(0)
						},
					},
				},
			},
		},
	}.Create(parentForm)

	if err != nil {
		return
	}

	// 居中显示
	CenterWindowToOwner(dlg, parentForm)
	ApplyInheritedTopMost(dlg)
	dlg.Show()
}
