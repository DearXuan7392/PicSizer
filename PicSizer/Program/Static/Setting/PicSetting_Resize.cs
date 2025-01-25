#region

using static PicSizer.Program.Static.PicUnit;

#endregion

namespace PicSizer.Program.Static
{
    public static partial class PicSetting
    {
        /// <summary>
        /// 限制宽度
        /// </summary>
        public static int LimitWidth = 1920;

        /// <summary>
        /// 限制高度
        /// </summary>
        public static int LimitHeight = 1080;

        /// <summary>
        /// 裁剪模式
        /// </summary>
        public static PicUnit.ResizeType ResizeType = PicUnit.ResizeType.Non;
    }
}