using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSizer
{
    public partial class Form1 : Form
    {
        public static Form1 main;
        public static SettingForm settingForm = new SettingForm();
        public static ProgressForm progressForm = new ProgressForm();
        public static DearXuan dearXuan = new DearXuan();

        ListBox.ObjectCollection paths;

        public Form1()
        {
            InitializeComponent();
            //为静态量赋值
            main = this;
            ProgressForm.form = progressForm;
            SettingForm.form = settingForm;
            paths = listBox1.Items;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// 打开图片按钮
        /// </summary>
        private void OnAddClick(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "添加图片";
            dialog.Filter = "图片(JPG,PNG,BMP,TIFF)|*.jpg;*.png;*.bmp;*.tiff";
            dialog.Multiselect = true;
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                int fileCount = dialog.FileNames.Length;
                foreach (string path in dialog.FileNames)
                {
                    if (!paths.Contains(path))
                    {
                        paths.Add(path);
                        fileCount--;
                    }
                }
                if(fileCount != 0)
                {
                    Dialog.ShowDialog(fileCount + " 张重复的图片已被忽略.");
                }
            }
        }

        /// <summary>
        /// 选择文件夹按钮
        /// </summary>
        private void OnChooseClick(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选择文件夹";
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    Dialog.ShowDialog_Error("路径不能为空!");
                    return;
                }
                textBox1.Text = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// 压缩按钮
        /// </summary>
        private async void OnSizerClick(object sender, EventArgs e)
        {
            string folderPath = textBox1.Text;
            if (!Path.IsPathRooted(folderPath))
            {
                Dialog.ShowDialog_Error("请输入正确的绝对路径!");
                return;
            }
            if (Directory.Exists(folderPath))
            {
                if(Directory.GetFiles(folderPath).Length > 0)
                {
                    if(MessageBox.Show("文件夹内的文件将被覆盖!","警告",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    {
                        return;
                    }
                }
            }
            else
            {
                try
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
                    dirInfo.Create();
                }
                catch (Exception ex)
                {
                    Dialog.ShowDialog_Exception(ex);
                    return;
                }
            }
            //创建处理图片的线程
            Thread thread = new Thread(() =>
            {
                Bmp.StartResizer(paths, folderPath);
            });
            thread.Priority = ThreadPriority.Highest;//设置线程优先级最高
            progressForm.init(paths.Count);
            thread.Start();
            progressForm.ShowDialog();
        }

        /// <summary>
        /// 设置按钮
        /// </summary>
        private void OnSetClick(object sender, EventArgs e)
        {
            settingForm.ShowDialog();
        }

        /// <summary>
        /// 关于按钮
        /// </summary>
        private void OnAboutClick(object sender, EventArgs e)
        {
            dearXuan.ShowDialog();
        }

        private void OnRemoveClick(object sender, EventArgs e)
        {
            int count = listBox1.SelectedItems.Count;
            int index;
            if(count == 0)
            {
                Dialog.ShowDialog_Warning("没有选中图片!");
                return;
            }
            if(Dialog.ShowDialog_OKDialog("移除所选的 " + count + " 张图片?"))
            {
                while((index = listBox1.SelectedIndex) != -1)
                {
                    listBox1.Items.RemoveAt(index);
                }
            }
        }
    }
}
