using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Partial
{
    public static class FileCheck
    {
        /// <summary>
        /// 判断文件后缀是否合法
        /// </summary>
        public static bool isExtensionCorrect(string path)
        {
            //如果允许任意后缀，则直接返回合法
            if (SharedVariable.setting.AllowAnyExtension) return true;
            //查找点号位置
            int i = path.LastIndexOf('.');
            //没有后缀名，直接返回非法
            if (i == -1) return false;
            //截取后缀名
            string extension = path.Substring(i).ToLower();
            switch (extension)
            {
                case ".jpg":
                case ".png":
                case ".bmp":
                case ".tiff":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 从文件名获取图片格式
        /// </summary>
        /// <param name="fullpath"></param>
        public static System.Drawing.Imaging.ImageCodecInfo GetImageInfoByFilename(string path)
        {
            //返回原格式，则判断原始图片的格式
            if(SharedVariable.SelectCodeInfo == null)
            {
                int i = path.LastIndexOf('.');
                if (i == -1) return Encoder.Info_Default;
                string extension = path.Substring(i).ToLower();
                switch (extension)
                {
                    case ".jpg":
                        return Encoder.Info_JPEG;
                    case ".png":
                        return Encoder.Info_PNG;
                    case ".bmp":
                        return Encoder.Info_BMP;
                    case ".tiff":
                        return Encoder.Info_TIFF;
                    default:
                        return Encoder.Info_Default;
                }
            }
            //直接返回选定的格式
            else
            {
                return SharedVariable.SelectCodeInfo;
            }
        }

        /// <summary>
        /// 设置选中的图片格式
        /// </summary>
        /// <param name="mode"></param>
        public static void SetImageCodeInfo(ExtensionMode mode)
        {
            switch (mode)
            {
                case ExtensionMode.JPEG:
                    SharedVariable.SelectCodeInfo = Encoder.Info_JPEG;
                    break;
                case ExtensionMode.PNG:
                    SharedVariable.SelectCodeInfo = Encoder.Info_PNG;
                    break;
                case ExtensionMode.BMP:
                    SharedVariable.SelectCodeInfo = Encoder.Info_BMP;
                    break;
                case ExtensionMode.TIFF:
                    SharedVariable.SelectCodeInfo = Encoder.Info_TIFF;
                    break;
                case ExtensionMode.Original:
                    SharedVariable.SelectCodeInfo = null;
                    break;
            }
        }
    }
}
