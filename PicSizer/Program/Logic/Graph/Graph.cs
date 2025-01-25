#region

using System.Drawing;
using System.Drawing.Imaging;

#endregion

namespace PicSizer.Program.Logic.Graph
{
    public static partial class Graph
    {
        public static void InitGraph(ref Bitmap img)
        {
            ResizeBitmap(ref img);
            //水印
            AddWatermark(ref img);
            //亮度
            SetBrightness(ref img);
        }

        /// <summary>
        /// 把颜色转成byte数组
        /// </summary>
        public static byte[] ColorToBytes(Color color)
        {
            return new[]
            {
                color.R,
                color.G,
                color.B
            };
        }

        /// <summary>
        /// 把byte数组转成颜色
        /// </summary>
        public static Color BytesToColor(byte[] bytes, byte alpha = 255)
        {
            return Color.FromArgb(
                alpha,
                bytes[0],
                bytes[1],
                bytes[2]);
        }

        public static int GetPixelBits(ref Bitmap bitmap)
        {
            return bitmap.PixelFormat == PixelFormat.Format24bppRgb
                ? 3
                : 4;
        }
    }
}