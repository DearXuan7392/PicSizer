#region

using System;
using System.Diagnostics;
using System.Windows.Forms;
using PicSizer.Program.Static;
using PicSizer.Program.Window.Partial;

#endregion

namespace PicSizer.Program.Window.Assemble
{
    public partial class PicListViewMenu : ContextMenuStrip
    {
        public PicListViewMenu()
        {
            InitializeComponent();
            Items.Add("在Explorer显示", null, OnItemClick_Show);
            Items.Add("预览", null, OnItemClick_Preview);
            Items.Add("删除", null, OnItemClick_Delete);
            Items.Add("错误信息", null, OnItemClick_ErrorInfo);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        private void OnItemClick_Show(object obj, EventArgs e)
        {
            Process.Start("explorer",
                "/select," + ((PicListViewItem)PicValue.PicListView.SelectedItems[0]).FullPath);
        }

        private void OnItemClick_Preview(object obj, EventArgs e)
        {
            FormsControl.ShowPicPreViewForm();
        }

        private void OnItemClick_Delete(object obj, EventArgs e)
        {
            PicValue.PicListView.InvokeEvent(PicListView.EventType.RemoveSelectedItem);
        }

        private void OnItemClick_ErrorInfo(object obj, EventArgs e)
        {
            string info = ((PicListViewItem)PicValue.PicListView.SelectedItems[0]).Message;
            Dialog.ShowDialog_Error(info ?? "无错误信息");
        }
    }
}