#region

using System.Drawing;
using PicSizer.Program.Static;
using static PicSizer.Program.Static.PicUnit;

#endregion

namespace PicSizer.Program.Logic.Graph
{
    public static partial class Graph
    {
        /// <summary>
        /// 添加水印,销毁原Bitmap
        /// </summary>
        public static void AddWatermark(ref Bitmap bitmap)
        {
            if (PicSetting.WatermarkType == WatermarkType.Non) return;
            Graphics g = null;
            //水印参数
            Color color = BytesToColor(PicSetting.WatermarkColor,
                (byte)(PicSetting.WatermarkAlpha * 255 / 100)); // 水印颜色
            //图片尺寸
            int width = bitmap.Width;
            int height = bitmap.Height;
            try
            {
                g = Graphics.FromImage(bitmap);
                //字符串画在Bitmap上的尺寸
                SizeF sizeF = g.MeasureString(PicSetting.WatermarkText, PicSetting.WatermarkFont);
                //水印范围
                float textWidth = sizeF.Width;
                float textHeight = sizeF.Height;
                float rectX;
                float rectY;
                //选取位置
                switch (PicSetting.WatermarkType)
                {
                    case WatermarkType.Center:
                        rectX = (width - textWidth) / 2;
                        rectY = (height - textHeight) / 2;
                        break;
                    case WatermarkType.LeftTop:
                        rectX = 0;
                        rectY = 0;
                        break;
                    case WatermarkType.LeftBottom:
                        rectX = 0;
                        rectY = height - textWidth;
                        break;
                    case WatermarkType.RightTop:
                        rectX = width - textWidth;
                        rectY = 0;
                        break;
                    case WatermarkType.RightBottom:
                        rectX = width - textWidth;
                        rectY = height - textHeight;
                        break;
                    default:
                        return;
                }

                RectangleF textArea = new RectangleF(rectX, rectY, textWidth, textHeight);
                SolidBrush brush = new SolidBrush(color);
                g.DrawString(
                    PicSetting.WatermarkText,
                    PicSetting.WatermarkFont,
                    brush,
                    textArea);
            }
            finally
            {
                g?.Dispose();
            }
        }
    }
}