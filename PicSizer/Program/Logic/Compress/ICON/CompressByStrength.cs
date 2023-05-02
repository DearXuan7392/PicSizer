using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PicSizer.Static;
using System.Runtime.InteropServices;
using PicSizer.FileIO;

namespace PicSizer.Logic
{
    public partial class ICON
    {
        /// <summary>
        /// 以指定压缩强度输出JPEG图片
        /// </summary>
        /// <param name="strength">压缩强度</param>
        /// <param name="output">输出路径</param>
        public void Compress_By_Strength(int strength, string output)
        {
            strength = 101 - strength;
            if (PicSetting.UseGPU && PicValue.IsGPUSupport)
            {
                _CompressByCUDA(ref bitmap, strength);
            }
            else
            {
                _CompressByCPP(ref bitmap, strength);
            }
            
            bitmap.Save(output, Encoder._Info_PNG, null);
        }

        private static void _CompressByCPP(ref Bitmap bitmap, int strength)
        {
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);
            // 单个像素长度
            int pixelBits = bitmap.PixelFormat == PixelFormat.Format24bppRgb
                ? 3 // 24 位 RGB 格式
                : 4;// 32  位 ARGB 格式
            // 像素所在内存区域起始地址
            IntPtr ptr = bitmapData.Scan0;
            DLL.CPP_CompressPNG(ptr, bitmap.Width, bitmap.Height, bitmapData.Stride, pixelBits, strength);
            bitmap.UnlockBits(bitmapData);
        }

        private static void _CompressByCUDA(ref Bitmap bitmap, int strength)
        {
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);
            // 单个像素长度
            int pixelBits = bitmap.PixelFormat == PixelFormat.Format24bppRgb
                ? 3 // 24 位 RGB 格式
                : 4;// 32  位 ARGB 格式
            // 像素所在内存区域起始地址
            IntPtr ptr = bitmapData.Scan0;
            DLL.CUDA_CompressPNG(ptr, bitmap.Width, bitmap.Height, bitmapData.Stride, pixelBits, strength);
            bitmap.UnlockBits(bitmapData);
        }

        /// <summary>
        /// 压缩PNG(CSharp)
        /// </summary>
        private static void _CompressByCSharp(ref Bitmap bitmap, int strength)
        {
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);
            // 单个像素长度
            int pixelBits = bitmap.PixelFormat == PixelFormat.Format24bppRgb
                ? 3 // 24 位 RGB 格式
                : 4;// 32  位 ARGB 格式
            // 图片扫描宽度
            int stride = bitmapData.Stride;
            // 图片占用总内存大小
            int size = stride * bitmap.Height;
            // 定义 byte 数组来存放图片像素
            byte[] pic = new byte[size];
            // 像素所在内存区域起始地址
            IntPtr ptr = bitmapData.Scan0;
            // 将像素复制到 byte 数组中
            Marshal.Copy(ptr, pic, 0, size);
            // 偏移值,防止图片过暗
            int offset = strength / 2;
            for (int x = 0; x < bitmap.Width * pixelBits; x++)
            {
                for(int y = 0; y < bitmap.Height; y++)
                {
                    int position = y * stride + x;
                    // 当前像素是 32 位像素格式里的透明像素,且值为 0 或 255,则跳过
                    if (pixelBits == 4 && x % 4 == 3 &&
                        (pic[position] == 255 || pic[position] == 0))
                    {
                        continue;
                    }
                    int result = pic[position] / strength * strength + offset;
                    pic[position] = result <= 255
                        ? (byte)result
                        : (byte)255;

                }
            }
            Marshal.Copy(pic, 0, ptr, size);
            bitmap.UnlockBits(bitmapData);
        }
    }
}
