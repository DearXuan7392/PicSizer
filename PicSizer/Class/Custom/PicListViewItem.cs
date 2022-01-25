using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PicSizer;
using PicSizer.Partial;

namespace PicSizer.Custom
{
    public class PicListViewItem : ListViewItem
    {
        private string _FileName;
        private string _FullPath;
        private long _Size;
        private PicState _State;

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        { 
            get
            {
                return this._FileName;
            } 
            set
            {
                this._FileName = value;
                this.SubItems[0].Text = value;
            }
        }

        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath
        {
            get
            {
                return this._FullPath;
            }
            set
            {
                this._FullPath = value;
                this.SubItems[1].Text = value;
            }
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size
        {
            get
            {
                return this._Size;
            }
            set
            {
                this._Size = value;
                this.SubItems[2].Text = this.Size.ToFileSizeStr();
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public PicState State
        {
            get
            {
                return this._State;
            }
            set
            {
                this._State = value;
                this.SubItems[3].Text = value.ToState();
            }
        }

        public PicListViewItem(string path)
        {
            for(int i = 0; i < 3; i++)
            {
                this.SubItems.Add(new ListViewSubItem());
            }
            this.UseItemStyleForSubItems = false;
            FileInfo fileInfo = new FileInfo(path);
            this.FullPath = fileInfo.FullName;
            this.FileName = fileInfo.Name;
            this.Size = fileInfo.Length >> 10;//单位:KB
            this.State = PicState.Waiting;
        }

        public void SetSuccess()
        {
            this.State = PicState.Success;
            this.SubItems[3].ForeColor = System.Drawing.Color.Green;
        }

        public void SetError()
        {
            this.State = PicState.Error;
            this.SubItems[3].ForeColor = System.Drawing.Color.Red;
        }

        public override bool Equals(object obj)
        {
            return obj is PicListViewItem picListViewItem
                ? this.FullPath == picListViewItem.FullPath
                : false;
        }
    }
}
