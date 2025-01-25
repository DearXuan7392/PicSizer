#region

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using PicSizer.Program.FileIO;
using PicSizer.Program.Static;
using PicSizer.Program.Window.Assemble;

#endregion

namespace PicSizer.Program.Window.Forms
{
    public partial class PicPreViewForm : PicBaseForm
    {
        private int _index;

        private Bitmap _img;

        /// <summary>
        /// 使用低画质图片
        /// </summary>
        public bool UseLowQualityPicture = true;

        public PicPreViewForm()
        {
            InitializeComponent();
        }

        private void PicPreViewForm_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 显示预览图片
        /// </summary>
        /// <param name="index"></param>
        public void UpdatePreviewPicture(int index)
        {
            this._index = index;
            PicListViewItem item = PicValue.PicListView[index];
            Text = string.Format($"第{index + 1}张:\"{item.FullPath}\"");
            //异步加载图片
            LoadPictureAsync(index);
            //选中目标项
            PicValue.PicListView.SelectedItems.Clear();
            PicValue.PicListView[index].Selected = true;
            PicValue.PicListView.EnsureVisible(index);
            arrow_left.Enabled = index != 0;
            arrow_right.Enabled = index != PicValue.PicListView.Items.Count - 1;
        }

        public void ShowTempBitmap(ref Bitmap bitmap)
        {
            pictureBox1.Image = bitmap;
            label1.Text = "文 件 名: 预览图";
            label2.Text = "图片大小: 预览图";
            label3.Text = "图片尺寸:" + bitmap.Width + "×" + bitmap.Height;
        }

        /// <summary>
        /// 多线程加载Bitmap
        /// </summary>
        private void LoadPictureAsync(int index)
        {
            Thread thread = new Thread(() =>
            {
                //获取Item项
                PicListViewItem item = PicValue.PicListView[index];
                Bitmap bitmap;
                //修改下方信息
                label1.Text = "文 件 名:" + item.FileName;
                label2.Text = "图片大小:" + FileProc.FileSizeToString(item.Size);
                //加载Bitmap,该过程耗时
                try
                {
                    bitmap = new Bitmap(item.FullPath);
                }
                catch (Exception)
                {
                    bitmap = null;
                }

                //两者相等,说明需要加载的图片没有发生变化
                if (this._index == index)
                {
                    //进入临界区
                    lock (this)
                    {
                        //加载失败
                        if (bitmap == null)
                        {
                            pictureBox1.Image = pictureBox1.ErrorImage;
                            label3.Text = "图片尺寸: 无";
                        }
                        //加载成功
                        else
                        {
                            //摧毁上一张图片
                            if (this._img != null)
                            {
                                this._img.Dispose();
                            }

                            //加载当前图片
                            this._img = bitmap;
                            pictureBox1.Image = bitmap;
                            //修改下方信息
                            label1.Text = "文 件 名:" + item.FileName;
                            label2.Text = "图片大小:" + FileProc.FileSizeToString(item.Size);
                            label3.Text = "图片尺寸:" + this._img.Width + "×" + this._img.Height;
                        }
                    }
                }
                //两者不相等,说明在等待期间用户加载了另一张图片,则当前图片作废
                else
                {
                    bitmap.Dispose();
                }
            });
            thread.Start();
        }

        private void OnArrowClick(object sender, EventArgs e)
        {
            //点击“上一张”
            if (sender == arrow_left)
            {
                if (_index > 0)
                {
                    UpdatePreviewPicture(--_index);
                }
            }
            //下一张
            else
            {
                if (_index < PicValue.PicListView.Items.Count - 1)
                {
                    UpdatePreviewPicture(++_index);
                }
            }
        }

        private void OnArrowEnableChanged(object sender, EventArgs e)
        {
            Button ori = sender as Button;
            ori.ForeColor = ori.Enabled
                ? Color.Black
                : SystemColors.ControlDark;
        }

        /// <summary>
        /// 关闭事件
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            Dispose();
            if (_img != null)
            {
                _img.Dispose();
            }
        }

        private void PicPreViewForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormsControl.PicPreViewForm = null;
            Dispose();
        }
    }
}