using PicSizer.Static;
using PicSizer.Window.Partial;
using System;
using System.Windows.Forms;

namespace PicSizer.Window
{
    /// <summary>
    /// PicSizer基础窗体
    /// </summary>
    public partial class PicBaseForm : Form
    {
        public PicBaseForm()
        {
            InitializeComponent();
            //屏蔽Alt+F4
            Window.Support.ForbidAltF4(this);
            //设置图标
            this.Icon = PicInfo.icon;
            //关闭线程安全检查
            CheckForIllegalCrossThreadCalls = false;
            //置顶
            this.TopMost = PicSetting.TopMost;
            //添加到窗体数组
            FormsControl.FormList.Add(this);
        }

        private void PicBaseForm_Load(object sender, EventArgs e)
        {
            
        }

        private void PicBaseForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormsControl.FormList.Remove(this);
            this.Dispose();
        }
    }
}
