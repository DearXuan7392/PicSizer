#region

using System;
using System.IO;
using System.Windows.Forms;
using PicSizer.Program.Static;

#endregion

namespace PicSizer.Program.Window.Assemble
{
    public partial class PicListView : ListView
    {
        /// <summary>
        /// 绘图事件
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        /// <summary>
        /// 鼠标单击事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            //右键显示菜单
            if (e.Button == MouseButtons.Right)
            {
                //生成点击信息
                ListViewHitTestInfo info = HitTest(e.X, e.Y);
                //获取点击项
                PicListViewItem item = info.Item as PicListViewItem;
                //弹出菜单
                picListViewMenu.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        /// <summary>
        /// 鼠标双击事件
        /// </summary>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            //判断左键还是右键
            if (e.Button == MouseButtons.Left)
            {
                //生成点击信息
                ListViewHitTestInfo info = HitTest(e.X, e.Y);
                //获取点击项
                PicListViewItem item = info.Item as PicListViewItem;
                //打开文件夹并选中文件
                //System.Diagnostics.Process.Start("explorer", "/select," + item.FullPath);
                //预览图片
                FormsControl.ShowPicPreViewForm(item.Index);
            }
        }

        /// <summary>
        /// 按下按键事件
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control) //Ctrl + A
            {
                InvokeEvent(EventType.SelectAll);
            }
            else if (e.KeyCode == Keys.R && e.Control)
            {
                InvokeEvent(EventType.SelectReverse);
            }
            else if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                InvokeEvent(EventType.RemoveSelectedItem);
            }
        }

        /// <summary>
        /// 拖入事件
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
            {
                drgevent.Effect = DragDropEffects.Copy;
            }
            else
            {
                drgevent.Effect = DragDropEffects.None;
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            string[] files = drgevent.Data.GetData(DataFormats.FileDrop, false) as string[];
            foreach (string path in files)
            {
                //该路径是文件
                if (File.Exists(path))
                {
                    AddPicturesFromPath(path);
                }
                //不是文件就是文件夹
                else
                {
                    AddPicturesFromDirectory(path);
                }
            }
        }

        /// <summary>
        /// 选中项改变事件
        /// </summary>
        protected override void OnSelectedIndexChanged(EventArgs e = null)
        {
            if (PicValue.PicListSelectListenerEnable)
            {
                FormsControl.MainForm.UpdateSelectTotalNumLabel();
            }
        }
    }
}