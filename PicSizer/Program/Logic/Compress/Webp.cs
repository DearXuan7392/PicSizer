#region

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using PicSizer.Program.FileIO;

#endregion

namespace PicSizer.Program.Logic.Compress
{
    public class Webp : CompressItem
    {
        public Webp(string imgPath, string outputFilename)
        {
            Init(imgPath, outputFilename);
        }

        public static Bitmap GetBitmap(Stream stream)
        {
            int width = 0, height = 0;
            long length;
            length = stream.Length;
            byte[] _data = new byte[length];
            stream.Read(_data, 0, (int)length);
            stream.Close();
            unsafe
            {
                fixed (byte* dataPtr = _data)
                {
                    IntPtr data = (IntPtr)dataPtr;
                    if (Dll.WebPGetInfo(data, (UIntPtr)length, ref width, ref height) == 0)
                    {
                        throw new Exception("图片已损坏");
                    }

                    Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                    BitmapData bitmapData = bitmap.LockBits(
                        new Rectangle(0, 0, width, height),
                        ImageLockMode.ReadWrite,
                        PixelFormat.Format24bppRgb);
                    IntPtr result = Dll.WebPDecodeBGRInto(
                        data, (UIntPtr)length, bitmapData.Scan0, (UIntPtr)(bitmapData.Stride * height),
                        bitmapData.Stride);
                    if (bitmapData.Scan0 != result)
                    {
                        throw new Exception("解码失败");
                    }

                    bitmap.UnlockBits(bitmapData);
                    return bitmap;
                }
            }
        }

        protected override void WriteToStreamWithQuality(Stream stream, int quality)
        {
            int width = Img.Width, height = Img.Height;
            IntPtr result = IntPtr.Zero;
            long length = 0;

            BitmapData bitmapData = Img.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            try
            {
                result = IntPtr.Zero;
                length = (long)Dll.WebPEncodeBGR(bitmapData.Scan0, width, height, bitmapData.Stride,
                    (float)quality / 100, ref result);
                if (length == 0)
                {
                    throw new Exception("无法写入图片");
                }

                byte[] buffer = new byte[4096];
                for (int i = 0; i < length; i += 4096)
                {
                    int used = (int)Math.Min(buffer.Length, length - i);
                    Marshal.Copy(result + i, buffer, 0, used);
                    stream.Write(buffer, 0, used);
                }
            }
            finally
            {
                Img.UnlockBits(bitmapData);
            }
        }
    }
}