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
using PicSizer.Partial;

namespace PicSizer
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// ListView集合
        /// </summary>
        private ListView.ListViewItemCollection collection;

        /// <summary>
        /// 用于检查文件是否重复
        /// </summary>
        private HashSet<string> picFiles = new HashSet<string>();

        public Form1()
        {
            InitializeComponent();
            //为静态量赋值
            SharedVariable.mainForm = this;
            Text = Info.ProjectName + " " + Info.ProjectVersion;
            collection = listView1.Items;
            DllExtern.DoInFirst();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private bool AddPicture(string path)
        {
            //判断文件是否已经在列表中
            if (picFiles.Contains(path))
            {
                return false;
            }
            else
            {
                //添加文件
                PicFile picFile = new PicFile(path);
                picFiles.Add(path);
                collection.Add(picFile.item);
                return true;
            }
        }

        private void RemovePicture(ListViewItem item)
        {
            //移除文件
            picFiles.Remove(item.SubItems[1].Text);
            collection.Remove(item);
        }

        /// <summary>
        /// 打开图片按钮
        /// </summary>
        private void OnAddClick(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "添加图片";
            if (SharedVariable.setting.AllowAnyExtension)
            {
                dialog.Filter = "图片(JPG,PNG,BMP,TIFF)|*.jpg;*.png;*.bmp;*.tiff|所有|*.*";
            }
            else
            {
                dialog.Filter = "图片(JPG,PNG,BMP,TIFF)|*.jpg;*.png;*.bmp;*.tiff";
            }
            dialog.Multiselect = true;
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                int fileCount = dialog.FileNames.Length;
                foreach (string path in dialog.FileNames)
                {
                    if (AddPicture(path)) fileCount--;
                }
                UpdateLowerLeftLabel();
                if (fileCount != 0)
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
        private async void OnResizeClick(object sender, EventArgs e)
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
                PictureProc.Resize.StartResizer(collection, folderPath);
            });
            thread.Priority = ThreadPriority.Highest;//设置线程优先级最高
            SharedVariable.progressForm.init(collection.Count);
            thread.Start();
            SharedVariable.progressForm.ShowDialog();
        }

        /// <summary>
        /// 设置按钮
        /// </summary>
        private void OnSetClick(object sender, EventArgs e)
        {
            SharedVariable.settingForm.ShowDialog();
        }

        /// <summary>
        /// 关于按钮
        /// </summary>
        private void OnAboutClick(object sender, EventArgs e)
        {
            SharedVariable.about.ShowDialog();
        }

        private void OnRemoveClick(object sender, EventArgs e)
        {
            int count = listView1.SelectedItems.Count;
            if(count == 0)
            {
                Dialog.ShowDialog_Warning("没有选中图片!");
                return;
            }
            if(Dialog.ShowDialog_OKDialog("移除所选的 " + count + " 张图片?"))
            {
                foreach(ListViewItem item in listView1.SelectedItems)
                {
                    RemovePicture(item);
                }
            }
            UpdateLowerLeftLabel();
            listView1.Focus();
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop, false) as string[];
                int count = files.Length;
                foreach(string path in files)
                {
                    if (SharedVariable.setting.AllowAnyExtension)
                    {
                        if (AddPicture(path)) count--;
                    }
                    else
                    {
                        if(path.EndsWith(".jpg") || path.EndsWith(".png") || path.EndsWith(".bmp") || path.EndsWith(".tiff"))
                        {
                            if (AddPicture(path)) count--;
                        }
                    }
                }
                UpdateLowerLeftLabel();
                if (count != 0)
                {
                    Dialog.ShowDialog(count + " 个重复或不符合格式的路径已被忽略.");
                }
            }
            catch(Exception ex)
            {
                Dialog.ShowDialog_Exception(ex);
            }
        }

        private new void DragEnter(object sender, DragEventArgs e)
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

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop, false) as string[];
                if(files.Length == 1 && Directory.Exists(files[0])){
                    textBox1.Text = files[0];
                }
                else
                {
                    Dialog.ShowDialog_Warning("请拖入一个文件夹!");
                }
            }
            catch (Exception ex)
            {
                Dialog.ShowDialog_Exception(ex);
            }
        }

        private void OnSelectAllClick(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            foreach(ListViewItem item in collection)
            {
                item.Selected = true;
            }
            UpdateLowerLeftLabel();
            listView1.Focus();
            listView1.EndUpdate();
        }

        private void OnSelectReverseClick(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            foreach (ListViewItem item in collection)
            {
                item.Selected = !item.Selected;
            }
            UpdateLowerLeftLabel();
            listView1.Focus();
            listView1.EndUpdate();
        }

        /// <summary>
        /// 更新左下角的Label，显示为"选中的项数/总项数"
        /// </summary>
        private void UpdateLowerLeftLabel()
        {
            label2.Text = listView1.SelectedItems.Count + "/" + collection.Count;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLowerLeftLabel();
        }
    }
}
