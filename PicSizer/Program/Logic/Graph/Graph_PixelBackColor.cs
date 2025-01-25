#region

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using PicSizer.Program.FileIO;
using PicSizer.Program.Static;

#endregion

namespace PicSizer.Program.Logic.Graph
{
    public static partial class Graph
    {
        /// <summary>
        /// 调整透明像素颜色(ARGB格式,一个像素4字节)
        /// </summary>
        public static void SetTransparentPixelBackgroundColor(ref Bitmap bitmap)
        {
            if (PicSetting.UseGPU && PicValue.IsGpuSupport)
            {
                _SetTransparentPixelBackgroundColorByCUDA(bitmap);
            }
            else
            {
                _SetTransparentPixelBackgroundColorByCPP(bitmap);
            }
        }

        private static void _SetTransparentPixelBackgroundColorByCUDA(Bitmap bitmap)
        {
            int width = bitmap.Width, height = bitmap.Height;
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            int stride = bitmapData.Stride; //图片扫描宽度
            IntPtr ptr = bitmapData.Scan0;
            if (!Dll.CUDA_SetTransparentPixelBackgroundColor(
                    ptr, width, height, stride,
                    PicSetting.BackgroundColor[0],
                    PicSetting.BackgroundColor[1],
                    PicSetting.BackgroundColor[2]))
            {
                throw new Exception("使用GPU加速时遇到了未知错误");
            }

            bitmap.UnlockBits(bitmapData);
        }

        private static void _SetTransparentPixelBackgroundColorByCPP(Bitmap bitmap)
        {
            int width = bitmap.Width, height = bitmap.Height;
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            int stride = bitmapData.Stride; //图片扫描宽度
            IntPtr ptr = bitmapData.Scan0;
            if (!Dll.CPP_SetTransparentPixelBackgroundColor(
                    ptr, width, height, stride,
                    PicSetting.BackgroundColor[0],
                    PicSetting.BackgroundColor[1],
                    PicSetting.BackgroundColor[2]))
            {
                throw new Exception("使用GPU加速时遇到了未知错误");
            }

            bitmap.UnlockBits(bitmapData);
        }

        private static void _SetTransparentPixelBackgroundColorByCSharp(Bitmap bitmap)
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
            byte R = PicSetting.BackgroundColor[0];
            byte G = PicSetting.BackgroundColor[1];
            byte B = PicSetting.BackgroundColor[2];
            int position;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    position = y * stride + x * 4; //像素起始地址(格式BGRA)
                    byte alpha = pic[position + 3]; //透明通道
                    if (alpha == 255) continue; //如果是完全不透明则跳过
                    pic[position + 3] = 255; //修改为完全不透明
                    //修改RGB颜色
                    pic[position + 2] = (byte)((pic[position + 2] * alpha + R * (255 - alpha)) / 255);
                    pic[position + 1] = (byte)((pic[position + 1] * alpha + G * (255 - alpha)) / 255);
                    pic[position] = (byte)((pic[position] * alpha + B * (255 - alpha)) / 255);
                }
            }

            Marshal.Copy(pic, 0, ptr, size);
            bitmap.UnlockBits(bitmapData);
        }
    }
}