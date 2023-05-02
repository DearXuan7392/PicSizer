using PicSizer.Static;
using PicSizer.Window.Partial;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static PicSizer.Static.PicUnit;

namespace PicSizer.Logic
{
    /// <summary>
    /// 图像压缩
    /// </summary>
    public static partial class Compress
    {
        /// <summary>
        /// 压缩图片
        /// </summary>
        public static void CompressPicture(Bitmap bitmap, string output)
        {
            //获取输出文件扩展名
            string extension = FileIO.FileProc.GetExtension(output);
            Compress_Based item = null;
            
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    item = new JPEG(bitmap);
                    break;
                case ".png":
                    item = new PNG(bitmap);
                    break;
                case ".webp":
                    item = new WEBP(bitmap);
                    break;
                default:
                    throw new Exception($"\"{extension}\"格式不受支持.", default);
            }

            switch(PicSetting.CompressType)
            {
                case CompressType.Strength:
                    item.Compress_By_Strength(PicSetting.Strength, output);
                    break;
                case CompressType.FileSize:
                    item.Compress_By_FileSize(PicSetting.LimitSize, output);
                    break;
            }
        }
    }
}
