using PicSizer.Unit;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Partial
{
    public static class FileCheck
    {
        /// <summary>
        /// 用于储存允许的后缀名的哈希集合
        /// </summary>
        private static HashSet<string> ExtensionHashSet = new HashSet<string>();

        /// <summary>
        /// 判断文件后缀是否合法
        /// </summary>
        public static bool IsExtensionCorrect(string path)
        {
            //如果允许任意后缀，则直接返回合法
            if (Value.setting.AllowAnyExtension) return true;
            //获取后缀名
            string extension = GetExtension(path);
            //如果支持的格式里包括文件格式，则返回true
            return Extension.BitmapSupportExtension.Contains(extension);
        }

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
        /// 从文件名获取图片格式
        /// </summary>
        public static ImageCodecInfo GetImageInfoByFilename(string path)
        {
            //返回原格式，则判断原始图片的格式
            if(Value.SelectCodeInfo == null)
            {
                string extension = GetExtension(path);
                //找到对应的编码信息
                if (Extension.BitmapExportExtension.ContainsKey(extension))
                {
                    return Extension.BitmapExportExtension[extension];
                }
                //没有找到编码信息
                else
                {
                    throw new Exception("不支持导出的编码: \"" + extension + "\"");
                }
            }
            //直接返回选定的格式
            else
            {
                return Value.SelectCodeInfo;
            }
        }

        /// <summary>
        /// 设置选中的图片格式
        /// </summary>
        public static void SetImageCodeInfo(ExtensionMode mode)
        {
            switch (mode)
            {
                case ExtensionMode.JPEG:
                    Value.SelectCodeInfo = Extension.BitmapExportExtension[".jpg"];
                    break;
                case ExtensionMode.PNG:
                    Value.SelectCodeInfo = Extension.BitmapExportExtension[".png"];
                    break;
                case ExtensionMode.BMP:
                    Value.SelectCodeInfo = Extension.BitmapExportExtension[".bmp"];
                    break;
                case ExtensionMode.TIFF:
                    Value.SelectCodeInfo = Extension.BitmapExportExtension[".tiff"];
                    break;
                case ExtensionMode.Original:
                    Value.SelectCodeInfo = null;
                    break;
            }
        }

        /// <summary>
        /// 从给定的源文件，生成路径，序号获取文件名
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
