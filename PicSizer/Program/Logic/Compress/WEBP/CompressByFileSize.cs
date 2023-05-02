using PicSizer.Static;
using PicSizer.Window.Partial;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Logic
{
    public partial class WEBP
    {
        /// <summary>
        /// 将图片压缩到指定大小之内
        /// </summary>
        /// <param name="limit">输出文件大小(KB)</param>
        /// <param name="output">输出路径</param>
        public void Compress_By_FileSize(long limit, string output)
        {
            long left = 0, right = 100, mid = 50;
            // 将画质分为 100 个等级: 1-100
            long[] sizeList = new long[101];

            // 寻找不大于 limit 的最优值, sizeList[mid] 不一定与 limit 严格相等
            while (left < right - 1)
            {
                // 获取下一个值
                mid = (left + right) / 2;
                // 获取当前画质下的图片大小,并存入数组
                sizeList[mid] = sizeList[mid] == 0
                    ? GetBitmapSize(ref bitmap, left)
                    : sizeList[mid];
                // 根据大小往左或往右查找
                if (sizeList[mid] <= limit * 1024)
                {
                    left = mid;
                }
                else
                {
                    right = mid;
                }
            }
            // 退出循环时，sizeList[left] 就是不大于 limit 的最大值
            // 获取画质为 left 时的大小，即是不大于 LimitSize 的最高画质
            sizeList[left] = sizeList[left] == 0
                    ? GetBitmapSize(ref bitmap, left)
                    : sizeList[left];
            // 如果文件大小符合要求或者接受超出限制的文件就输出
            if (sizeList[left] <= (limit - 1) * 1024 || PicSetting.AcceptExceedPicture)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(output, FileMode.Create, FileAccess.Write);
                    long length = BitmapToStream(bitmap, fs, (int)left);
                    fs.Close();
                }
                finally
                {
                    fs?.Dispose();
                }
            }
            else
            {
                throw new Exception(PicStrings.COMPRESS_CANNOT_RESIZE_TO_LIMIT_SIZE);
            }
        }

        /// <summary>
        /// 计算指定画质下 Bitmap 输出到流后的大小
        /// </summary>
        private static long GetBitmapSize(ref Bitmap bitmap, long value)
        {
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream();
                long length = BitmapToStream(bitmap, ms, (int)value);
                ms.Close();
                return length;
            }
            finally
            {
                ms?.Dispose();
            }
        }
    }
}
