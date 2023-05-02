using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.FileIO
{
    public static partial class DLL
    {
        /// Return Type: int
        [DllImportAttribute("libwebp", EntryPoint = "WebPGetEncoderVersion", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPGetEncoderVersion();


        /// Return Type: size_t->unsigned int
        ///rgb: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///quality_factor: float
        ///output: uint8_t**
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncodeRGB", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeRGB([InAttribute()] IntPtr rgb, int width, int height, int stride, float quality_factor, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///bgr: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///quality_factor: float
        ///output: uint8_t**
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncodeBGR", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeBGR([InAttribute()] IntPtr bgr, int width, int height, int stride, float quality_factor, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///rgba: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///quality_factor: float
        ///output: uint8_t**
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncodeRGBA", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeRGBA([InAttribute()] IntPtr rgba, int width, int height, int stride, float quality_factor, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///bgra: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///quality_factor: float
        ///output: uint8_t**
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncodeBGRA", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPEncodeBGRA([InAttribute()] IntPtr bgra, int width, int height, int stride, float quality_factor, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///rgb: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///output: uint8_t**
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncodeLosslessRGB", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeLosslessRGB([InAttribute()] IntPtr rgb, int width, int height, int stride, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///bgr: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///output: uint8_t**
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncodeLosslessBGR", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeLosslessBGR([InAttribute()] IntPtr bgr, int width, int height, int stride, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///rgba: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///output: uint8_t**
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncodeLosslessRGBA", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeLosslessRGBA([InAttribute()] IntPtr rgba, int width, int height, int stride, ref IntPtr output);


        /// Return Type: size_t->unsigned int
        ///bgra: uint8_t*
        ///width: int
        ///height: int
        ///stride: int
        ///output: uint8_t**
        [DllImportAttribute("libwebp", EntryPoint = "WebPEncodeLosslessBGRA", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr WebPEncodeLosslessBGRA([InAttribute()] IntPtr bgra, int width, int height, int stride, ref IntPtr output);
    }
}
