using PicSizer.Class.Partial;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using PicSizer.Class.Static;

namespace PicSizer.Class.PictureProc
{
    /// <summary>
    /// 图像裁剪，缩放
    /// </summary>
    public static partial class Compress
    {
        public static Bitmap GetBitmapFromPath(string path)
        {
            Bitmap source = null;
            try
            {
                source = new Bitmap(path);
                return new Bitmap(source);
            }
            finally
            {
                source?.Dispose();
            }
        }

        
    }

    /// <summary>
    /// 图像压缩
    /// </summary>
    public static partial class Compress
    {
        /// <summary>
        /// 根据生成的类型自动选择压缩方式
        /// </summary>
        public static bool CompressionBySize(string file)
        {
            AtomBitmap atomBitmap = null;
            try
            {
                //加载图片
                atomBitmap = new AtomBitmap(file);
                //获取输出路径
                atomBitmap.output = FileCheck.GetResultFileName(file, ThreadsPool.OutputDir, ThreadsPool.GetPicNum());
                //压缩为JPEG,
                if (atomBitmap.exportImageFormat == ImageFormat.Jpeg)
                {
                    //压缩到指定大小
                    if (Value.setting.compressionMode == CompressionMode.SizeFirst)
                    {
                        return CompressionBySize_QualityFirst(atomBitmap.bitmap, atomBitmap.output, Encoder._Info_JPEG);
                    }
                    //压缩到指定画质
                    else
                    {
                        return CompressionByValue(atomBitmap.bitmap, atomBitmap.output);
                    }

                }
                //压缩为ICON
                else if (atomBitmap.exportImageFormat == ImageFormat.Icon)
                {
                    return CompressionBySize_ScaleAndPixelDeep(atomBitmap);
                }
                //其它
                else
                {
                    if (Value.setting.nonJEPGCompressMethod == NonJEPGCompressMethod.PixelDeepBased)
                    {
                        return CompressionBySize_PixelDeepFirst(atomBitmap);
                    }
                    else
                    {
                        return CompressionBySize_ScaleFirst(atomBitmap);
                    }
                }
            }
            finally
            {
                atomBitmap?.Dispose();
            }
        }

