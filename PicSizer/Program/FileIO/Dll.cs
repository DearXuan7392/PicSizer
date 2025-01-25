#region

using System;
using System.Runtime.InteropServices;
using PicSizer.Program.Static;
using PicSizer.Program.Window.Partial;
using static PicSizer.Program.Static.PicUnit;

#endregion

namespace PicSizer.Program.FileIO
{
    public static partial class Dll
    {
        /// <summary>
        /// 通用dll路径
        /// </summary>
        private const string DllPath = "PicLibrary.dll";

        /// <summary>
        /// CUDA 判断GPU是否支持CUDA
        /// </summary>
        /// <returns></returns>
        [DllImport(DllPath, EntryPoint = "LoadDll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _LoadDll(ref PicDllResultStr res);

        /// <summary>
        /// 在程序刚开始执行,异步加载CUDA.dll
        /// </summary>
        public static void LoadDll()
        {
            try
            {
                //返回CUDA错误码
                int picDllError = _LoadDll(ref PicValue.PicDllResult);
                //未报错且GPU数量大于 0,则启用加速
                if (picDllError == 0 && PicValue.PicDllResult.GpuCount > 0)
                {
                    PicValue.IsGpuSupport = true;
                }
            }
            catch (Exception e)
            {
                Dialog.ShowDialog_Exception(e);
            }
        }
    }
}