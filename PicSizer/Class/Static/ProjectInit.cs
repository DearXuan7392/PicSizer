using PicSizer.Partial;
using PicSizer.Unit;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSizer.Static
{
    public static class ProjectInit
    {
        /// <summary>
        /// 程序运行时的初始化任务
        /// </summary>
        public static void Init()
        {
            //载入dll
            DllExtern.DoInFirst();

            //添加扩展名
            string[] extensionList = new string[] {
                ".jpg",".jpeg",".png",".bmp",".tif",".tiff",".pcx",".ico",".dxf",".cgm",".cdr",".wmf",".eps",".emf"
            };
            foreach(string extension in extensionList)
            {
                Extension.BitmapSupportExtension.Add(extension);
            }

            //添加图像解码信息
            ImageCodecInfo _Info_JPEG = Encoder.GetEncoderInfo("image/jpeg");
            ImageCodecInfo _Info_PNG = Encoder.GetEncoderInfo("image/png");
            ImageCodecInfo _Info_BMP = Encoder.GetEncoderInfo("image/bmp");
            ImageCodecInfo _Info_TIFF = Encoder.GetEncoderInfo("image/tiff");
            Extension.BitmapExportExtension.Add(".jpg", _Info_JPEG);
            Extension.BitmapExportExtension.Add(".jpeg", _Info_JPEG);
            Extension.BitmapExportExtension.Add(".png", _Info_PNG);
            Extension.BitmapExportExtension.Add(".bmp", _Info_BMP);
            Extension.BitmapExportExtension.Add(".tif", _Info_TIFF);
            Extension.BitmapExportExtension.Add(".tiff", _Info_TIFF);
        }
    }
}
