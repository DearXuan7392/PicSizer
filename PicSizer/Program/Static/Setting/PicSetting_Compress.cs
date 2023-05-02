using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PicSizer.Static.PicUnit;

namespace PicSizer.Static
{
    public static partial class PicSetting
    {
        /// <summary>
        /// 输出类型
        /// </summary>
        public static OutputType OutputType = OutputType.OutputDirection;

        /// <summary>
        /// 后缀格式
        /// </summary>
        public static ExtensionType ExtensionType = ExtensionType.JPEG;

        /// <summary>
        /// 按画质压缩的画质值(1~100)
        /// </summary>
        public static int Strength = 80;

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
        public static CompressType CompressType = CompressType.Strength;
    }
}
