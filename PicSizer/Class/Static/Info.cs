using System.Drawing;

namespace PicSizer
{
    public static class Info
    {
        public static Icon icon = Icon.FromHandle(Resource.PicSizer_png.GetHicon());

        /// <summary>
        /// 是否支持硬件加速
        /// </summary>
        public static bool isGPUSupport = false;

        /// <summary>
        /// 项目名称
        /// </summary>
        public const string ProjectName = "PicSizer";

        /// <summary>
        /// 设置文件的扩展名
        /// </summary>
        public const string SettingFileExtension = "pics";

        /// <summary>
        /// 软件描述
        /// </summary>
        public const string Description = ProjectName +
            "是一款可以指定压缩后大小的图片压缩软件，通过该软件，您可以轻易地将大量图片压缩到适合您的大小.\n" +
            "本软件是一款开源软件，具体实现算法和程序代码均全部开放，因此您不必担心软件会存在病毒或后门.";

        /// <summary>
        /// 版本
        /// </summary>
        public static readonly Unit.Version ProjectVersion = new Unit.Version()
        {
            mainVersion = 4,
            secondVersion = 6,
            thirdVersion = 1,
            alpha = true
        };

        /// <summary>
        /// 配置文件的版本，防止跨版本导入
        /// </summary>
        public static readonly Unit.Version SettingVersion = new Unit.Version()
        {
            mainVersion = 1,
            secondVersion = 1,
            thirdVersion = 0,
            alpha = false
        };
    }
}
