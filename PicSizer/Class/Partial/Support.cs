using System;

namespace PicSizer.Class.Partial
{
    /// <summary>
    /// 尺寸修正模式
    /// </summary>
    public enum ResizeMode
    {
        /// <summary>
        /// 无修正
        /// </summary>
        None = 0,
        /// <summary>
        /// 不小于限定值
        /// </summary>
        MinSize = 1,
        /// <summary>
        /// 不大于限定值
        /// </summary>
        MaxSize = 2,
        /// <summary>
        /// 强制修正
        /// </summary>
        Custom = 3,
        /// <summary>
        /// 居中裁剪
        /// </summary>
        Cut = 4
    }

    /// <summary>
    /// 压缩模式
    /// </summary>
    public enum CompressionMode_JPEG
    {
        SizeFirst = 0,
        QualityFirst = 1
    }

    /// <summary>
    /// 对ICON图片的压缩方式
    /// </summary>
    public enum CompressionMode_ICON
    {
        /// <summary>
        /// 基于缩放的压缩
        /// </summary>
        ScaleBased = 0
    }
    
    /// <summary>
    /// 对其它图片的压缩方式
    /// </summary>
    public enum CompressionMode_Other
    {
        /// <summary>
        /// 基于缩放的压缩
        /// </summary>
        ScaleBased = 0,
        /// <summary>
        /// 基于位深度的压缩
        /// </summary>
        BitDepthBased = 1
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
        ICON = 4,
        Original = 5
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
    /// 水印模式
    /// </summary>
    public enum WatermarkMode
    {
        /// <summary>
        /// 无水印
        /// </summary>
        Non = 0,
        /// <summary>
        /// 居中
        /// </summary>
        Center = 1,
        /// <summary>
        /// 左上
        /// </summary>
        LeftTop = 2,
        /// <summary>
        /// 左下
        /// </summary>
        LeftBottom = 3,
        /// <summary>
        /// 右上
        /// </summary>
        RightTop = 4,
        /// <summary>
        /// 右下
        /// </summary>
        RightBottom = 5
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
        public static string ToFormat(this ExtensionMode extensionMode)
        {
            switch (extensionMode)
            {
                case ExtensionMode.JPEG: return ".jpg";
                case ExtensionMode.PNG: return ".png";
                case ExtensionMode.BMP: return ".bmp";
                case ExtensionMode.TIFF: return ".tiff";
                case ExtensionMode.ICON: return ".ico";
                default: return null;
            }
        }

        public static System.Drawing.Imaging.ImageFormat ToImageFormat(this ExtensionMode extensionMode)
        {
            switch (extensionMode)
            {
                case ExtensionMode.JPEG: return System.Drawing.Imaging.ImageFormat.Jpeg;
                case ExtensionMode.PNG: return System.Drawing.Imaging.ImageFormat.Png;
                case ExtensionMode.BMP: return System.Drawing.Imaging.ImageFormat.Bmp;
                case ExtensionMode.TIFF: return System.Drawing.Imaging.ImageFormat.Tiff;
                case ExtensionMode.ICON: return System.Drawing.Imaging.ImageFormat.Icon;
                default: throw new Exception("格式错误");
            }
        }
    }

}
