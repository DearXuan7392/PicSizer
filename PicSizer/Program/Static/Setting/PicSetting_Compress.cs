#region

using static PicSizer.Program.Static.PicUnit;

#endregion

namespace PicSizer.Program.Static
{
    public static partial class PicSetting
    {
        /// <summary>
        /// 输出类型
        /// </summary>
        public static PicUnit.OutputType OutputType = PicUnit.OutputType.OutputDirection;

        /// <summary>
        /// 后缀格式
        /// </summary>
        public static PicUnit.ExtensionType ExtensionType = PicUnit.ExtensionType.Jpeg;

        /// <summary>
        /// 按画质压缩的画质值(1~100)
        /// </summary>
        public static int Quality = 80;

        /// <summary>
        /// 是否接受超出大小的图片
        /// </summary>
        public static bool AcceptExceedPicture = false;

        /// <summary>
        /// 指定大小(KB)
        /// </summary>
        public static long LimitSize = 200;

        /// <summary>
        /// 输出文件名
        /// </summary>
        public static string OutputFilename = "{index}.jpg";

        /// <summary>
        /// 输出图片的起始下标
        /// </summary>
        public static int OutputIndex = 1;

        /// <summary>
        /// 压缩方式
        /// </summary>
        public static PicUnit.CompressType CompressType = PicUnit.CompressType.Quality;
    }
}