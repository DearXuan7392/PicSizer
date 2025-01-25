#region

using System;
using PicSizer.Program.Window.Assemble;
using static PicSizer.Program.Static.PicUnit;

#endregion

namespace PicSizer.Program.Static
{
    public static class PicValue
    {
        /// <summary>
        /// 输出路径
        /// </summary>
        public static string OutputDirection = "";

        /// <summary>
        /// 公共目录
        /// </summary>
        public static string PublicDirectory = "";

        /// <summary>
        /// PicListView
        /// </summary>
        public static PicListView PicListView = null;

        /// <summary>
        /// 压缩过程中立即退出
        /// </summary>
        public static bool ExitNow = false;

        /// <summary>
        /// PicListView的选中项改变监听是否生效
        /// </summary>
        public static bool PicListSelectListenerEnable = true;

        /// <summary>
        /// CPU核心数量
        /// </summary>
        public static int CpuCoresNum = Environment.ProcessorCount;

        /// <summary>
        /// GPU是否支持
        /// </summary>
        public static bool IsGpuSupport = false;

        /// <summary>
        /// DLL加载结果
        /// </summary>
        public static PicDllResultStr PicDllResult = new PicUnit.PicDllResultStr();
    }
}