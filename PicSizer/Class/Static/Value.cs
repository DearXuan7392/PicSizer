namespace PicSizer.Class.Static
{
    public static class Value
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

        /// <summary>
        /// 程序设置
        /// </summary>
        public static Partial.Setting setting = new Partial.Setting();
    }
}
