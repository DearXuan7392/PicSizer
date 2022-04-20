using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using PicSizer.Class.Partial;
using PicSizer.Class.Static;

namespace PicSizer.Class.PictureProc
{
    public class AtomPic
    {
        public string originalFilename;
        
        /// <summary>
        /// 图片
        /// </summary>
        public Bitmap bitmap;

        /// <summary>
        /// 后缀格式
        /// </summary>
        public ExtensionMode Extension;

        /// <summary>
        /// 导出的图片格式
        /// </summary>
        public ImageFormat ExportImageFormat;

        /// <summary>
        /// 输出路径
        /// </summary>
        public string OutputFilename
        {
            get => FileProc.GetResultFileName(this, ThreadsPool.GetPicNum());
        }

        /// <summary>
        /// 创建一个新的图片压缩任务
        /// </summary>
        public AtomPic(string file)
        {
            originalFilename = file;
            Bitmap original = null;
            try
            {
                //判断要生成的文件后缀
                Extension = FileProc.GetExtensionMode(originalFilename);
                ExportImageFormat = Extension == ExtensionMode.ICON
                    ? ImageFormat.Png
                    : Extension.ToImageFormat();
                //尝试加载原始图片
                //如果修正模式不是"无修正",或者导出格式是ICON,则修正图片
                if (Value.setting.resizeMode != ResizeMode.None || Extension == ExtensionMode.ICON)
                {
                    original = new Bitmap(file);
                    bitmap = ResizeHelper.ResizeBitmap(original, Extension);
                    original.Dispose();
                }
                else
                {
                    bitmap = new Bitmap(file);
                }
                //ICON图片直接转化为32位ARGB,不需要去除透明像素
                if (Extension == ExtensionMode.ICON)
                {
                    if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                    {
                        Bitmap newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                            PixelFormat.Format32bppArgb);
                        bitmap.Dispose();
                        bitmap = newBitmap;
                    }
                }
                else
                {
                    //如果含有透明像素,则去除
                    if (ResizeHelper.CheckTransparentPixel(bitmap))
                    {
                        Bitmap newBitmap = null;
                        try
                        {
                            //如果图片已经是32位ARGB,则直接修改像素
                            if (bitmap.PixelFormat == PixelFormat.Format32bppArgb)
                            {
                                newBitmap = bitmap;
                                Graphy.SetAlphaPixelColor(newBitmap);
                            }
                            //如果不是,则转换成32位ARGB,再修改像素
                            else
                            {
                                newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppArgb);
                                bitmap.Dispose();
                                Graphy.SetAlphaPixelColor(newBitmap);
                            }
                            bitmap = newBitmap.Clone(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), PixelFormat.Format24bppRgb);
                        }
                        finally
                        {
                            newBitmap?.Dispose();
                        }
                    }
                }
                
                //设置亮度
                Graphy.SetBrightness(bitmap);
                //添加水印
                Graphy.AddWatermark(bitmap);
            }
            finally
            {
                original?.Dispose();
            }
        }

        /// <summary>
        /// 按照指定的长宽输出图片
        /// </summary>
        public void SaveToFileBySize(int width, int height)
        {
            Bitmap final = null;
            try
            {
                final = ResizeHelper.ScaleBitmap(bitmap, width, height);
                if (Extension == ExtensionMode.ICON)
                {
                    BitmapStream.SaveBitmapToFile_Icon(final, OutputFilename);
                }
                else
                {
                    final.Save(OutputFilename, ExportImageFormat);
                }
            }
            finally
            {
                final?.Dispose();
            }
        }

        /// <summary>
        /// 按照指定的画质输出图片(仅限JPEG)
        /// </summary>
        public void SaveToFileByQuality(long value)
        {
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = Encoder.GetParameter(value);
            bitmap.Save(OutputFilename, Encoder._Info_JPEG, encoderParameters);

        }

        /// <summary>
        /// 按照指定的分辨率输出图片
        /// </summary>
        public void SaveToFileByResolution(float horizontalResolution, float verticalResolution)
        {
            bitmap.SetResolution(horizontalResolution, verticalResolution);
            bitmap.Save(OutputFilename, ExportImageFormat);
        }

        /// <summary>
        /// 根据指定的位深度输出图片
        /// </summary>
        public void SaveToFileByBitDeep(Rectangle rect, PixelFormat format)
        {
            Bitmap final = null;
            try
            {
                final = bitmap.Clone(rect, format);
                final.Save(OutputFilename, ExportImageFormat);
            }
            finally
            {
                final?.Dispose();
            }
        }

        public void Dispose()
        {
            bitmap?.Dispose();
        }
    }
}
