#region

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using PicSizer.Program.Logic.Compress;
using PicSizer.Program.Static;

#endregion

namespace PicSizer.Program.FileIO
{
    public static class FileProc
    {
        /// <summary>
        /// 获取文件后缀名(包括点号,小写字母)
        /// </summary>
        public static string GetExtension(string path)
        {
            //查找点号位置
            int i = path.LastIndexOf('.');
            //没有后缀名，直接返回非法
            if (i == -1) return string.Empty;
            //截取后缀名
            return path.Substring(i).ToLower();
        }

        /// <summary>
        /// 把文件大小转换成字符串(单位:KB)
        /// </summary>
        public static string FileSizeToString(long size)
        {
            if (size < 1024)
            {
                return size + " KB";
            }

            return (size / 1024.0).ToString("#0.00") + " MB";
        }

        public static Bitmap GetBitmap(string imgPath)
        {
            MemoryStream ms = null;
            byte[] byteArray = File.ReadAllBytes(imgPath);
            try
            {
                ms = new MemoryStream(byteArray);
                if (CheckByte(ref byteArray, WebpBytes))
                {
                    var webpImg = Webp.GetBitmap(ms);
                    ms.Dispose();
                    return webpImg;
                }

                var bitmap = new Bitmap(ms);
                Bitmap copy = null;
                switch (bitmap.PixelFormat)
                {
                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppArgb:
                        //已经是24位RGB或32位ARGB,则什么也不做
                        break;
                    case PixelFormat.Format16bppArgb1555:
                    case PixelFormat.Format32bppPArgb:
                    case PixelFormat.Format64bppArgb:
                    case PixelFormat.Format64bppPArgb:
                        //含有Alpha通道,设置为32位ARGB格式
                        copy = bitmap.Clone(
                            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                            PixelFormat.Format32bppArgb);
                        ms.Dispose();
                        break;
                    case PixelFormat.Format16bppGrayScale:
                    case PixelFormat.Format16bppRgb555:
                    case PixelFormat.Format16bppRgb565:
                    case PixelFormat.Format32bppRgb:
                    case PixelFormat.Format48bppRgb:
                        //不含Alpha通道,设置为24位RGB格式
                        copy = bitmap.Clone(
                            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                            PixelFormat.Format24bppRgb);
                        ms.Dispose();
                        break;
                    default:
                        throw new Exception("未知的像素格式");
                }

                if (copy == null)
                {
                    return bitmap;
                }

                bitmap.Dispose();
                return copy;
            }
            catch (Exception e)
            {
                ms?.Dispose();
                throw;
            }
        }

        private static readonly byte[] JpegBytes = { 0xff, 0xd8 };
        private static readonly byte[] PngBytes = { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a };
        private static readonly byte[] GifBytes = { 0x47, 0x49, 0x46, 0x38 };
        private static readonly byte[] BmpBytes = { 0x42, 0x4d };
        private static readonly byte[] Tiff1Bytes = { 0x49, 0x49 };
        private static readonly byte[] Tiff2Bytes = { 0x4d, 0x4d };
        private static readonly byte[] IcoBytes = { 0x00, 0x00, 0x01, 0x00 };
        private static readonly byte[] WebpBytes = { 0x52, 0x49, 0x46, 0x46 };

        private static bool CheckByte(ref byte[] data, byte[] format)
        {
            int len = format.Length;
            for (int i = 0; i < len; i++)
            {
                if (data[i] != format[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static byte[] GetFirstBytes(FileStream fileStream, int length = 8)
        {
            if (fileStream.Length < length)
            {
                throw new Exception(PicStrings.BrokenFile);
            }
            byte[] data = new byte[length];
            fileStream.Position = 0;
            fileStream.Read(data, 0, length);
            return data;
        }
    }
}