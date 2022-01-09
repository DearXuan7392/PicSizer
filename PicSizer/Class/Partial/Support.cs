using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSizer
{
    /// <summary>
    /// 尺寸修正模式
    /// </summary>
    public enum ResizeMode
    {
        None = 0,
        MinSize = 1,
        MaxSize = 2,
        Custom
    }

    /// <summary>
    /// 压缩模式
    /// </summary>
    public enum CompressionMode
    {
        SizeFirst = 0,
        QualityFirst = 1
    }

    //命名方式
    public enum RenameMode
    {
        Number = 0,
        Original = 1,
        Custom = 2
    }

    //后缀方式
    public enum ExtensionMode
    {
        JPEG = 0,
        PNG = 1,
        BMP = 2,
        TIFF = 3,
        Original = 4
    }

    /// <summary>
    /// 遇到异常时的操作
    /// </summary>
    public enum DoWhenException
    {
        /// <summary>
        /// 忽略错误并继续编号
        /// </summary>
        IgnoreAndContinue = 0,
        /// <summary>
        /// 忽略错误并跳过当前编号
        /// </summary>
        IgnoreAndJump = 1,
        /// <summary>
        /// 弹出错误但继续编号
        /// </summary>
        ShowAndContinue = 2,
        /// <summary>
        /// 弹出错误并跳过当前编号
        /// </summary>
        ShowAndJump = 3,
        /// <summary>
        /// 弹出错误并立即退出
        /// </summary>
        ShowAndExit = 4
    }

    /// <summary>
    /// 待压缩的文件
    /// </summary>
    public class PicFile
    {
        public PicFile(string fullPath)
        {

        }

        /// <summary>
        /// 完整路径
        /// </summary>
        public string fullPath { get; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string fileName { get; }

        /// <summary>
        /// 是否已经完成压缩
        /// </summary>
        public bool hasResize { get; set; } = false;
    }

    public static class ImageInfo
    {
        public static ImageCodecInfo Info_JPEG = GetEncoderInfo("image/jpeg");
        public static ImageCodecInfo Info_PNG = GetEncoderInfo("image/png");
        public static ImageCodecInfo Info_BMP = GetEncoderInfo("image/bmp");
        public static ImageCodecInfo Info_TIFF = GetEncoderInfo("image/tiff");

        /// <summary>
        /// 获取编码信息
        /// </summary>
        private static ImageCodecInfo GetEncoderInfo(string type)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == type)
                {
                    return encoders[j];
                }
            }
            return null;
        }
    }

    /// <summary>
    /// 支持
    /// </summary>
    public static class Support
    {
        //Enum转int
        public static int ToInt(this Enum e)
        {
            return e.GetHashCode();
        }

        //FormatMode转后缀名
        public static string ToFormat(this ExtensionMode formatMode)
        {
            switch (formatMode)
            {
                case ExtensionMode.JPEG: return ".jpg";
                case ExtensionMode.PNG: return ".png";
                case ExtensionMode.BMP: return ".bmp";
                case ExtensionMode.TIFF: return ".tiff";
                default:
                    throw new Exception("内部错误: 文件格式转换时出错");
            }
        }

        //从path获取格式
        public static ExtensionMode GetFormat(this string path)
        {
            string format = Path.GetExtension(path).ToLower();
            switch (format)
            {
                case ".jpg":
                case ".jpeg":
                    return ExtensionMode.JPEG;
                case ".png":
                    return ExtensionMode.PNG;
                case ".bmp":
                    return ExtensionMode.BMP;
                case ".tiff":
                    return ExtensionMode.TIFF;
                default:
                    throw new Exception("不支持的文件格式: \"" + format + "\"");
            }
        }
    }

}
