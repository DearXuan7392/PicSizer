using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PicSizer
{
    public static class BmpProc
    {
        public static void SetBrightness(Bitmap bitmap)
        {
            if (Setting.brightness == 100) return;
            if (Setting.useGPU && Info.isGPUSupport)
            {
                _SetBrightnessByCUDA(bitmap);
            }
            else
            {
                _SetBrightnessByCSharp(bitmap);
            }
        }

        private static void _SetBrightnessByCUDA(Bitmap bitmap)
        {
            int width = bitmap.Width, height = bitmap.Height;
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);
            int length = width * height;
            IntPtr ptr = bitmapData.Scan0;
            if (!DllExtern.SetBrightness(ptr, length, Setting.brightness))
            {
                throw new Exception("使用GPU加速时遇到了未知错误");
            }
            bitmap.UnlockBits(bitmapData);
        }

        private unsafe static void _SetBrightnessByCSharp(Bitmap bitmap)
        {
            int width = bitmap.Width, height = bitmap.Height;
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);
            int size = width * height * 3;
            byte[] pic = new byte[size];
            IntPtr ptr = bitmapData.Scan0;
            Marshal.Copy(ptr, pic, 0, size);
            for(int i = 0; i < size; i++)
            {
                pic[i] = (byte)(pic[i] * Setting.brightness / 100);
            }
            Marshal.Copy(pic, 0, ptr, size);
            bitmap.UnlockBits(bitmapData);
        }
    }
}
