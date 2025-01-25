#region

using System.IO;
using System.Windows.Forms;

#endregion

namespace PicSizer.Program.Window.Assemble
{
    public partial class PicListView : ListView
    {
        /// <summary>
        /// 索引器
        /// </summary>
        public PicListViewItem this[int index]
        {
            get { return (PicListViewItem)Items[index]; }
            set { Items[index] = value; }
        }

        /// <summary>
        /// 从单个文件夹加载图片,递归方式
        /// </summary>
        public int AddPicturesFromDirectory(string dir)
        {
            BeginUpdate();
            int success = _AddPicturesFromDirectory(dir);
            EndUpdate();
            FormsControl.MainForm.UpdateSelectTotalNumLabel();
            return success;

            int _AddPicturesFromDirectory(string _dir)
            {
                int _success = 0;
                DirectoryInfo directoryInfo = new DirectoryInfo(_dir);
                FileInfo[] fileInfos = directoryInfo.GetFiles(); // 获取该目录下图片
                DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories(); // 获取该目录下文件夹
                //加入所有图片
                foreach (FileInfo fileInfo in fileInfos)
                {
                    _success += AddPicturesFromPath(fileInfo.FullName);
                }

                //递归遍历文件夹
                foreach (DirectoryInfo directory in directoryInfos)
                {
                    _success += _AddPicturesFromDirectory(directory.FullName);
                }

                return _success;
            }
        }

        /// <summary>
        /// 从路径数组加载图片
        /// </summary>
        public int AddPicturesFromPath(string[] picList)
        {
            int success = 0;
            BeginUpdate();
            //拆分开一张一张加入
            foreach (string pic in picList)
            {
                success += AddPicturesFromPath(pic);
            }

            EndUpdate();
            FormsControl.MainForm.UpdateSelectTotalNumLabel();
            return success;
        }

        /// <summary>
        /// 从单个路径加载图片
        /// </summary>
        private int AddPicturesFromPath(string path)
        {
            //检测图片是否存在
            if (ItemPathHashSet.Contains(path))
            {
                return 0;
            }

            //添加到列表
            PicListViewItem item = new PicListViewItem(path);
            Items.Add(item);
            //添加到集合
            ItemPathHashSet.Add(path);
            return 1;
        }
    }
}