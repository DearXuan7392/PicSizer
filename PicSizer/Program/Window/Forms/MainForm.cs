#region

using System;
using System.IO;
using System.Windows.Forms;
using PicSizer.Program.Deliver;
using PicSizer.Program.Static;
using PicSizer.Program.Window.Partial;
using PicSizer.Program.Window.Assemble;

#endregion

namespace PicSizer.Program.Window.Forms
{
    public partial class MainForm : PicBaseForm
    {
        public MainForm()
        {
            InitializeComponent();
            //为静态量赋值
            PicValue.PicListView = PicListView;
            FormsControl.MainForm = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 压缩按钮
        /// </summary>
        private void OnResizeClick(object sender, EventArgs e)
        {
            //开始压缩任务
            PicValue.OutputDirection = textBox_OutputDir.Text;

            if (radioButton_CoverOrigin.Checked)
            {
                PicSetting.OutputType = PicUnit.OutputType.CoverOrigin;
            }
            else if (radioButton_OutputDirection.Checked)
            {
                PicSetting.OutputType = PicUnit.OutputType.OutputDirection;
            }
            else if (radioButton_OutputStructure.Checked)
            {
                PicSetting.OutputType = PicUnit.OutputType.OutputStructure;
            }

            Task.StartCompressTask();
        }

        /// <summary>
        /// 设置按钮
        /// </summary>
        private void OnSetClick(object sender, EventArgs e)
        {
            FormsControl.ShowSettingForm();
        }

        /// <summary>
        /// 更新右上角的Label，显示为"选中的项数/总项数"
        /// </summary>
        public void UpdateSelectTotalNumLabel()
        {
            int select = PicListView.SelectedItems.Count;
            int total = PicListView.Items.Count;
            label_SelectByTotal.Text = $"{select}/{total}";
        }

        /// <summary>
        /// 单击帮助菜单
        /// </summary>
        private void OnHelpMenuClick(object sender, EventArgs e)
        {
            if (sender == 关于ToolStripMenuItem)
            {
                FormsControl.ShowAboutForm();
            }
        }

        /// <summary>
        /// 菜单栏-文件菜单
        /// </summary>
        private void OnFileItemClick(object sender, EventArgs e)
        {
            if (sender == 添加文件ToolStripMenuItem)
            {
                string[] fileList = Dialog.Show_OpenFileDialog();
                if (fileList != null && fileList.Length != 0)
                {
                    PicListView.AddPicturesFromPath(fileList);
                }
            }
            else if (sender == 打开文件夹ToolStripMenuItem)
            {
                string selectPath = Dialog.Show_FolderBrowserDialog();
                if (selectPath != null)
                {
                    PicListView.AddPicturesFromDirectory(selectPath);
                }
            }
            else if (sender == 退出ToolStripMenuItem)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// 菜单栏-移除菜单
        /// </summary>
        private void OnRemoveItemClick(object sender, EventArgs e)
        {
            if (sender == 选中项ToolStripMenuItem)
            {
                PicListView.InvokeEvent(Assemble.PicListView.EventType.RemoveSelectedItem);
            }
            else if (sender == 已完成ToolStripMenuItem)
            {
                PicListView.InvokeEvent(Assemble.PicListView.EventType.RemoveSuccess);
            }
            else if (sender == 错误项ToolStripMenuItem)
            {
                PicListView.InvokeEvent(Assemble.PicListView.EventType.RemoveError);
            }
            else if (sender == 全部项ToolStripMenuItem)
            {
                if (Dialog.ShowDialog_OKDialog("是否清空列表,包括未完成的项目?"))
                {
                    PicListView.InvokeEvent(Assemble.PicListView.EventType.RemoveAll);
                }
            }
        }

        /// <summary>
        /// 菜单栏-选择菜单
        /// </summary>
        private void OnSelectItemClick(object sender, EventArgs e)
        {
            if (sender == 全选ToolStripMenuItem)
            {
                PicListView.InvokeEvent(Assemble.PicListView.EventType.SelectAll);
            }
            else if (sender == 反选ToolStripMenuItem)
            {
                PicListView.InvokeEvent(Assemble.PicListView.EventType.SelectReverse);
            }
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        private void OnAppExit(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void textBox_OutputDir_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void textBox_OutputDir_DragDrop(object sender, DragEventArgs e)
        {
            string[] dirs = e.Data.GetData(DataFormats.FileDrop, false) as string[];
            if (dirs.Length == 1 && Directory.Exists(dirs[0]))
            {
                textBox_OutputDir.Text = dirs[0];
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_CoverOrigin.Checked)
            {
                textBox_OutputDir.Enabled = false;
                button_OpenFolder.Enabled = false;
            }
            else
            {
                textBox_OutputDir.Enabled = true;
                button_OpenFolder.Enabled = true;
            }
        }

        private void button_OpenFolder_Click(object sender, EventArgs e)
        {
            string selectPath = Dialog.Show_FolderBrowserDialog();
            if (selectPath != null)
            {
                textBox_OutputDir.Text = selectPath;
            }
        }
    }
}