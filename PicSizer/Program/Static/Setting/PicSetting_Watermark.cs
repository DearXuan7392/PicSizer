using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PicSizer.Static.PicUnit;

namespace PicSizer.Static
{
    public static partial class PicSetting
    {
        /// <summary>
        /// 水印类型
        /// </summary>
        public static WatermarkType WatermarkType = WatermarkType.Non;

        /// <summary>
        /// 水印透明度,范围: 0~100,0表示完全没有,100表示明显水印
        /// </summary>
        public static byte WatermarkAlpha = 100;

        /// <summary>
        /// 水印字体
        /// </summary>
        public static Font WatermarkFont = SystemFonts.DefaultFont;

        /// <summary>
        /// 水印颜色
        /// </summary>
        public static byte[] WatermarkColor = { 0, 0, 0 };

        /// <summary>
        /// 水印字符
        /// </summary>
        public static string WatermarkText = null;
    }
}
