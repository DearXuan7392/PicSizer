using PicSizer.FileIO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Logic
{
    public partial class WEBP : CompressTaskItem, Compress_Based, ITaskBased
    {
        public WEBP(Bitmap bitmap) : base(bitmap) { }

        public static unsafe Bitmap GetBitmap(string path)
        {
            FileStream fs = null;
            int width = 0, height = 0;
            long length = 0;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                length = fs.Length;
                byte[] _data = new byte[length];
                fs.Read(_data, 0, (int)length);
                fs.Close();

                fixed (byte* dataPtr = _data)
                {
                    IntPtr data = (IntPtr)dataPtr;
                    if (DLL.WebPGetInfo(data, (UIntPtr)length, ref width, ref height) == 0)
                    {
                        throw new Exception("图片已损坏");
                    }
                    Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                    BitmapData bitmapData = bitmap.LockBits(
                        new Rectangle(0, 0, width, height),
                        ImageLockMode.ReadWrite,
                        PixelFormat.Format24bppRgb);
                    IntPtr result = DLL.WebPDecodeBGRInto(
                        data, (UIntPtr)length, bitmapData.Scan0, (UIntPtr)(bitmapData.Stride * height), bitmapData.Stride);
                    if (bitmapData.Scan0 != result)
                    {
                        throw new Exception("解码失败");
                    }
                    bitmap.UnlockBits(bitmapData);
                    return bitmap;
                }
            }
            finally
            {
                fs?.Dispose();
            }
        }

        private static unsafe long BitmapToStream(Bitmap bitmap, Stream stream, int quality, bool alpha = false)
        {
            int width = bitmap.Width, height = bitmap.Height;
            IntPtr result = IntPtr.Zero;
            long length = 0;

            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            try
            {
                result = IntPtr.Zero;
                length = (long)DLL.WebPEncodeBGR(bitmapData.Scan0, width, height, bitmapData.Stride, (float)quality / 100, ref result);
                if (length == 0)
                {
                    throw new Exception("无法写入图片");
                }
                byte[] buffer = new byte[4096];
                for(int i = 0; i < length; i += 4096)
                {
                    int used = (int)Math.Min((int)buffer.Length, length - i);
                    Marshal.Copy((IntPtr)(result + i), buffer, 0, used);
                    stream.Write(buffer, 0, used);
                }
                return length;
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }
    }
}
