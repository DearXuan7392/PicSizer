#region

using System;
using System.Runtime.InteropServices;

#endregion

namespace PicSizer.Program.FileIO
{
    /// <summary>
    /// PicSizer dll加载类
    /// </summary>
    public static partial class Dll
    {
        /// <summary>
        /// C++ 设置图片亮度
        /// </summary>
        /// <param name="ori">像素起始路径</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        /// <param name="stride">扫描行</param>
        /// <param name="dark">亮度</param>
        [DllImport(DllPath, EntryPoint = "CPP_SetBrightness", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CPP_SetBrightness(IntPtr ori, int width, int height, int stride, byte brightness);

        /// <summary>
        /// C++ 设置透明像素背景色
        /// </summary>
        /// <param name="ori">像素起始路径</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        /// <param name="stride">扫描行</param>
        /// <param name="R">R参量</param>
        /// <param name="G">G参量</param>
        /// <param name="B">B参量</param>
        /// <returns></returns>
        [DllImport(DllPath, EntryPoint = "CPP_SetTransparentPixelBackgroundColor",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CPP_SetTransparentPixelBackgroundColor(IntPtr ori, int width, int height, int stride,
            byte R, byte G, byte B);

        /// <summary>
        /// CUDA 压缩PNG图片
        /// </summary>
        /// <param name="ori">像素起始路径</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        /// <param name="stride">扫描行</param>
        /// <param name="pixelBits">像素位数</param>
        /// <param name="level">压缩强度</param>
        [DllImport(DllPath, EntryPoint = "CUDA_CompressPNG", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CPP_CompressPNG(IntPtr ori, int width, int height, int stride, int pixelBits,
            int level);
    }
}