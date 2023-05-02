using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Logic
{
    public partial class ICON : CompressTaskItem, Compress_Based, ITaskBased
    {
        public ICON(Bitmap bitmap) : base(bitmap) { }

        /// <summary>
        /// Icon的文件标头
        /// </summary>
        private static readonly byte[] _ICON_HEADER = { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 32, 0, 0, 0, 0, 0, 22, 0, 0, 0 };

        /// <summary>
        /// 将Bitmap保存到Icon文件
        /// </summary>
        public static void SaveIconToFile(ref Bitmap bitmap, string outputFilename)
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
                // 偏移 0x6 处为图片宽度
                writer.Seek(0x6, SeekOrigin.Begin);
                writer.Write(width);
                // 偏移 0x7 处为图片高度
                writer.Seek(0x7, SeekOrigin.Begin);
                writer.Write(height);
                // 偏移 0xE 处为图片主体部分长度，应减去标头
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
