using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer
{
    public class Setting
    {
        //尺寸修正模式
        public ResizeMode resizeMode = ResizeMode.MinSize;

        //压缩模式
        public CompressionMode compressionMode = CompressionMode.SizeFirst;

        //命名方式
        public RenameMode renameMode = RenameMode.Number;

        //文件后缀
        public ExtensionMode extensionMode = ExtensionMode.JPEG;

        //限制尺寸
        public int LimitWidth = 1920;
        public int LimitHeight = 1080;

        //指定大小(KB)
        public long LimitSize = 200;

        //指定画质
        public long CompressionValue = 80L;

        //起始下标
        public int StartIndex = 1;

        //混合方式命名
        public string CustomRenameStr;

        //错误处理
        public DoWhenException doWhenException = DoWhenException.IgnoreAndContinue;

        //允许任意后缀
        public bool AllowAnyExtension = false;

        //亮度
        public byte brightness = 100; // 0表示完全黑暗，100表示不变暗

        //最大线程数
        public int maxThreads = 1;

        //是否启动GPU加速
        public bool useGPU = false;
    }
}
