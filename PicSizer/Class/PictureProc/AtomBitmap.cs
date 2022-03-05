using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PicSizer.Class.Partial;
using PicSizer.Class.Static;

namespace PicSizer.Class.PictureProc
{
    public class AtomBitmap
    {
        /// <summary>
        /// 图片
        /// </summary>
        public Bitmap bitmap = null;

        /// <summary>
        /// 导出的格式
        /// </summary>
        public ImageFormat ExportImageFormat;

        /// <summary>
        /// 输出路径
        /// </summary>
        public string OutputFilename;

        public AtomBitmap(string file)
        {
            Bitmap original = null;
            try
            {
                //尝试加载图片
                original = new Bitmap(file);
                //缩放图片,此时保持原像素位数
                this.bitmap = ResizeHelper.ResizeBitmap(original);
                //如果含有透明像素,则去除
                if (ResizeHelper.CheckTransparentPixel(bitmap))
                {
                    Bitmap newBitmap = null;
                    try
                    {
                        //克隆图片
                        newBitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppArgb);
                        Graphy.SetAlphaPixelColor(newBitmap);
                        bitmap.Dispose();
                        bitmap = newBitmap.Clone(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), PixelFormat.Format24bppRgb);
                    }
                    finally
                    {
                        newBitmap?.Dispose();
                    }
                    
                }
                //设置亮度
                Graphy.SetBrightness(bitmap);
                //判断要生成的文件后缀
                if (Value.setting.extensionMode == ExtensionMode.Original)
                {
                    this.ExportImageFormat = FileCheck.GetImageFormat(file);
                }
                else
                {
                    this.ExportImageFormat = Value.setting.extensionMode.ToImageFormat();
                }
            }
            catch (Exception)
            {
                bitmap?.Dispose();
                throw new Exception("图片加载失败");
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
                BitmapSave.SaveBitmapToFile(final, OutputFilename, ExportImageFormat);
            }
            finally
            {
                final?.Dispose();
            }
        }

        /// <summary>
        /// 按照指定的分辨率输出图片
        /// </summary>
        public void SaveToFileByResolution(float horizontalResolution, float verticalResolution)
        {
            bitmap.SetResolution(horizontalResolution, verticalResolution);
            BitmapSave.SaveBitmapToFile(bitmap, OutputFilename, ExportImageFormat);
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
                BitmapSave.SaveBitmapToFile(final, OutputFilename, ExportImageFormat);
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

    public static class ResizeHelper
    {
        /// <summary>
        /// 检查Bitmap是否含有透明通道
        /// </summary>
        public static bool CheckTransparentPixel(Bitmap bitmap)
        {
            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 调整图片像素
        /// </summary>
        public static Bitmap ResizeBitmap(Bitmap bitmap)
        {
            //关闭了尺寸修正
            if (Value.setting.resizeMode == ResizeMode.None)
            {
                return new Bitmap(bitmap);
            }
            int width = bitmap.Width;
            int height = bitmap.Height;
            //求出比值
            float widthByMin = (float)width / Value.setting.LimitWidth;
            float heightByMin = (float)height / Value.setting.LimitHeight;
            //temp是临时变量，用于计算缩放比例
            float temp;
            //重新设定边长
            switch (Value.setting.resizeMode)
            {
                case ResizeMode.MinSize://不小于限定值
                    temp = Math.Min(widthByMin, heightByMin);
                    if (temp > 1)
                    {
                        width = (int)(width / temp);
                        height = (int)(height / temp);
                    }
                    return ScaleBitmap(bitmap, width, height);
                case ResizeMode.MaxSize://不大于限定值
                    temp = Math.Max(widthByMin, heightByMin);
                    if (temp > 1)
                    {
                        width = (int)(width / temp);
                        height = (int)(height / temp);
                    }
                    return ScaleBitmap(bitmap, width, height);
                case ResizeMode.Custom://强制修正
                    width = Value.setting.LimitWidth;
                    height = Value.setting.LimitHeight;
                    return ScaleBitmap(bitmap, width, height);
                case ResizeMode.Cut://裁剪
                    temp = Math.Min(widthByMin, heightByMin);
                    //缩放图片，使得width和height有一个恰好满足要求，另一个大于等于要求，则下一步仅需要裁剪
                    return CenterCutBitmap(bitmap, temp);
                default://无修正,不可能运行到这里
                    return null;
            }
        }

        /// <summary>
        /// 居中裁剪图片
        /// </summary>
        private static Bitmap CenterCutBitmap(Bitmap bitmap, float scale)
        {
            //width和height是bitmap需要裁剪的区域
            int final_width = (int)(Value.setting.LimitWidth * scale);
            int final_height = (int)(Value.setting.LimitHeight * scale);
            //bitmap的裁剪区域左上角位置
            int left = (bitmap.Width - final_width) / 2;
            int top = (bitmap.Height - final_height) / 2;
            Bitmap newBitmap = new Bitmap(Value.setting.LimitWidth, Value.setting.LimitHeight, bitmap.PixelFormat);
            Graphics g = Graphics.FromImage(newBitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(bitmap,
                new Rectangle(0, 0, Value.setting.LimitWidth, Value.setting.LimitHeight),
                new Rectangle(left, top, final_width, final_height),
                GraphicsUnit.Pixel);
            g.Dispose();
            return newBitmap;
        }

        /// <summary>
        /// 缩放图片
        /// </summary>
        public static Bitmap ScaleBitmap(Bitmap bitmap, int width, int height)
        {
            //缩放图片
            Bitmap newBitmap = new Bitmap(width, height, bitmap.PixelFormat);
            Graphics g = Graphics.FromImage(newBitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(bitmap,
                new Rectangle(0, 0, width, height), //画在新Bitmap上的区域
                new Rectangle(0, 0, bitmap.Width, bitmap.Height), //老Bitmap截取的区域
                GraphicsUnit.Pixel);
            g.Dispose();//摧毁
            bitmap.Dispose();//摧毁
            return newBitmap;
        }
    }
}
