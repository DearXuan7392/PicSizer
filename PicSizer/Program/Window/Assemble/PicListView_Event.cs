using PicSizer.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSizer.Window.Assemble
{
    public partial class PicListView : ListView
    {
        public enum EventType
        {
            SelectAll = 0, //全选
            SelectReverse = 1, //反选
            RemoveAll = 2, //删除全部的项
            RemoveWaiting = 3, //删除等待中的项
            RemoveSuccess = 4, //删除已完成的项
            RemoveError = 5, //删除错误的项
            RemoveSelectedItem = 6, //删除选中的项
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        public void InvokeEvent(EventType selectType)
        {
            //关闭监听
            PicValue.PicListSelectListenerEnable = false;
            this.BeginUpdate();
            switch (selectType)
            {
                //全选
                case EventType.SelectAll:
                    foreach (PicListViewItem item in this.Items)
                    {
                        item.Selected = true;
                    }
                    break;
                //反选
                case EventType.SelectReverse:
                    foreach (PicListViewItem item in this.Items)
                    {
                        item.Selected = !item.Selected;
                    }
                    break;
                //删除全部的项
                case EventType.RemoveAll:
                    foreach (PicListViewItem item in this.Items)
                    {
                        this.Items.Clear();
                        this.ItemPathHashSet.Clear();
                    }
                    break;
                //删除等待中的项
                case EventType.RemoveWaiting:
                    for(int i = this.Items.Count - 1; i >= 0; i--)
                    {
                        PicListViewItem item = this[i];
                        if (item.State == Static.PicUnit.PicItemState.Waiting)
                        {
                            this.Items.RemoveAt(i);
                            this.ItemPathHashSet.Remove(item.FullPath);
                        }
                    }
                    break;
                //删除已完成的项
                case EventType.RemoveSuccess:
                    for (int i = this.Items.Count - 1; i >= 0; i--)
                    {
                        PicListViewItem item = this[i];
                        if (item.State == Static.PicUnit.PicItemState.Success)
                        {
                            this.Items.RemoveAt(i);
                            this.ItemPathHashSet.Remove(item.FullPath);
                        }
                    }
                    break;
                //删除错误的项
                case EventType.RemoveError:
                    for (int i = this.Items.Count - 1; i >= 0; i--)
                    {
                        PicListViewItem item = this[i];
                        if (item.State == Static.PicUnit.PicItemState.Error)
                        {
                            this.Items.RemoveAt(i);
                            this.ItemPathHashSet.Remove(item.FullPath);
                        }
                    }
                    break;
                //删除选中项
                case EventType.RemoveSelectedItem:
                    for (int i = this.Items.Count - 1; i >= 0; i--)
                    {
                        PicListViewItem item = this[i];
                        if (item.Selected)
                        {
                            this.Items.RemoveAt(i);
                            this.ItemPathHashSet.Remove(item.FullPath);
                        }
                    }
                    break;
            }
            this.EndUpdate();
            //打开监听
            PicValue.PicListSelectListenerEnable = true;
            //更新一次数据
            OnSelectedIndexChanged();
        }
    }
}
