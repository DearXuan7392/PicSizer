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
            //当亮度为100时跳过该函数
            if (SharedVariable.setting.brightness == 100) return;
            _SetBrightnessByCSharp(bitmap);
            
            //根据GPU是否支持来决定使用GPU加速还是CPU计算
            if (SharedVariable.setting.useGPU && Info.isGPUSupport)
            {
                _SetBrightnessByCUDA(bitmap);
            }
            else
            {
                _SetBrightnessByCSharp(bitmap);
            }
        }
        
        /// <summary>
        /// 使用GPU加速调整亮度
        /// </summary>
        private static void _SetBrightnessByCUDA(Bitmap bitmap)
        {
            int width = bitmap.Width, height = bitmap.Height;
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);
            int length = width * height;
            IntPtr ptr = bitmapData.Scan0;
            if (!Partial.DllExtern.SetBrightness(ptr, length, SharedVariable.setting.brightness))
            {
                throw new Exception("使用GPU加速时遇到了未知错误");
            }
            bitmap.UnlockBits(bitmapData);
        }
        
        /// <summary>
        /// 使用CPU调整亮度
        /// </summary>
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
                pic[i] = (byte)(pic[i] * SharedVariable.setting.brightness / 100);
            }
            Marshal.Copy(pic, 0, ptr, size);
            bitmap.UnlockBits(bitmapData);
        }
    }
}
