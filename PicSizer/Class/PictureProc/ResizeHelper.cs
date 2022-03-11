using System;
using System.Drawing;
using System.Drawing.Imaging;
using PicSizer.Class.Partial;
using PicSizer.Class.Static;

namespace PicSizer.Class.PictureProc
{
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
        public static Bitmap ResizeBitmap(Bitmap bitmap, ExtensionMode extensionMode)
        {
            //如果是ICON,则直接强制修正
            if (extensionMode == ExtensionMode.ICON)
            {
                return ScaleBitmap(bitmap, Value.setting.IconLimitWidth, Value.setting.IconLimitHeight);
            }
            //获取图片尺寸
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
                case ResizeMode.Cut://裁剪
                    temp = Math.Min(widthByMin, heightByMin);
                    //缩放图片，使得width和height有一个恰好满足要求，另一个大于等于要求，则下一步仅需要裁剪
                    return CenterCutBitmap(bitmap, temp);
                case ResizeMode.Custom://强制修正
                    width = Value.setting.LimitWidth;
                    height = Value.setting.LimitHeight;
                    return ScaleBitmap(bitmap, width, height);
                default://无修正,不可能运行到这里
                    throw new Exception("修正模式异常");
            }
        }

        /// <summary>
        /// 居中裁剪图片
        /// </summary>
        private static Bitmap CenterCutBitmap(Bitmap bitmap, float scale)
        {
            //width和height是bitmap需要裁剪的区域
            int limitWidth = (int)(Value.setting.LimitWidth * scale);
            int limitHeight = (int)(Value.setting.LimitHeight * scale);
            //bitmap的裁剪区域左上角位置
            int left = (bitmap.Width - limitWidth) / 2;
            int top = (bitmap.Height - limitHeight) / 2;
            Bitmap newBitmap = new Bitmap(Value.setting.LimitWidth, Value.setting.LimitHeight, bitmap.PixelFormat);
            Graphics g = Graphics.FromImage(newBitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(bitmap,
                new Rectangle(0, 0, Value.setting.LimitWidth, Value.setting.LimitHeight),
                new Rectangle(left, top, limitWidth, limitHeight),
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
            return newBitmap;
        }
    }
}