#region

using System;
using System.Runtime.InteropServices;

#endregion

namespace PicSizer.Program.FileIO
{
    public static partial class Dll
    {
        /// Return Type: int
        [DllImport("libwebp", EntryPoint = "WebPGetEncoderVersion",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPGetEncoderVersion();


        /// Return Type: size_t->unsigned int
        ///rgb: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///quality_factor: float
        ///output: uint8_t**
        [DllImport("libwebp", EntryPoint = "WebPEncodeRGB", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeRGB([In] IntPtr rgb, int width, int height, int stride,
            float quality_factor, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///bgr: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///quality_factor: float
        ///output: uint8_t**
        [DllImport("libwebp", EntryPoint = "WebPEncodeBGR", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeBGR([In] IntPtr bgr, int width, int height, int stride,
            float quality_factor, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///rgba: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///quality_factor: float
        ///output: uint8_t**
        [DllImport("libwebp", EntryPoint = "WebPEncodeRGBA", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeRGBA([In] IntPtr rgba, int width, int height, int stride,
            float quality_factor, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///bgra: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///quality_factor: float
        ///output: uint8_t**
        [DllImport("libwebp", EntryPoint = "WebPEncodeBGRA", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPEncodeBGRA([In] IntPtr bgra, int width, int height, int stride,
            float quality_factor, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///rgb: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///output: uint8_t**
        [DllImport("libwebp", EntryPoint = "WebPEncodeLosslessRGB",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeLosslessRGB([In] IntPtr rgb, int width, int height,
            int stride, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///bgr: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///output: uint8_t**
        [DllImport("libwebp", EntryPoint = "WebPEncodeLosslessBGR",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeLosslessBGR([In] IntPtr bgr, int width, int height,
            int stride, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///rgba: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///output: uint8_t**
        [DllImport("libwebp", EntryPoint = "WebPEncodeLosslessRGBA",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeLosslessRGBA([In] IntPtr rgba, int width, int height,
            int stride, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///bgra: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///output: uint8_t**
        [DllImport("libwebp", EntryPoint = "WebPEncodeLosslessBGRA",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeLosslessBGRA([In] IntPtr bgra, int width, int height,
            int stride, ref IntPtr output);
    }
}