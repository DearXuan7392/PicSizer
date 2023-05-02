using PicSizer.Static;
using PicSizer.Window.Assemble;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PicSizer.Server
{
    public static partial class ThreadsPool
    {
        /// <summary>
        /// PicListView里的图片下标
        /// </summary>
        private static int Index_Of_PicListView = 0;

        /// <summary>
        /// 输出的图片下标
        /// </summary>
        private static int Index_Of_Output = 1;

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
            if(Index_Of_PicListView >= PicValue.picListView.Items.Count)
            {
                item = null;
                input = null;
                output = null;
                return false;
            }
            item = PicValue.picListView[Index_Of_PicListView];
            input = item.SubItems[1].Text;
            //控制主界面滚动条下移
            PicValue.picListView.EnsureVisible(Index_Of_PicListView);
            //根据不同的输出方式生成输出文件名
            output = FileIO.OutputPath.GetOutputPath(input, Index_Of_Output);
            Index_Of_PicListView++;
            Index_Of_Output++;
            return true;
        }
    }
}
