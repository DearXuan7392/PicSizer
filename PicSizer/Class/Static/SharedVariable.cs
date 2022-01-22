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

        /// <summary>
        /// 覆盖源文件
        /// </summary>
        public static bool CoverOriginalFile = false;

        //窗体
        public static Form1 mainForm;
        public static SettingForm settingForm = new SettingForm();
        public static ProgressForm progressForm = new ProgressForm();
        public static DearXuan dearXuan = new DearXuan();
        public static About about = new About();

        /// <summary>
        /// 程序设置
        /// </summary>
        public static Partial.Setting setting = new Partial.Setting();

        /// <summary>
        /// 当前选中的图片后缀
        /// </summary>
        public static System.Drawing.Imaging.ImageCodecInfo SelectCodeInfo;
    }
}
