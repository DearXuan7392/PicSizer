using PicSizer.Class.Unit;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using PicSizer.Class.PictureProc;
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

        public static ExtensionMode GetExtensionMode(string path)
        {
            if (Value.setting.extensionMode == ExtensionMode.Original)
            {
                string extension = GetExtension(path);
                switch (extension)
                {
                    case ".jpg":
                    case ".jpeg":
                        return ExtensionMode.JPEG;
                    case ".png":
                        return ExtensionMode.PNG;
                    case ".bmp":
                        return ExtensionMode.BMP;
                    case ".tif":
                    case ".tiff":
                        return ExtensionMode.TIFF;
                    case ".ico":
                    case ".icon":
                        return ExtensionMode.ICON;
                    default:
                        throw new Exception("不支持导出的编码: \"" + extension + "\"");
                }
            }
            else
            {
                return Value.setting.extensionMode;
            }
        }

        /// <summary>
        /// 从给定的源文件，生成路径，序号获取文件名，并保存扩展名方式
        /// </summary>
        public static string GetResultFileName(AtomPic atomPic, int num)
        {
            //如果选择“覆盖源文件”，则直接返回源文件路径
            if (Value.CoverOriginalFile)
            {
                return atomPic.originalFilename;
            }
            //求出后缀名
            string extension = atomPic.Extension.ToFormat();
            switch (Value.setting.renameMode)
            {
                case RenameMode.Number://纯数字
                    return Path.Combine(Value.OutputDir, num + extension);
                case RenameMode.Original://原名
                    return Path.Combine(Value.OutputDir, Path.GetFileNameWithoutExtension(atomPic.originalFilename) + extension);
                case RenameMode.Custom://混合命名
                    string oriStr = Path.GetFileNameWithoutExtension(atomPic.originalFilename);//文件原名
                    string numStr = num.ToString();//序号
                    return Path.Combine(Value.OutputDir, Value.setting.CustomRenameStr.Replace("{ori}", oriStr).Replace("{num}", numStr) + extension);
                default:
                    return null;
            }
        }
    }
}
