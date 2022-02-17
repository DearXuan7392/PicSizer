using System;
using System.Runtime.InteropServices;
using System.Threading;
using PicSizer.Class.Static;

namespace PicSizer.Class.Partial
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
                        Value.setting.useGPU = true;
                        Value.settingForm.checkBox_UseGPU.Enabled = true;
                    }
                }
                catch (Exception)
                {
                    //Dialog.ShowDialog_Exception(e);
                }
            })
            {
                Priority = ThreadPriority.Highest
            };
            thread.Start();
        }
    }
}
