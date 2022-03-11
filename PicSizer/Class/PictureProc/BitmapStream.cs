using PicSizer.Class.Partial;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PicSizer.Class.Static;

namespace PicSizer.Class.PictureProc
{
    /// <summary>
    /// Bitmap流式输出(非JPEG)
    /// </summary>
    public static class BitmapStream
    {
        /// <summary>
        /// Icon的文件标头
        /// </summary>
        private static readonly byte[] _ICON_HEADER = { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 32, 0, 0, 0, 0, 0, 22, 0, 0, 0 };

        /// <summary>
        /// 把图片保存到内存中的大小(单位:字节)
        /// </summary>
        private static long LengthOfBitmapInMemory(Bitmap bitmap, ImageFormat imageFormat)
        {
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, imageFormat);
                return memoryStream.Length;
            }
            finally
            {
                memoryStream?.Dispose();
            }
        }
        
        /// <summary>
        /// 计算指定画质下Bitmap输出到流后的大小(仅限JPEG)
        /// </summary>
        public static long GetBitmapSizeByQuality(Bitmap bitmap, EncoderParameters encoderParameters, ImageCodecInfo info, long value)
        {
            encoderParameters.Param[0] = Encoder.GetParameter(value);
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, info, encoderParameters);
                long size = memoryStream.Length;
                return size;
            }
            finally
            {
                memoryStream?.Dispose();
            }
        }
        
        /// <summary>
        /// 计算指定位深度下Bitmap输出到流后的大小(非JPEG)
        /// </summary>
        public static long GetBitmapSizeByPixelDeep(Bitmap bitmap, ImageFormat imageFormat, Rectangle rect, PixelFormat pixelFormat)
        {
            Bitmap output = null;
            try
            {
                output = bitmap.Clone(rect, pixelFormat);//用指定的位深度复制Bitmap
                return LengthOfBitmapInMemory(output, imageFormat);
            }
            finally
            {
                output?.Dispose();
            }
        }
        
        /// <summary>
        /// 计算指定分辨率下Bitmap输出到流后的大小(非JPEG)
        /// </summary>
        public static long GetBitmapSizeByDpi(Bitmap bitmap, ImageFormat imageFormat, float xDpi, float yDpi)
        {
            Bitmap output = null;
            try
            {
                output = new Bitmap(bitmap);
                output.SetResolution(xDpi, yDpi);
                return LengthOfBitmapInMemory(output, imageFormat);
            }
            finally
            {
                output?.Dispose();
            }
        }
        
        /// <summary>
        /// 计算指定缩放比例下Bitmap输出到流后的大小(非JPEG)
        /// </summary>
        public static long GetBitmapSizeByScale(Bitmap bitmap, ImageFormat imageFormat, int width, int height)
        {
            Bitmap output = null;
            try
            {
                output = bitmap.Clone(new Rectangle(0, 0, width, height), bitmap.PixelFormat);
                return LengthOfBitmapInMemory(output, imageFormat);
            }
            finally
            {
                output?.Dispose();
            }
        }

        /// <summary>
        /// 将Bitmap保存到Icon文件
        /// </summary>
        public static void SaveBitmapToFile_Icon(Bitmap bitmap, string outputFilename)
        {
            FileStream fileStream = null;
            BinaryWriter writer = null;
            byte width = (byte)bitmap.Width;
            byte height = (byte)bitmap.Height;
            try
            {
                fileStream = new FileStream(outputFilename, FileMode.Create);
                writer = new BinaryWriter(fileStream);
                writer.Write(_ICON_HEADER);//写入标头
                bitmap.Save(fileStream, ImageFormat.Png);//主体部分写入文件流
                //偏移0x6处为图片宽度
                writer.Seek(0x6, SeekOrigin.Begin);
                writer.Write(width);
                //偏移0x7处为图片高度
                writer.Seek(0x7, SeekOrigin.Begin);
                writer.Write(height);
                //偏移0xE处为图片主体部分长度，应减去标头
                writer.Seek(0xE, SeekOrigin.Begin);
                writer.Write((int)fileStream.Length - 22);
            }
            finally
            {
                writer?.Dispose();
                fileStream?.Dispose();
            }
        }
    }
}
