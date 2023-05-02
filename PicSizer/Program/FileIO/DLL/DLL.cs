using System;
using System.Runtime.InteropServices;
using System.Threading;
using PicSizer.Static;
using PicSizer.Window.Partial;
using static PicSizer.Static.PicUnit;

namespace PicSizer.FileIO
{
    public static partial class DLL
    {
        /// <summary>
        /// 通用dll路径
        /// </summary>
        private const string DLL_PATH = "PicLibrary.dll";

        /// <summary>
        /// CUDA 判断GPU是否支持CUDA
        /// </summary>
        /// <returns></returns>
        [DllImport(DLL_PATH, EntryPoint = "LoadDll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _LoadDll(ref PicDllResultStr res);

        /// <summary>
        /// 在程序刚开始执行,异步加载CUDA.dll
        /// </summary>
        public static void LoadDll()
        {
            try
            {
                //返回CUDA错误码
                int _Pic_Dll_Error = _LoadDll(ref PicValue.picDllResult);
                //未报错且GPU数量大于 0,则启用加速
                if (_Pic_Dll_Error == 0 && PicValue.picDllResult.GpuCount > 0)
                {
                    PicValue.IsGPUSupport = true;
                }
            }
            catch (Exception e)
            {
                Dialog.ShowDialog_Exception(e);
            }
        }
    }
}
