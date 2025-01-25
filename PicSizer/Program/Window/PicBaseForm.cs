#region

using System;
using System.Windows.Forms;
using PicSizer.Program.Static;

#endregion

namespace PicSizer.Program.Window
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
            Support.ForbidAltF4(this);
            //设置图标
            Icon = PicInfo.AppIcon;
            //关闭线程安全检查
            CheckForIllegalCrossThreadCalls = false;
            //置顶
            TopMost = PicSetting.TopMost;
            //添加到窗体数组
            FormsControl.FormList.Add(this);
        }

        private void PicBaseForm_Load(object sender, EventArgs e)
        {
        }

        private void PicBaseForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormsControl.FormList.Remove(this);
            Dispose();
        }
    }
}