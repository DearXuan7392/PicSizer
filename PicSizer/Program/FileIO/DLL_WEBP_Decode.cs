#region

using System;
using System.Runtime.InteropServices;

#endregion

namespace PicSizer.Program.FileIO
{
    public static partial class Dll
    {
        /// Return Type: int
        [DllImport("libwebp", EntryPoint = "WebPGetDecoderVersion",
            CallingConvention = CallingConvention.Cdecl)]
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
        [DllImport("libwebp", EntryPoint = "WebPGetInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WebPGetInfo([In] IntPtr data, UIntPtr data_size, ref int width,
            ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImport("libwebp", EntryPoint = "WebPDecodeRGBA", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeRGBA([In] IntPtr data, UIntPtr data_size, ref int width,
            ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImport("libwebp", EntryPoint = "WebPDecodeARGB", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeARGB([In] IntPtr data, UIntPtr data_size, ref int width,
            ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImport("libwebp", EntryPoint = "WebPDecodeBGRA", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeBGRA([In] IntPtr data, UIntPtr data_size, ref int width,
            ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImport("libwebp", EntryPoint = "WebPDecodeRGB", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeRGB([In] IntPtr data, UIntPtr data_size, ref int width,
            ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        [DllImport("libwebp", EntryPoint = "WebPDecodeBGR", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeBGR([In] IntPtr data, UIntPtr data_size, ref int width,
            ref int height);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///width: int*
        ///height: int*
        ///u: uint8_t**
        ///v: uint8_t**
        ///stride: int*
        ///uv_stride: int*
        [DllImport("libwebp", EntryPoint = "WebPDecodeYUV", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeYUV([In] IntPtr data, UIntPtr data_size, ref int width,
            ref int height, ref IntPtr u, ref IntPtr v, ref int stride, ref int uv_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImport("libwebp", EntryPoint = "WebPDecodeRGBAInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeRGBAInto([In] IntPtr data, UIntPtr data_size,
            IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImport("libwebp", EntryPoint = "WebPDecodeARGBInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeARGBInto([In] IntPtr data, UIntPtr data_size,
            IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImport("libwebp", EntryPoint = "WebPDecodeBGRAInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeBGRAInto([In] IntPtr data, UIntPtr data_size,
            IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImport("libwebp", EntryPoint = "WebPDecodeRGBInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeRGBInto([In] IntPtr data, UIntPtr data_size,
            IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);


        /// Return Type: uint8_t*
        ///data: uint8_t*
        ///data_size: size_t->unsigned int
        ///output_buffer: uint8_t*
        ///output_buffer_size: size_t->unsigned int
        ///output_stride: int
        [DllImport("libwebp", EntryPoint = "WebPDecodeBGRInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeBGRInto([In] IntPtr data, UIntPtr data_size,
            IntPtr output_buffer, UIntPtr output_buffer_size, int output_stride);


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
        [DllImport("libwebp", EntryPoint = "WebPDecodeYUVInto", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr WebPDecodeYUVInto([In] IntPtr data, UIntPtr data_size, IntPtr luma,
            UIntPtr luma_size, int luma_stride, IntPtr u, UIntPtr u_size, int u_stride, IntPtr v, UIntPtr v_size,
            int v_stride);
    }
}