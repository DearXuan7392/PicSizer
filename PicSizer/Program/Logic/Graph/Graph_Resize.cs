using PicSizer.Static;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PicSizer.Static.PicUnit;

namespace PicSizer.Logic
{
    public static partial class Graph
    {
        /// <summary>
        /// 调整图片像素,返回一个新的Bitmap,且销毁老Bitmap
        /// </summary>
        public static void ResizeBitmap(ref Bitmap bitmap)
        {
            if(PicSetting.ResizeType == ResizeType.Non)
            {
                return;
            }
            //获取图片尺寸
            int width = bitmap.Width;
            int height = bitmap.Height;
            //求出比值
            float widthByMin = (float)width / PicSetting.LimitWidth;
            float heightByMin = (float)height / PicSetting.LimitHeight;
            //temp是临时变量，用于计算缩放比例
            float temp;
            //重新设定边长
            switch (PicSetting.ResizeType)
            {
                case ResizeType.NotSmallerThanLimit://不小于限定值
                    temp = Math.Min(widthByMin, heightByMin);
                    if (temp > 1)
                    {
                        width = (int)(width / temp);
                        height = (int)(height / temp);
                    }
                    ScaleBitmap(ref bitmap, width, height);
                    break;
                case ResizeType.NotBiggerThanLimit://不大于限定值
                    temp = Math.Max(widthByMin, heightByMin);
                    if (temp > 1)
                    {
                        width = (int)(width / temp);
                        height = (int)(height / temp);
                    }
                    ScaleBitmap(ref bitmap, width, height);
                    break;
                case ResizeType.ForceCut://裁剪
                    temp = Math.Min(widthByMin, heightByMin);
                    //缩放图片，使得width和height有一个恰好满足要求，另一个大于等于要求，则下一步仅需要裁剪
                    CenterCutBitmap(ref bitmap, temp);
                    break;
                case ResizeType.ForceScale://不考虑长宽比强制缩放
                    width = PicSetting.LimitWidth;
                    height = PicSetting.LimitHeight;
                    ScaleBitmap(ref bitmap, width, height);
                    break;
            }
        }

        /// <summary>
        /// 居中裁剪图片
        /// </summary>
        private static void CenterCutBitmap(ref Bitmap bitmap, float scale)
        {
            //width和height是bitmap需要裁剪的区域
            int limitWidth = (int)(PicSetting.LimitWidth * scale);
            int limitHeight = (int)(PicSetting.LimitHeight * scale);
            //bitmap的裁剪区域左上角位置
            int left = (bitmap.Width - limitWidth) / 2;
            int top = (bitmap.Height - limitHeight) / 2;
            Bitmap newBitmap = new Bitmap(PicSetting.LimitWidth, PicSetting.LimitHeight, bitmap.PixelFormat);
            Graphics g = Graphics.FromImage(newBitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(bitmap,
                new Rectangle(0, 0, PicSetting.LimitWidth, PicSetting.LimitHeight),
                new Rectangle(left, top, limitWidth, limitHeight),
                GraphicsUnit.Pixel);
            g.Dispose();
            bitmap.Dispose();
            bitmap = newBitmap;
        }

        /// <summary>
        /// 缩放图片
        /// </summary>
        public static void ScaleBitmap(ref Bitmap bitmap, int width, int height)
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
            bitmap.Dispose();
            bitmap = newBitmap;
        }
    }
}
