using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer
{
    public static class SharedVariable
    {
        /// <summary>
        /// 线程信号灯
        /// </summary>
        public static bool ThreadExitNow = false;

        //窗体
        public static Form1 mainForm;
        public static SettingForm settingForm;
        public static ProgressForm progressForm;
        public static DearXuan dearXuan;

        /// <summary>
        /// 程序设置
        /// </summary>
        public static Partial.Setting setting = new Partial.Setting();
    }
}
