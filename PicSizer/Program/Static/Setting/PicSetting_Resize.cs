using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PicSizer.Static.PicUnit;

namespace PicSizer.Static
{
    public static partial class PicSetting
    {
        /// <summary>
        /// 限制宽度
        /// </summary>
        public static int LimitWidth = 1920;

        /// <summary>
        /// 限制高度
        /// </summary>
        public static int LimitHeight = 1080;

        /// <summary>
        /// 裁剪模式
        /// </summary>
        public static ResizeType ResizeType = ResizeType.Non;
    }
}
