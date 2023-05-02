using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Static
{
    public static partial class PicSetting
    {
        /// <summary>
        /// 最大线程数
        /// </summary>
        public static int MaxThreads = 2;

        /// <summary>
        /// 窗体置顶
        /// </summary>
        public static bool TopMost = true;

        /// <summary>
        /// 是否启用GPU加速
        /// </summary>
        public static bool UseGPU = true;
    }
}
