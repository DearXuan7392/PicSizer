using System.Drawing;

namespace PicSizer.Class.Static
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
        /// 版本
        /// </summary>
        public static readonly Unit.Version ProjectVersion = new Unit.Version()
        {
            mainVersion = 4,
            secondVersion = 9,
            thirdVersion = 2,
            alpha = false
        };

        /// <summary>
        /// 配置文件的版本，防止跨版本导入
        /// </summary>
        public static readonly Unit.Version SettingVersion = new Unit.Version()
        {
            mainVersion = 1,
            secondVersion = 4,
            thirdVersion = 0,
            alpha = true
        };
    }
}
