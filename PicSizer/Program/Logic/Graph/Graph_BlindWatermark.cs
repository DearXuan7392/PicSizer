#region

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

#endregion

namespace PicSizer.Program.Logic.Graph
{
    public static partial class Graph
    {
        /// <summary>
        /// 添加暗水印
        /// </summary>
        private static void _AddBlindWatermarkByCSharp(Bitmap bitmap, Bitmap watermark)
        {
            int width = bitmap.Width, height = bitmap.Height;
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            int stride = bitmapData.Stride; //图片扫描宽度
            int size = stride * height;
            byte[] pic = new byte[size];
            IntPtr ptr = bitmapData.Scan0;
            Marshal.Copy(ptr, pic, 0, size);
            int position;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    position = y * stride + x * 4; //像素起始地址(格式BGRA)
                }
            }
        }
    }
}