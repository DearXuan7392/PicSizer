using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using PicSizer.Partial;
using PicSizer.Custom;

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
            this.Icon = Info.icon;
            //为静态量赋值
            Value.mainForm = this;
            Text = Info.ProjectName + " " + Info.ProjectVersion;
            collection = listView1.Items;
            listView1.GridLines = true;//增加分割线
            
            DllExtern.DoInFirst();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// 添加一张图片
        /// </summary>
        private bool AddPicture(string path)
        {
            //判断文件是否已经在列表中
            if (picFiles.Contains(path))
            {
                return false;
            }
            else
            {
                //判断文件后缀是否合法
                if (FileCheck.isExtensionCorrect(path))
                {
                    //添加文件
                    picFiles.Add(path);
                    //collection.Add(Support.GetListViewItemByPath(path));
                    collection.Add(new PicListViewItem(path));
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 获取文件夹里的所有图片，包括子文件夹
        /// </summary>
        private List<string> GetPictureFromDir(string dirPath)
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            List<string> fileList = new List<string>();
            FileInfo[] files = dir.GetFiles();//文件夹里的所有图片
            DirectoryInfo[] dirs = dir.GetDirectories();//文件夹里的所有子文件夹
            //遍历所有子文件
            foreach (FileInfo info in files)
            {
                if (FileCheck.isExtensionCorrect(info.FullName))
                {
                    fileList.Add(info.FullName);
                }
            }
            //遍历所有子文件夹
            foreach (DirectoryInfo info in dirs)
            {
                fileList.AddRange(GetPictureFromDir(info.FullName));
            }
            return fileList;
        }

        /// <summary>
        /// 移除一张图片
        /// </summary>
        private void RemovePicture(ListViewItem item)
        {
            //移除文件
            picFiles.Remove(item.SubItems[1].Text);
            collection.Remove(item);
        }

        /// <summary>
        /// 选择文件夹按钮
        /// </summary>
        private void OnChooseClick(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = Dialog.GetFolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    Dialog.ShowDialog_Error("路径不能为空!");
                    return;
                }
                textBox_OutputDirText.Text = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// 压缩按钮
        /// </summary>
        private void OnResizeClick(object sender, EventArgs e)
        {
            string folderPath = textBox_OutputDirText.Text;
            //如果选择指定目录，则判断目录是否合法
            if (!Value.CoverOriginalFile)
            {
                if (!Path.IsPathRooted(folderPath))
                {
                    Dialog.ShowDialog_Error("请输入正确的绝对路径!");
                    return;
                }
                if (Directory.Exists(folderPath))
                {
                    if (Directory.GetFiles(folderPath).Length > 0)
                    {
                        if (MessageBox.Show("文件夹内的文件将被覆盖!", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
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
            }
            //创建处理图片的线程
            Thread thread = new Thread(() =>
            {
                PictureProc.Resize.StartResizer(collection, folderPath);
            });
            thread.Priority = ThreadPriority.Highest;//设置线程优先级最高
            Value.progressForm.init(collection.Count);
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

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop, false) as string[];
                int total = files.Length;
                int success = 0;
                foreach (string path in files)
                {
                    //该路径是文件
                    if (File.Exists(path))
                    {
                        if (AddPicture(path)) success++;
                    }
                    //不是文件就是文件夹
                    else
                    {
                        List<string> fList = GetPictureFromDir(path);
                        total += fList.Count - 1;//图片总数加上文件夹内的图片总数，并减去文件夹自己
                        //加载文件夹里的图片
                        foreach (string f in fList)
                        {
                            if (AddPicture(f)) success++;
                        }
                    }
                    
                }
                UpdateSelectTotalNumLabel();
                if (total != success)
                {
                    string s = string.Format("共发现{0}个文件,其中{1}个因重复或格式不符而加载失败.", total, (total - success));
                    Dialog.ShowDialog(s);
                }
            }
            catch (Exception ex)
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
                if (files.Length == 1 && Directory.Exists(files[0]))
                {
                    textBox_OutputDirText.Text = files[0];
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
            foreach (ListViewItem item in collection)
            {
                item.Selected = true;
            }
            UpdateSelectTotalNumLabel();
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
            UpdateSelectTotalNumLabel();
            listView1.Focus();
            listView1.EndUpdate();
        }

        /// <summary>
        /// 更新左下角的Label，显示为"选中的项数/总项数"
        /// </summary>
        private void UpdateSelectTotalNumLabel()
        {
            label2.Text = listView1.SelectedItems.Count + "/" + collection.Count;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //更新选择的项数
            UpdateSelectTotalNumLabel();
        }

        private void 添加文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = Dialog.GetOpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int fileCount = dialog.FileNames.Length;
                foreach (string path in dialog.FileNames)
                {
                    if (AddPicture(path)) fileCount--;
                }
                UpdateSelectTotalNumLabel();
                if (fileCount != 0)
                {
                    Dialog.ShowDialog(fileCount + " 张重复的图片已被忽略.");
                }
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
                Dialog.OpenLink("https://gitee.com/dearxuan/pic-sizer#%E9%A1%B9%E7%9B%AE%E4%BB%8B%E7%BB%8D");
            }
        }

        private void CoverOriginalFile(object sender, EventArgs e)
        {
            Value.CoverOriginalFile = radioButton_Cover.Checked;
            textBox_OutputDirText.Enabled = button_Choose.Enabled = !Value.CoverOriginalFile;
        }

        /// <summary>
        /// 按下移除按钮
        /// </summary>
        private void OnRemoveItemClick(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            if (sender == 选中项ToolStripMenuItem)
            {
                int count = listView1.SelectedItems.Count;
                if (count == 0)
                {
                    Dialog.ShowDialog_Warning("没有选中图片!");
                    listView1.EndUpdate();
                    return;
                }
                if (Dialog.ShowDialog_OKDialog("移除所选的 " + count + " 张图片?"))
                {
                    foreach (ListViewItem item in listView1.SelectedItems)
                    {
                        RemovePicture(item);
                    }
                }
                UpdateSelectTotalNumLabel();
                listView1.Focus();
            }
            else if (sender == 已完成ToolStripMenuItem)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    if (((PicListViewItem)collection[i]).State == PicState.Success)
                    {
                        collection.RemoveAt(i);
                        i--;
                    }
                }
            }
            else if (sender == 错误项ToolStripMenuItem)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    if (((PicListViewItem)collection[i]).State == PicState.Error)
                    {
                        collection.RemoveAt(i);
                        i--;
                    }
                }
            }
            else if (sender == 全部项ToolStripMenuItem)
            {
                if (Dialog.ShowDialog_OKDialog("是否清空列表,包括未完成的项目?"))
                {
                    collection.Clear();
                    picFiles.Clear();
                }
            }
            listView1.EndUpdate();
        }

        private void OnListViewKetDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control)//Ctrl + A
            {
                //模拟按下全选键
                OnSelectAllClick(null, null);
            }
            else if (e.KeyCode == Keys.R && e.Control)
            {
                //模拟按下反选
                OnSelectReverseClick(null, null);
            }
            else if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                //模拟按下“删除选中项”
                OnRemoveItemClick(选中项ToolStripMenuItem, null);
            }
        }

        private void 打开文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选择文件夹";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    Dialog.ShowDialog_Warning("文件夹路径不能为空!");
                    return;
                }
                List<string> fileList = GetPictureFromDir(dialog.SelectedPath);
                listView1.BeginUpdate();
                foreach (string item in fileList)
                {
                    AddPicture(item);
                }
                listView1.EndUpdate();
            }
            return;
        }
    }
}