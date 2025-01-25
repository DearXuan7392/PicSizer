#region

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using PicSizer.Program.FileIO;

#endregion

namespace PicSizer.Program.Logic.Compress
{
    public class Icon : CompressItem
    {
        public Icon(string imgPath, string outputFilename)
        {
            Init(imgPath, outputFilename);
        }

        protected override void WriteToStreamWithQuality(Stream stream, int quality)
        {
            Bitmap copy = null;
            try
            {
                copy = (Bitmap)Img.Clone();
                var compressStrength = MaxQuality - quality + 1;
                _CompressByCPP(copy, compressStrength);
                copy.Save(stream, Encoder.InfoPng, null);
            }
            finally
            {
                copy?.Dispose();
            }
        }

        private static void _CompressByCPP(Bitmap bitmap, int strength)
        {
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);
            // 单个像素长度
            int pixelBits = bitmap.PixelFormat == PixelFormat.Format24bppRgb
                ? 3 // 24 位 RGB 格式
                : 4; // 32  位 ARGB 格式
            // 像素所在内存区域起始地址
            IntPtr ptr = bitmapData.Scan0;
            Dll.CPP_CompressPNG(ptr, bitmap.Width, bitmap.Height, bitmapData.Stride, pixelBits, strength);
            bitmap.UnlockBits(bitmapData);
        }

        private static void _CompressByCUDA(Bitmap bitmap, int strength)
        {
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);
            // 单个像素长度
            int pixelBits = bitmap.PixelFormat == PixelFormat.Format24bppRgb
                ? 3 // 24 位 RGB 格式
                : 4; // 32  位 ARGB 格式
            // 像素所在内存区域起始地址
            IntPtr ptr = bitmapData.Scan0;
            Dll.CUDA_CompressPNG(ptr, bitmap.Width, bitmap.Height, bitmapData.Stride, pixelBits, strength);
            bitmap.UnlockBits(bitmapData);
        }

        /// <summary>
        /// 压缩PNG(CSharp)
        /// </summary>
        private static void _CompressByCSharp(Bitmap bitmap, int strength)
        {
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);
            // 单个像素长度
            int pixelBits = bitmap.PixelFormat == PixelFormat.Format24bppRgb
                ? 3 // 24 位 RGB 格式
                : 4; // 32  位 ARGB 格式
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
                for (int y = 0; y < bitmap.Height; y++)
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