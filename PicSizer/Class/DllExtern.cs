using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer
{
    public static class DllExtern
    {
        [DllImport("D:/PicSizer_CUDA.dll", EntryPoint = "SetBrightness", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetBrightness(IntPtr ori, int length, byte dark);
        
        [DllImport("D:/PicSizer_CUDA.dll", EntryPoint = "IsGPUSupport", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsGPUSupport();
    }
}
