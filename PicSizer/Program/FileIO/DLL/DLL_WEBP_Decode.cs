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
        [DllImportAttribute("libwebp", EntryPoint = "WebPGetDecoderVersion", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPGetDecoderVersion();


        /// <summary>
        /// Retrieve basic header information: width, height.
        /// This function will also validate the header and return 0 in
        /// case of formatting error.
        /// Pointers 'width' and 'height' can be passed NULL if deemed irrelevant.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="data_size"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [DllImportAttribute("libwebp", EntryPoint = "WebPGetInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPGetInfo([InAttribute()] IntPtr data, UIntPtr data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImportAttribute("libwebp", EntryPoint = "WebPDecodeRGBA", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeRGBA([InAttribute()] IntPtr data, UIntPtr data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImportAttribute("libwebp", EntryPoint = "WebPDecodeARGB", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeARGB([InAttribute()] IntPtr data, UIntPtr data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImportAttribute("libwebp", EntryPoint = "WebPDecodeBGRA", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeBGRA([InAttribute()] IntPtr data, UIntPtr data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImportAttribute("libwebp", EntryPoint = "WebPDecodeRGB", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeRGB([InAttribute()] IntPtr data, UIntPtr data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImportAttribute("libwebp", EntryPoint = "WebPDecodeBGR", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeBGR([InAttribute()] IntPtr data, UIntPtr data_size, ref int width, ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        ///u: uint8_t**
        ///v: uint8_t**
        ///stride: int*
        ///uv_stride: int*
        [DllImportAttribute("libwebp", EntryPoint = "WebPDecodeYUV", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeYUV([InAttribute()] IntPtr data, UIntPtr data_size, ref int width, ref int height, ref IntPtr u, ref IntPtr v, ref int stride, ref int uv_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImportAttribute("libwebp", EntryPoint = "WebPDecodeRGBAInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeRGBAInto([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImportAttribute("libwebp", EntryPoint = "WebPDecodeARGBInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeARGBInto([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImportAttribute("libwebp", EntryPoint = "WebPDecodeBGRAInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeBGRAInto([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImportAttribute("libwebp", EntryPoint = "WebPDecodeRGBInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeRGBInto([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImportAttribute("libwebp", EntryPoint = "WebPDecodeBGRInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeBGRInto([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///luma: uint8_t*
        ///luma_size: size_t->unsigned int
        ///luma_stride: int
        ///u: uint8_t*
        ///u_size: size_t->unsigned int
        ///u_stride: int
        ///v: uint8_t*
        ///v_size: size_t->unsigned int
        ///v_stride: int
        [DllImportAttribute("libwebp", EntryPoint = "WebPDecodeYUVInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeYUVInto([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr luma, UIntPtr luma_size, int luma_stride, IntPtr u, UIntPtr u_size, int u_stride, IntPtr v, UIntPtr v_size, int v_stride);
    }
}
