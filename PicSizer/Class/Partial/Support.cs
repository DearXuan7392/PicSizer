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

    /// <summary>
    /// 命名方式
    /// </summary>
    public enum RenameMode
    {
        Number = 0,
        Original = 1,
        Custom = 2
    }

    /// <summary>
    /// 后缀方式 
    /// </summary>
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
                default: return null;
            }
        }
    }

}
