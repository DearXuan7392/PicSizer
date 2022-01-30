using PicSizer_ControlLibrary;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSizer.Partial
{
    /// <summary>
    /// 尺寸修正模式
    /// </summary>
    public enum ResizeMode
    {
        None = 0,
        MinSize = 1,
        MaxSize = 2,
        Custom = 3,
        Cut = 4
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

    [Serializable]
    public class Version
    {
        /// <summary>
        /// 主版本
        /// </summary>
        public byte mainVersion = 0;

        /// <summary>
        /// 次要版本
        /// </summary>
        public byte secondVersion = 0;

        /// <summary>
        /// 再次版本
        /// </summary>
        public byte thirdVersion = 0;

        /// <summary>
        /// 是否开发板
        /// </summary>
        public bool alpha = false;

        /// <summary>
        /// 版本比较
        /// </summary>
        public int CompareTo(Version other)
        {
            if (mainVersion != other.mainVersion) return mainVersion > other.mainVersion ? 1 : -1;
            if (secondVersion != other.secondVersion) return secondVersion > other.secondVersion ? 1 : -1;
            if (thirdVersion != other.thirdVersion) return thirdVersion > other.thirdVersion ? 1 : -1;
            if (alpha == other.alpha) return 0;
            return alpha ? -1 : 1;
        }

        override public string ToString()
        {
            string s = mainVersion + "." + secondVersion + "." + thirdVersion;
            if (alpha)
            {
                s += "-alpha";
            }
            return s;
        }
    }

    /// <summary>
    /// 支持
    /// </summary>
    public static class Support
    {
        /// <summary>
        /// Enum转int
        /// </summary>
        public static int ToInt(this Enum e)
        {
            return e.GetHashCode();
        }

        /// <summary>
        /// FormatMode转后缀名
        /// </summary>
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

        public static string ToState(this PicState state)
        {
            switch (state)
            {
                case PicState.Waiting:
                    return "待压缩";
                case PicState.Success:
                    return "已完成";
                case PicState.Error:
                    return "错误";
                default:
                    return null;
            }
        }

        /// <summary>
        /// 从path获取格式
        /// </summary>
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

        /// <summary>
        /// 文件大小(KB)转字符串
        /// </summary>
        public static string ToFileSizeStr(this long size)
        {
            if (size < 1024)
            {
                return size.ToString() + " KB";
            }
            else
            {
                return (size / 1024.0).ToString("#0.00") + " MB";
            }
        }
    }

}
