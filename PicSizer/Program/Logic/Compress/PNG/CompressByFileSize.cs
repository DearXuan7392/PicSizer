using PicSizer.Static;
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
    public partial class PNG
    {
        /// <summary>
        /// 将图片压缩到指定大小之内
        /// </summary>
        /// <param name="bitmap">图片</param>
        /// <param name="limit">输出文件大小(KB)</param>
        /// <param name="output">输出路径</param>
        public void Compress_By_FileSize(long limit, string output)
        {
            int left = 1, right = 100, mid = 0;
            //将压缩强度分为100个等级sizeList[1-100]
            long[] sizeList = new long[101];
            while (left < right - 1)
            {
                //取left和right中间值
                mid = (left + right) / 2;
                //获取当前画质下的图片大小,并存入数组
                sizeList[mid] = GetBitmapSize(bitmap, mid);
                //根据大小往左或往右查找
                if (sizeList[mid] <= (limit - 1) * 1024)
                {
                    left = mid;
                }
                else
                {
                    right = mid;
                }
            }
            //退出循环时，left就是不大于limit的最大值
            //获取画质为left时的大小，即是不大于LimitSize的最高画质
            if (sizeList[left] == 0)
            {
                sizeList[left] = GetBitmapSize(bitmap, left);
            }
            //如果文件大小符合要求或者接受超出限制的文件就输出
            if (sizeList[left] <= (limit - 1) * 1024 || PicSetting.AcceptExceedPicture)
            {
                _CompressByCSharp(bitmap, left);
                bitmap.Save(output, Encoder._Info_PNG, null);
            }
            else
            {
                throw new Exception(PicStrings.COMPRESS_CANNOT_RESIZE_TO_LIMIT_SIZE);
            }
        }

        /// <summary>
        /// 计算指定画质下Bitmap输出到流后的大小
        /// </summary>
        public static long GetBitmapSize(Bitmap bitmap, int value)
        {
            MemoryStream memoryStream = null;
            Bitmap newCopy = null;
            try
            {
                memoryStream = new MemoryStream();
                newCopy = (Bitmap)bitmap.Clone();
                _CompressByCPP(bitmap, value);
                newCopy.Save(memoryStream, Encoder._Info_PNG, null);
                long size = memoryStream.Length;
                return size;
            }
            finally
            {
                memoryStream?.Dispose();
                newCopy?.Dispose();
            }
        }
    }
}
