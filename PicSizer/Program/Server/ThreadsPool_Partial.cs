#region

using System.Runtime.CompilerServices;
using PicSizer.Program.FileIO;
using PicSizer.Program.Static;
using PicSizer.Program.Window.Assemble;

#endregion

namespace PicSizer.Program.Server
{
    public static partial class ThreadsPool
    {
        /// <summary>
        /// PicListView里的图片下标
        /// </summary>
        private static int _indexOfPicListView;

        /// <summary>
        /// 输出的图片下标
        /// </summary>
        private static int _indexOfOutput = 1;

        /// <summary>
        /// 获取下一个图片信息
        /// </summary>
        /// <param name="item">所在的PicListViewItem对象</param>
        /// <param name="input">文件名</param>
        /// <param name="output">输出文件名</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static bool GetNextOutputPictureInfo(out PicListViewItem item, out string input, out string output)
        {
            //如果当前下标超出了PicListView范围 [0, Count - 1],则返回 false
            if (_indexOfPicListView >= PicValue.PicListView.Items.Count)
            {
                item = null;
                input = null;
                output = null;
                return false;
            }

            item = PicValue.PicListView[_indexOfPicListView];
            input = item.SubItems[1].Text;
            //控制主界面滚动条下移
            PicValue.PicListView.EnsureVisible(_indexOfPicListView);
            //根据不同的输出方式生成输出文件名
            output = OutputPath.GetOutputPath(input, _indexOfOutput);
            _indexOfPicListView++;
            _indexOfOutput++;
            return true;
        }
    }
}