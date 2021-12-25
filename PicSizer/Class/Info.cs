using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Class
{
    public static class Info
    {
        /// <summary>
        /// 是否支持硬件加速
        /// </summary>
        public static bool isGPUSupport = false;

        public static void DoInFirst()
        {
            isGPUSupport = DllExtern.IsGPUSupport();
        }
    }
}
