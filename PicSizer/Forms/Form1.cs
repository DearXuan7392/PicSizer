using PicSizer_ControlLibrary;
using System;
using System.Threading;
using System.Windows.Forms;
using PicSizer.Class.Partial;
using PicSizer.Class.Static;
using System.IO;

namespace PicSizer.Forms
{
    public partial class Form1 : Form
    {
        public Form1(string[] args)
        {
            InitializeComponent();
            this.Icon = Info.icon;
            //为静态量赋值
            Value.mainForm = this;
            this.Text = Info.ProjectName + " " + Info.ProjectVersion;
            foreach(string extension in Class.Unit.Extension.BitmapSupportExtension)
            {
                this.listView1.ExtensionCollection.Add(extension);
            }
            foreach(string path in args)
            {
                if (File.Exists(path))
                {
                    listView1.AddPicturesFromPath(new string[] { path });
                }
                else if (Directory.Exists(path))
                {
                    listView1.AddPicturesFromDirection(path);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PicSizer.Class.Static.ProjectInit.After();
        }

        /// <summary>
        /// 压缩按钮
        /// </summary>
        private void OnResizeClick(object sender, EventArgs e)
        {
            if(listView1.Items.Count == 0)
            {
                Dialog.ShowDialog_Warning("没有待压缩的图片.");
                return;
            }
            Value.CoverOriginalFile = picDirPathText1.IsUseOriginalDir();
            string folderPath = null;
            //如果选择指定目录，则判断目录是否合法
            if (!Value.CoverOriginalFile)
            {
                folderPath = picDirPathText1.GetDirPath();
                if (folderPath == null)
                {
                    return;
                }
            }
            listView1.SetAllToWaiting();
            //创建处理图片的线程
            Thread thread = new Thread(() =>
            {
                Class.PictureProc.Resize.StartResizer(folderPath);
            });
            thread.Priority = ThreadPriority.Highest;//设置线程优先级最高
            Value.progressForm.Init(listView1.Items.Count);
            thread.Start();
            Value.progressForm.ShowDialog();
        }

        /// <summary>
        /// 设置按钮
        /// </summary>
        private void OnSetClick(object sender, EventArgs e)
        {
            Value.settingForm.ShowDialog();
        }

        /// <summary>
        /// 更新左下角的Label，显示为"选中的项数/总项数"
        /// </summary>
        private void UpdateSelectTotalNumLabel(PicListView picListView, int select, int total)
        {
            label2.Text = string.Format("已选中 {0}/{1}", select, total);
        }

        private void OnHelpMenuClick(object sender, EventArgs e)
        {
            if (sender == 作者ToolStripMenuItem)
            {
                (new DearXuan()).ShowDialog();
            }
            else if (sender == 关于ToolStripMenuItem)
            {
                (new About()).ShowDialog();
            }
            else if (sender == 文档ToolStripMenuItem)
            {
                Dialog.OpenLink("https://picsizer.dearxuan.top");
            }
            else if (sender == 反馈和建议ToolStripMenuItem)
            {
                Dialog.OpenLink("https://gitee.com/picsizer/pic-sizer/issues");
            }
            else if(sender == 检查更新ToolStripMenuItem)
            {
                Class.Partial.Update.CheckUpdate(Info.ProjectVersion.alpha);
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
                    listView1.AddPicturesFromPath(fileList);
                }
            }
            else if (sender == 打开文件夹ToolStripMenuItem)
            {
                string selectPath = Dialog.Show_FolderBrowserDialog();
                if (selectPath != null)
                {
                    listView1.AddPicturesFromDirection(selectPath);
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
                listView1.RemovePicture(RemoveType.Select);
            }
            else if (sender == 已完成ToolStripMenuItem)
            {
                listView1.RemovePicture(RemoveType.Success);
            }
            else if (sender == 错误项ToolStripMenuItem)
            {
                listView1.RemovePicture(RemoveType.Error);
            }
            else if (sender == 全部项ToolStripMenuItem)
            {
                if (Dialog.ShowDialog_OKDialog("是否清空列表,包括未完成的项目?"))
                {
                    listView1.RemovePicture(RemoveType.All);
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
                listView1.SelectAll();
            }
            else if (sender == 反选ToolStripMenuItem)
            {
                listView1.SelectReverse();
            }
        }

        private void OnAppExit(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}