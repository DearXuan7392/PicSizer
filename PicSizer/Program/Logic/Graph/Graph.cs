using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Logic
{
    public static partial class Graph
    {
        /// <summary>
        /// 把颜色转成byte数组
        /// </summary>
        public static byte[] ColorToBytes(Color color)
        {
            return new byte[]
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
