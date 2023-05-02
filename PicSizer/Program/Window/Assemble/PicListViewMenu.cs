using PicSizer.Static;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSizer.Window.Assemble
{
    public partial class PicListViewMenu : ContextMenuStrip
    {
        public PicListViewMenu()
        {
            InitializeComponent();
            this.Items.Add("在Explorer显示", null, OnItemClick_Show);
            this.Items.Add("预览", null, OnItemClick_Preview);
            this.Items.Add("删除", null, OnItemClick_Delete);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        private void OnItemClick_Show(object obj, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", "/select," + ((PicListViewItem)PicValue.picListView.SelectedItems[0]).FullPath);
        }

        private void OnItemClick_Preview(object obj, EventArgs e)
        {
            FormsControl.ShowPicPreViewForm();
        }

        private void OnItemClick_Delete(object obj, EventArgs e)
        {
            PicValue.picListView.InvokeEvent(PicListView.EventType.RemoveSelectedItem);
        }
    }
}
