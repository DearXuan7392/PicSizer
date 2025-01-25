#region

using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PicSizer.Program.FileIO;
using static PicSizer.Program.Static.PicUnit;

#endregion

namespace PicSizer.Program.Window.Assemble
{
    public class PicListViewItem : ListViewItem
    {
        /// <summary>
        /// 文件大小(单位:KB)
        /// </summary>
        private long _size;

        /// <summary>
        /// 图片状态
        /// </summary>
        private PicItemState _state;

        private string _message = null;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get => SubItems[0].Text;
            private set => SubItems[0].Text = value;
        }

        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath
        {
            get => SubItems[1].Text;
            private set => SubItems[1].Text = value;
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size
        {
            get => _size;
            private set
            {
                _size = value;
                SubItems[2].Text = FileProc.FileSizeToString(_size);
            }
        }

        /// <summary>
        /// 其他信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public PicItemState State
        {
            get => _state;
            set
            {
                _state = value;
                switch (value)
                {
                    case PicItemState.Waiting:
                        SubItems[3].Text = "待压缩";
                        SubItems[3].ForeColor = Color.Black;
                        break;
                    case PicItemState.Compression:
                        SubItems[3].Text = "压缩中";
                        SubItems[3].ForeColor = Color.Blue;
                        break;
                    case PicItemState.Success:
                        SubItems[3].Text = "完成";
                        SubItems[3].ForeColor = Color.Green;
                        break;
                    case PicItemState.OutOfLimit:
                        SubItems[3].Text = "超出";
                        SubItems[3].ForeColor = Color.Orange;
                        break;
                    case PicItemState.Error:
                        SubItems[3].Text = "错误";
                        SubItems[3].ForeColor = Color.Red;
                        break;
                }
            }
        }

        public PicListViewItem(string path)
        {
            for (int i = 0; i < 3; i++)
            {
                SubItems.Add(new ListViewSubItem());
            }

            UseItemStyleForSubItems = false;
            FileInfo fileInfo = new FileInfo(path);
            FullPath = fileInfo.FullName;
            FileName = fileInfo.Name;
            Size = fileInfo.Length >> 10; //单位:KB
            State = PicItemState.Waiting;
        }
    }
}