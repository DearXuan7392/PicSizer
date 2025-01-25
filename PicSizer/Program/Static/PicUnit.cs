#region

using System.Runtime.InteropServices;

#endregion

namespace PicSizer.Program.Static
{
    public static class PicUnit
    {
        /// <summary>
        /// 输出类型
        /// </summary>
        public enum OutputType
        {
            CoverOrigin = 0, //覆盖源文件
            OutputStructure = 1, //输出到文件夹并保留结构
            OutputDirection = 2, //输出到单独文件夹
        }

        /// <summary>
        /// 后缀类型
        /// </summary>
        public enum ExtensionType
        {
            Jpeg = 0,
            Png = 1,
            Webp = 2,
            Origin = 3,
        }

        /// <summary>
        /// 压缩类型
        /// </summary>
        public enum CompressType
        {
            Quality = 0,
            FileSize = 1,
        }

        /// <summary>
        /// 图片项目状态
        /// </summary>
        public enum PicItemState
        {
            Waiting = 0,
            Compression = 1,
            Success = 2,
            OutOfLimit = 3,
            Error = 4,
        }

        /// <summary>
        /// 水印类型
        /// </summary>
        public enum WatermarkType
        {
            Non = 0,
            Center = 1,
            LeftTop = 2,
            LeftBottom = 3,
            RightTop = 4,
            RightBottom = 5,
        }

        /// <summary>
        /// 修正图片尺寸
        /// </summary>
        public enum ResizeType
        {
            Non = 0, //无修正
            NotSmallerThanLimit = 1, //不小于限定值
            NotBiggerThanLimit = 2, //不大于限定值
            ForceCut = 3, //强制居中裁剪
            ForceScale = 4, //强制缩放到指定的大小
        }

        /// <summary>
        /// Dll返回的结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct PicDllResultStr
        {
            /// <summary>
            /// Dll是否加载成功
            /// </summary>
            public bool enable;

            /// <summary>
            /// 支持的GPU数量
            /// </summary>
            public int GpuCount;

            /// <summary>
            /// 当前GPU名称
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public char[] GpuName;

            /// <summary>
            /// GPU最大线程数
            /// </summary>
            public int GpuMaxThreadsPerBlock;

            /// <summary>
            /// GPU Grid数量
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public int[] GpuGrid;

            /// <summary>
            /// GPU 错误名称
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public char[] GpuErrorName;

            /// <summary>
            /// GPU 报错信息
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
            public char[] GpuErrorStr;
        }
    }
}