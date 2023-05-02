using PicSizer.FileIO;
using PicSizer.Static;
using PicSizer.Window.Partial;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PicSizer.Logic
{
    public static partial class Graph
    {
        /// <summary>
        /// 修改亮度(RGB格式,一个像素3字节)
        /// </summary>
        public static void SetBrightness(ref Bitmap bitmap)
        {
            //当亮度为100时跳过该函数
            if (PicSetting.Brightness == 100) return;
            //根据GPU是否支持来决定使用GPU加速还是CPU计算
            if (PicSetting.UseGPU)
            {
                _SetBrightnessByCUDA(bitmap); // 通过CUDA执行
            }
            else
            {
                _SetBrightnessByCSharp(bitmap); // 通过C#执行
            }
        }

        /// <summary>
        /// 使用GPU加速调整亮度(RGB格式,一个像素3字节)
        /// </summary>
        private static void _SetBrightnessByCUDA(Bitmap bitmap)
        {
            int width = bitmap.Width, height = bitmap.Height;
            // 在内存中以读写方式锁定像素
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);
            // 获取像素长度
            int pixelBits = GetPixelBits(ref bitmap);
            // 图片扫描宽度
            int stride = bitmapData.Stride;
            // 内存起始地址
            IntPtr ptr = bitmapData.Scan0;
            if (!DLL.CUDA_SetBrightness(ptr, width, height, stride, PicSetting.Brightness))
            {
                throw new Exception("使用GPU加速时遇到了未知错误");
            }
            bitmap.UnlockBits(bitmapData);
        }

        /// <summary>
        /// 使用CPU调整亮度(RGB格式,一个像素3字节)
        /// </summary>
        private static void _SetBrightnessByCSharp(Bitmap bitmap)
        {
            int width = bitmap.Width, height = bitmap.Height;
            // 在内存中以读写方式锁定像素
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);
            // 获取像素长度
            int pixelBits = GetPixelBits(ref bitmap);
            // 图片扫描宽度
            int stride = bitmapData.Stride;
            // 内存中的像素数据长度
            int size = stride * height;
            // 定义数组以保存像素
            byte[] pic = new byte[size];
            // 将内存中的像素值复制到数组里
            IntPtr ptr = bitmapData.Scan0;
            Marshal.Copy(ptr, pic, 0, size);
            int brightness = PicSetting.Brightness;
            for (int x = 0; x < width * pixelBits; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if(pixelBits == 4 && x % 4 == 3)
                    {
                        // 跳过Alpha通道
                        continue;
                    }
                    else
                    {
                        int position = y * stride + x;
                        pic[position] = (byte)(pic[position] * brightness / 100);
                    }
                }
            }
            // 复制回内存中并解锁像素
            Marshal.Copy(pic, 0, ptr, size);
            bitmap.UnlockBits(bitmapData);
        }

        /// <summary>
        /// 使用C++加速调整亮度(RGB格式,一个像素3字节)
        /// </summary>
        private static void _SetBrightnessByCPP(Bitmap bitmap)
        {
            int width = bitmap.Width, height = bitmap.Height;
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);
            int stride = bitmapData.Stride;//图片扫描宽度
            IntPtr ptr = bitmapData.Scan0;
            if (!DLL.CPP_SetBrightness(ptr, width, height, stride, PicSetting.Brightness))
            {
                throw new Exception("使用GPU加速时遇到了未知错误");
            }
            bitmap.UnlockBits(bitmapData);
        }
    }
}
