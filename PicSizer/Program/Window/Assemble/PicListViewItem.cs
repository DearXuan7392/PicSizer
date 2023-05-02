using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PicSizer.Static.PicUnit;

namespace PicSizer.Window.Assemble
{
    public class PicListViewItem : System.Windows.Forms.ListViewItem
    {
        /// <summary>
        /// 文件大小(单位:KB)
        /// </summary>
        private long _Size;

        /// <summary>
        /// 图片状态
        /// </summary>
        private PicItemState _State;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get { return this.SubItems[0].Text; }
            set { this.SubItems[0].Text = value; }
        }

        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath
        {
            get { return this.SubItems[1].Text; }
            set { this.SubItems[1].Text = value; }
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size
        {
            get { return this._Size; }
            set
            {
                this._Size = value;
                this.SubItems[2].Text = FileIO.FileProc.FileSizeToString(_Size);
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public PicItemState State
        {
            get { return this._State; }
            set
            {
                this._State = value;
                switch (value)
                {
                    case PicItemState.Waiting:
                        this.SubItems[3].Text = "待压缩";
                        this.SubItems[3].ForeColor = System.Drawing.Color.Black;
                        break;
                    case PicItemState.Compression:
                        this.SubItems[3].Text = "压缩中";
                        this.SubItems[3].ForeColor = System.Drawing.Color.Blue;
                        break;
                    case PicItemState.Success:
                        this.SubItems[3].Text = "已完成";
                        this.SubItems[3].ForeColor = System.Drawing.Color.Green;
                        break;
                    case PicItemState.Error:
                        this.SubItems[3].Text = "错误";
                        this.SubItems[3].ForeColor = System.Drawing.Color.Red;
                        break;
                }
            }
        }

        public PicListViewItem(string path)
        {
            for (int i = 0; i < 3; i++)
            {
                this.SubItems.Add(new ListViewSubItem());
            }
            this.UseItemStyleForSubItems = false;
            FileInfo fileInfo = new FileInfo(path);
            this.FullPath = fileInfo.FullName;
            this.FileName = fileInfo.Name;
            this.Size = fileInfo.Length >> 10;//单位:KB
            this.State = PicItemState.Waiting;
        }
    }
}
