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

        public List<string> paths = new List<string>();

        public Form1()
        {
            InitializeComponent();
            main = this;
            ProgressForm.form = progressForm;
            SettingForm.form = settingForm;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void OnOpenClick(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "选择图片";
            dialog.Filter = "图片(JPG,PNG,BMP,TIFF)|*.jpg;*.png;*.bmp;*.tiff";
            dialog.Multiselect = true;
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                listBox1.Items.Clear();
                foreach (string path in dialog.FileNames)
                {
                    paths.Add(path);
                    listBox1.Items.Add(path);
                }
            }
        }

        private void OnChooseClick(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选择文件夹";
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    MessageBox.Show("路径不能为空!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                textBox1.Text = dialog.SelectedPath;
            }
        }

        private async void OnSizerClick(object sender, EventArgs e)
        {
            string folderPath = textBox1.Text;
            if (!Path.IsPathRooted(folderPath))
            {
                MessageBox.Show("请输入正确的绝对路径!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("发生了错误\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            Thread thread = new Thread(() =>
            {
                Bmp.StartResizer(paths, folderPath);
            });
            thread.Priority = ThreadPriority.Highest;
            progressForm.init(paths.Count);
            thread.Start();
            progressForm.ShowDialog();
            //MessageBox.Show("压缩完成", "PicSizer", MessageBoxButtons.OK);
        }

        private void OnSetClick(object sender, EventArgs e)
        {
            settingForm.ShowDialog();
        }

        private void OnAboutClick(object sender, EventArgs e)
        {
            dearXuan.ShowDialog();
        }
    }
}
