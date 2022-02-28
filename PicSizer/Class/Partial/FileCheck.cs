using PicSizer.Class.Unit;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using PicSizer.Class.Static;

namespace PicSizer.Class.Partial
{
    public static class FileCheck
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
        /// 获取要把指定图片导出的格式
        /// </summary>
        public static ImageFormat GetFileExportFormat(string path)
        {
            //选择了原格式
            if(Value.setting.extensionMode == ExtensionMode.Original)
            {
                return GetImageFormat(path);
            }
            //选择了某一个指定的格式
            else
            {
                return Value.setting.extensionMode.ToImageFormat();
            }
        }

        /// <summary>
        /// 从路径里获取图片编码方式
        /// </summary>
        public static ImageFormat GetImageFormat(string path)
        {
            string extension = GetExtension(path);
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".png":
                    return ImageFormat.Png;
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".tif":
                case ".tiff":
                    return ImageFormat.Tiff;
                case ".ico":
                    return ImageFormat.Icon;
                default:
                    throw new Exception("不支持导出的编码: \"" + extension + "\"");
            }
        }

        /// <summary>
        /// 从给定的源文件，生成路径，序号获取文件名，并保存扩展名方式
        /// </summary>
        public static string GetResultFileName(string ori, string dir, int num)
        {
            //如果选择“覆盖源文件”，则直接返回源文件路径
            if (Value.CoverOriginalFile)
            {
                return ori;
            }
            //求出后缀名
            string extension;
            if (Value.setting.extensionMode == ExtensionMode.Original)
            {
                //原格式
                extension = GetExtension(ori);
            }
            else
            {
                //自定义格式
                extension = Value.setting.extensionMode.ToFormat();
            }
            switch (Value.setting.renameMode)
            {
                case RenameMode.Number://纯数字
                    return Path.Combine(dir, num + extension);
                case RenameMode.Original://原名
                    return Path.Combine(dir, Path.GetFileNameWithoutExtension(ori) + extension);
                case RenameMode.Custom://混合命名
                    string oriStr = Path.GetFileNameWithoutExtension(ori);//文件原名
                    string numStr = num.ToString();//序号
                    return Path.Combine(dir, Value.setting.CustomRenameStr.Replace("{ori}", oriStr).Replace("{num}", numStr) + extension);
                default:
                    return null;
            }
        }
    }
}
