using System.Drawing;

namespace PicSizer.Static
{
    public static class PicInfo
    {
        /// <summary>
        /// 窗体图标
        /// </summary>
        public static Icon icon = Icon.FromHandle(Resource.PicSizer_png.GetHicon());

        /// <summary>
        /// 项目名称
        /// </summary>
        public static string ProjectName = "PicSizer";

        /// <summary>
        /// 项目版本
        /// </summary>
        public static string ProjectVersion = "v5.0.0";
    }
}
