using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using PicSizer.Class.Static;

namespace PicSizer.Class.PictureProc
{
    public static class Graphy
    {
        /// <summary>
        /// 修改亮度(RGB格式,一个像素3字节)
        /// </summary>
        public static void SetBrightness(Bitmap bitmap)
        {
            //当亮度为100时跳过该函数
            if (Value.setting.brightness == 100) return;
            //根据GPU是否支持来决定使用GPU加速还是CPU计算
            if (Value.setting.useGPU && Info.isGPUSupport)
            {
                _SetBrightnessByCUDA(bitmap);
            }
            else
            {
                _SetBrightnessByCSharp(bitmap);
            }
        }

        /// <summary>
        /// 调整透明像素颜色(ARGB格式,一个像素4字节)
        /// </summary>
        public static void SetAlphaPixelColor(Bitmap bitmap)
        {
            if(Value.setting.useGPU && Info.isGPUSupport)
            {
                _SetAlphaPixelColorByCUDA(bitmap);
            }
            else
            {
                _SetAlphaPixelColorByCSharp(bitmap);
            }
        }

        /// <summary>
        /// 添加水印
        /// </summary>
        public static void AddWatermark(Bitmap bitmap)
        {

        }

        /// <summary>
        /// 使用GPU加速调整亮度(RGB格式,一个像素3字节)
        /// </summary>
        private static void _SetBrightnessByCUDA(Bitmap bitmap)
        {
            int width = bitmap.Width, height = bitmap.Height;
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);
            int stride = bitmapData.Stride;//图片扫描宽度
            IntPtr ptr = bitmapData.Scan0;
            if (!Partial.DllExtern.SetBrightness(ptr, width, height, stride, Value.setting.brightness))
            {
                throw new Exception("使用GPU加速时遇到了未知错误");
            }
            bitmap.UnlockBits(bitmapData);
        }

        /// <summary>
        /// 使用CPU调整亮度(RGB格式,一个像素3字节)
        /// </summary>
        private unsafe static void _SetBrightnessByCSharp(Bitmap bitmap)
        {
            int width = bitmap.Width, height = bitmap.Height;
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);
            int stride = bitmapData.Stride;//图片扫描宽度
            int size = stride * height;
            byte[] pic = new byte[size];
            IntPtr ptr = bitmapData.Scan0;
            Marshal.Copy(ptr, pic, 0, size);
            int rowLength = width * 3;//每行的有效像素位字节数
            int brightness = Value.setting.brightness;
            for (int y = 0; y < height; ++y)
            {
                for(int x = 0; x < rowLength; ++x)
                {
                    int position = y * stride + x;
                    pic[position] = (byte)(pic[position] * brightness / 100);
                }
            }
            Marshal.Copy(pic, 0, ptr, size);
            bitmap.UnlockBits(bitmapData);
        }

        private static void _SetAlphaPixelColorByCUDA(Bitmap bitmap)
        {
            int width = bitmap.Width, height = bitmap.Height;
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            int stride = bitmapData.Stride;//图片扫描宽度
            IntPtr ptr = bitmapData.Scan0;
            if (!Partial.DllExtern.SetAlphaPixelColor(ptr, width, height, stride, Value.setting.backgroundColor[0], Value.setting.backgroundColor[1], Value.setting.backgroundColor[2]))
            {
                throw new Exception("使用GPU加速时遇到了未知错误");
            }
            bitmap.UnlockBits(bitmapData);
        }

        private unsafe static void _SetAlphaPixelColorByCSharp(Bitmap bitmap)
        {
            int width = bitmap.Width, height = bitmap.Height;
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            int stride = bitmapData.Stride;//图片扫描宽度
            int size = stride * height;
            byte[] pic = new byte[size];
            IntPtr ptr = bitmapData.Scan0;
            Marshal.Copy(ptr, pic, 0, size);
            byte R = Value.setting.backgroundColor[0];
            byte G = Value.setting.backgroundColor[1];
            byte B = Value.setting.backgroundColor[2];
            int position;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    position = y * stride + x * 4;//像素起始地址(格式BGRA)
                    byte alpha = pic[position + 3];//透明通道
                    if (alpha == 255) continue;//如果是完全不透明则跳过
                    pic[position + 3] = 255;//修改为完全不透明
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
