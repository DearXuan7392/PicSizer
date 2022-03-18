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

        /// <summary>
        /// 输出目录
        /// </summary>
        public static string OutputDir;

        //窗体
        public static Forms.Form1 mainForm;
        public static Forms.SettingForm settingForm = new Forms.SettingForm();
        public static Forms.ProgressForm progressForm = new Forms.ProgressForm();

        /// <summary>
        /// 程序设置
        /// </summary>
        public static Partial.Setting setting = new Partial.Setting();
    }
}
