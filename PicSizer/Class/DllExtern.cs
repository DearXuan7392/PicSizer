using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PicSizer
{
    public static class DllExtern
    {
        const string dll_path = "PicSizer_CUDA.dll";

        [DllImport(dll_path, EntryPoint = "SetBrightness", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetBrightness(IntPtr ori, int length, byte dark);
        
        [DllImport(dll_path, EntryPoint = "IsGPUSupport", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsGPUSupport();

        public static void DoInFirst()
        {
            Thread thread = new Thread(() =>
            {
                try
                {
                    Info.isGPUSupport = IsGPUSupport();
                    if (Info.isGPUSupport)
                    {
                        Setting.useGPU = true;
                    }
                }
                catch (Exception e)
                {
                    Dialog.ShowDialog_Exception(e);
                }
            });
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
    }
}
