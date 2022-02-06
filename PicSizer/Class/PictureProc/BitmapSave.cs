using PicSizer.Partial;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Class.PictureProc
{
    /// <summary>
    /// 保存Bitmap(非JPEG)
    /// </summary>
    public static class BitmapSave
    {
        /// <summary>
        /// Icon的文件标头
        /// </summary>
        private static readonly byte[] _ICON_HEADER = new byte[] { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 32, 0, 0, 0, 0, 0, 22, 0, 0, 0 };

        /// <summary>
        /// 把Bitmap保存到文件
        /// </summary>
        public static void SaveBitmapToFile(Bitmap bitmap, string path, ImageFormat imageFormat)
        {
            if(imageFormat == ImageFormat.Icon)
            {
                SaveBitmapToFile_Icon(bitmap, path, Value.setting.IconLimitSize);
            }
            else
            {
                SaveBitmapToFile_NonIcon(bitmap, path, imageFormat);
            }
        }

        /// <summary>
        /// 把图片保存到内存中的大小
        /// </summary>
        public static long LengthOfBitmapInMemory(Bitmap bitmap, ImageFormat imageFormat)
        {
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, imageFormat);
                return memoryStream.Length >> 10;
            }
            finally
            {
                memoryStream?.Dispose();
            }
        }

        /// <summary>
        /// 将Bitmap保存到非Icon文件
        /// </summary>
        private static void SaveBitmapToFile_NonIcon(Bitmap bitmap, string path, ImageFormat imageFormat)
        {
            bitmap.Save(path, imageFormat);
        }

        /// <summary>
        /// 将Bitmap保存到Icon文件
        /// </summary>
        private static void SaveBitmapToFile_Icon(Bitmap bitmap, string path, byte size)
        {
            FileStream fileStream = null;
            BinaryWriter writer = null;
            try
            {
                fileStream = new FileStream(path, FileMode.Create);
                writer = new BinaryWriter(fileStream);
                writer.Write(_ICON_HEADER);//写入标头
                bitmap.Save(fileStream, ImageFormat.Png);//主体部分写入文件流
                //偏移0x6处为图片宽度
                writer.Seek(0x6, SeekOrigin.Begin);
                writer.Write(size);
                //偏移0x7处为图片高度
                writer.Seek(0x7, SeekOrigin.Begin);
                writer.Write(size);
                //偏移0xE处为图片主体部分长度，应减去标头
                writer.Seek(0xE, SeekOrigin.Begin);
                writer.Write((int)fileStream.Length - 14);
            }
            finally
            {
                writer?.Dispose();
                fileStream?.Dispose();
            }
        }
    }
}
