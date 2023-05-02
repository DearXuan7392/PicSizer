using PicSizer.Window.Partial;
using PicSizer.Window.Assemble;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PicSizer.Static.PicUnit;
using System.Windows.Forms;
using PicSizer.Window;
using PicSizer.Window.Forms;

namespace PicSizer.Static
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
        public static PicListView picListView = null;

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
        public static bool IsGPUSupport = false;

        /// <summary>
        /// DLL加载结果
        /// </summary>
        public static PicDllResultStr picDllResult = new PicDllResultStr();
    }
}