        /// <summary>
        /// 计算指定画质下Bitmap输出到流后的大小(仅限JPEG)
        /// </summary>
        private static long GetBitmapSizeByQuality(Bitmap bitmap, EncoderParameters encoderParameters, ImageCodecInfo info, long value)
        {
            encoderParameters.Param[0] = Encoder.GetParameter(value);
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, info, encoderParameters);
                long size = memoryStream.Length >> 10;
                return size;
            }
            finally
            {
                memoryStream?.Dispose();
            }
        }

        /// <summary>
        /// 计算指定缩放比例下Bitmap输出到流后的大小(非JPEG)
        /// </summary>
        private static long GetBitmapSizeByScale(Bitmap bitmap, ImageFormat imageFormat, int width, int height)
        {
            Bitmap output = null;
            Graphics g = null;
            try
            {
                output = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                g = Graphics.FromImage(output);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(bitmap,
                    new Rectangle(0, 0, width, height), //画在新Bitmap上的区域
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height), //老Bitmap截取的区域
                    GraphicsUnit.Pixel);
                return BitmapSave.LengthOfBitmapInMemory(output, imageFormat);
            }
            finally
            {
                g?.Dispose();
                output?.Dispose();
            }
        }

        /// <summary>
        /// 计算指定位深度下Bitmap输出到流后的大小(非JPEG)
        /// </summary>
        private static long GetBitmapSizeByPixelDeep(Bitmap bitmap, ImageFormat imageFormat, Rectangle rect, PixelFormat pixelFormat)
        {
            Bitmap output = null;
            try
            {
                output = bitmap.Clone(rect, pixelFormat);//用指定的位深度复制Bitmap
                return BitmapSave.LengthOfBitmapInMemory(output, imageFormat);
            }
            finally
            {
                output?.Dispose();
            }
        }

        /// <summary>
        /// 计算指定分辨率下Bitmap输出到流后的大小(非JPEG)
        /// </summary>
        private static long GetBitmapSizeByDpi(Bitmap bitmap, ImageFormat imageFormat, float xDpi, float yDpi)
        {
            Bitmap output = null;
            try
            {
                output = new Bitmap(bitmap);
                output.SetResolution(xDpi, yDpi);
                return BitmapSave.LengthOfBitmapInMemory(output, imageFormat);
            }
            finally
            {
                output?.Dispose();
            }
        }

        /// <summary>
        /// 基于画质压缩(仅限JPEG)
        /// </summary>
        public static bool CompressionByValue(Bitmap original, string output)
        {
            try
            {
                Bitmap bitmap = ResizeHelper.ResizeBitmap(original);
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = Encoder.GetParameter(Value.setting.CompressionValue);
                bitmap.Save(output, Encoder._Info_JPEG, encoderParameters);
                return true;
            }
            finally
            {
                original?.Dispose();
            }
        }

        /// <summary>
        /// 基于大小压缩,依照画质区分(仅限JPEG)
        /// </summary>
        private static bool CompressionBySize_QualityFirst(Bitmap bitmap, string output, ImageCodecInfo image_type)
        {
            using (bitmap = ResizeHelper.ResizeBitmap(bitmap))
            {
                EncoderParameters encoderParameters = new EncoderParameters(1);
                long left = 0L, right = 100L, mid = 0L;
                long[] sizeList = new long[101];
                while (left < right - 1)
                {
                    mid = (left + right) / 2;
                    sizeList[mid] = GetBitmapSizeByQuality(bitmap, encoderParameters, image_type, mid);
                    if (sizeList[mid] <= Value.setting.LimitSize)
                    {
                        left = mid;
                    }
                    else
                    {
                        right = mid;
                    }
                }
                //获取画质为left时的大小，即是不大于LimitSize的最高画质
                if (sizeList[left] == 0)
                {
                    sizeList[left] = GetBitmapSizeByQuality(bitmap, encoderParameters, image_type, left);
                }
                //如果文件大小符合要求或者接受超出限制的文件就输出
                if (sizeList[left] <= Value.setting.LimitSize || Value.setting.AcceptExceedPicture)
                {
                    encoderParameters.Param[0] = Encoder.GetParameter(left);
                    bitmap.Save(output, image_type, encoderParameters);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 基于大小压缩,依照缩放比例区分(非JPEG)
        /// </summary>
        private static bool CompressionBySize_ScaleFirst(AtomBitmap atomBitmap)
        {
            int left = 1, right = 100, mid = 50;
            long[] sizeList = new long[101];
            while (left < right - 1)
            {
                mid = (left + right) / 2;
                sizeList[mid] = GetBitmapSizeByScale(atomBitmap.bitmap, atomBitmap.exportImageFormat, atomBitmap.bitmap.Width * mid / 100, atomBitmap.bitmap.Height * mid / 100);
                if (sizeList[mid] <= Value.setting.LimitSize)
                {
                    left = mid;
                }
                else
                {
                    right = mid;
                }
            }
            //获取缩放为left时的大小，即是不大于LimitSize的最高画质
            if (sizeList[left] == 0)
            {
                sizeList[left] = GetBitmapSizeByScale(atomBitmap.bitmap, atomBitmap.exportImageFormat, atomBitmap.bitmap.Width * mid / 100, atomBitmap.bitmap.Height * mid / 100);
            }
            //如果文件大小符合要求或者接受超出限制的文件就输出
            if (sizeList[left] <= Value.setting.LimitSize || Value.setting.AcceptExceedPicture)
            {
                using (Bitmap result = ResizeHelper.ScaleBitmap(atomBitmap.bitmap, atomBitmap.bitmap.Width * left / 100, atomBitmap.bitmap.Height * left / 100))
                {
                    BitmapSave.SaveBitmapToFile(result, atomBitmap.output, atomBitmap.exportImageFormat);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 基于大小压缩,依照分辨率区分(非JPEG)
        /// </summary>
        private static bool CompressionBySize_DpiFirst(AtomBitmap atomBitmap)
        {
            int maxDpi = (int)Math.Max(atomBitmap.bitmap.HorizontalResolution, atomBitmap.bitmap.VerticalResolution);
            int left = 1, right = maxDpi, mid = 0;
            long[] sizeList = new long[right + 1];
            while (left < right - 1)
            {
                mid = (left + right) / 2;
                sizeList[mid] = GetBitmapSizeByDpi(atomBitmap.bitmap, atomBitmap.exportImageFormat, atomBitmap.bitmap.HorizontalResolution * mid / maxDpi, atomBitmap.bitmap.VerticalResolution * mid / maxDpi);
                if (sizeList[mid] <= Value.setting.LimitSize)
                {
                    left = mid;
                }
                else
                {
                    right = mid;
                }
            }
            //获取Dpi为left时的大小，即是不大于LimitSize的最高画质
            if (sizeList[left] == 0)
            {
                sizeList[left] = GetBitmapSizeByDpi(atomBitmap.bitmap, atomBitmap.exportImageFormat, atomBitmap.bitmap.HorizontalResolution * left / maxDpi, atomBitmap.bitmap.VerticalResolution * left / maxDpi);
            }
            //如果文件大小符合要求或者接受超出限制的文件就输出
            if (sizeList[left] <= Value.setting.LimitSize || Value.setting.AcceptExceedPicture)
            {
                atomBitmap.bitmap.SetResolution(atomBitmap.bitmap.HorizontalResolution * left / maxDpi, atomBitmap.bitmap.VerticalResolution * left / maxDpi);
                BitmapSave.SaveBitmapToFile(atomBitmap.bitmap, atomBitmap.output, atomBitmap.exportImageFormat);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 基于大小压缩,依照位深度区分(非JPEG)
        /// </summary>
        private static bool CompressionBySize_PixelDeepFirst(AtomBitmap atomBitmap)
        {
            Rectangle rect = new Rectangle(0, 0, atomBitmap.bitmap.Width, atomBitmap.bitmap.Height);
            int left = 0, right = Encoder.pixelFormats.Length - 1, mid = 0;
            long[] sizeList = new long[Encoder.pixelFormats.Length];
            while (left < right - 1)
            {
                mid = (left + right) / 2;
                sizeList[mid] = GetBitmapSizeByPixelDeep(atomBitmap.bitmap, atomBitmap.exportImageFormat, rect, Encoder.pixelFormats[mid]);
                if (sizeList[mid] <= Value.setting.LimitSize)
                {
                    left = mid;
                }
                else
                {
                    right = mid;
                }
            }
            //获取画质为left时的大小，即是不大于LimitSize的最高画质
            if (sizeList[left] == 0)
            {
                sizeList[left] = GetBitmapSizeByPixelDeep(atomBitmap.bitmap, atomBitmap.exportImageFormat, rect, Encoder.pixelFormats[left]);
            }
            //如果文件大小符合要求或者接受超出限制的文件就输出
            if (sizeList[left] <= Value.setting.LimitSize || Value.setting.AcceptExceedPicture)
            {
                using(Bitmap result = atomBitmap.bitmap.Clone(rect, Encoder.pixelFormats[left]))
                {
                    BitmapSave.SaveBitmapToFile(result, atomBitmap.output, atomBitmap.exportImageFormat);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 基于大小压缩,先缩放再按照位深度区分(仅限ICON)
        /// </summary>
        private static bool CompressionBySize_ScaleAndPixelDeep(AtomBitmap atomBitmap)
        {
            Rectangle rect = new Rectangle(0, 0, atomBitmap.bitmap.Width, atomBitmap.bitmap.Height);
            int left = 0, right = Encoder.pixelFormats.Length - 1, mid = 0;
            long[] sizeList = new long[Encoder.pixelFormats.Length];
            while (left < right - 1)
            {
                mid = (left + right) / 2;
                sizeList[mid] = GetBitmapSizeByPixelDeep(atomBitmap.bitmap, ImageFormat.Png, rect, Encoder.pixelFormats[mid]);
                if (sizeList[mid] <= Value.setting.LimitSize)
                {
                    left = mid;
                }
                else
                {
                    right = mid;
                }
            }
            //获取画质为left时的大小，即是不大于LimitSize的最高画质
            if (sizeList[left] == 0)
            {
                sizeList[left] = GetBitmapSizeByPixelDeep(atomBitmap.bitmap, ImageFormat.Png, rect, Encoder.pixelFormats[left]);
            }
            //如果文件大小符合要求或者接受超出限制的文件就输出
            if (sizeList[left] <= Value.setting.LimitSize || Value.setting.AcceptExceedPicture)
            {
                using (Bitmap result = atomBitmap.bitmap.Clone(rect, Encoder.pixelFormats[left]))
                {
                    BitmapSave.SaveBitmapToFile(result, atomBitmap.output, ImageFormat.Icon);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
