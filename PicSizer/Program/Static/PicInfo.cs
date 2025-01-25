#region

using System.Drawing;

#endregion

namespace PicSizer.Program.Static
{
    public static class PicInfo
    {
        /// <summary>
        /// 窗体图标
        /// </summary>
        public static readonly Icon AppIcon = Icon.FromHandle(Resource.PicSizer_png.GetHicon());

        /// <summary>
        /// 项目名称
        /// </summary>
        public const string ProjectName = "PicSizer";

        /// <summary>
        /// 项目版本
        /// </summary>
        public const string ProjectVersion = "v5.0.0";
    }
}