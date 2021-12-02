using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer
{
    

    public static class Setting
    {
        //线程信号灯
        public static bool ThreadExitNow = false;

        //尺寸修正模式
        public static ResizeMode resizeMode = ResizeMode.MinSize;

        //压缩模式
        public static CompressionMode compressionMode = CompressionMode.SizeFirst;

        //命名方式
        public static RenameMode renameMode = RenameMode.Number;

        //文件后缀
        public static ExtensionMode extensionMode = ExtensionMode.JPEG;

        //限制尺寸
        public static int LimitWidth = 1920;
        public static int LimitHeight = 1080;

        //指定大小(KB)
        public static long LimitSize = 200;

        //指定画质
        public static long CompressionValue = 80L;

        //起始下标
        public static int StartIndex = 1;

        //混合方式命名
        public static string CustomRenameStr;
    }
}
