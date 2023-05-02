using PicSizer.Static;
using PicSizer.Window.Assemble;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSizer.Window.Forms
{
    public partial class PicPreViewForm : PicBaseForm
    {
        private int index = 0;

        private Bitmap bitmap = null;

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
            this.index = index;
            PicListViewItem item = PicValue.picListView[index];
            this.Text = string.Format($"第{index + 1}张:\"{item.FullPath}\"");
            //异步加载图片
            LoadPictureAsync(index);
            //选中目标项
            PicValue.picListView.SelectedItems.Clear();
            PicValue.picListView[index].Selected = true;
            PicValue.picListView.EnsureVisible(index);
            arrow_left.Enabled = index != 0;
            arrow_right.Enabled = index != PicValue.picListView.Items.Count - 1;
        }

        public void ShowTempBitmap(ref Bitmap bitmap)
        {
            this.pictureBox1.Image = bitmap;
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
                PicListViewItem item = PicValue.picListView[index];
                Bitmap bitmap;
                //修改下方信息
                label1.Text = "文 件 名:" + item.FileName;
                label2.Text = "图片大小:" + FileIO.FileProc.FileSizeToString(item.Size);
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
                if (this.index == index)
                {
                    //进入临界区
                    lock (this)
                    {
                        //加载失败
                        if (bitmap == null)
                        {
                            this.pictureBox1.Image = pictureBox1.ErrorImage;
                            label3.Text = "图片尺寸: 无";
                        }
                        //加载成功
                        else
                        {
                            //摧毁上一张图片
                            if (this.bitmap != null)
                            {
                                this.bitmap.Dispose();
                            }
                            //加载当前图片
                            this.bitmap = bitmap;
                            this.pictureBox1.Image = bitmap;
                            //修改下方信息
                            label1.Text = "文 件 名:" + item.FileName;
                            label2.Text = "图片大小:" + FileIO.FileProc.FileSizeToString(item.Size);
                            label3.Text = "图片尺寸:" + this.bitmap.Width + "×" + this.bitmap.Height;
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
                if (index > 0)
                {
                    UpdatePreviewPicture(--index);
                }
            }
            //下一张
            else
            {
                if (index < PicValue.picListView.Items.Count - 1)
                {
                    UpdatePreviewPicture(++index);
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
            this.Dispose();
            if (bitmap != null)
            {
                bitmap.Dispose();
            }
        }

        private void PicPreViewForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormsControl.PicPreViewForm = null;
            this.Dispose();
        }
    }
}
