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
        /// <summary>
        /// 调整图片像素，如果输入了width和height，则无视设置，强制缩放到给定的尺寸
        /// </summary>
        private static Bitmap ResizeBitmap(Bitmap bitmap)
        {
            //图片位深度是24位且关闭了尺寸修正
            if (Value.setting.resizeMode == ResizeMode.None && bitmap.PixelFormat == PixelFormat.Format24bppRgb)
            {
                return bitmap;
            }
            int width = bitmap.Width;
            int height = bitmap.Height;
            //求出比值
            float widthByMin = (float)width / Value.setting.LimitWidth;
            float heightByMin = (float)height / Value.setting.LimitHeight;
            //temp是临时变量，用于计算缩放比例
            float temp;
            //重新设定边长
            switch (Value.setting.resizeMode)
            {
                case ResizeMode.MinSize://不小于限定值
                    temp = Math.Min(widthByMin, heightByMin);
                    if (temp > 1)
                    {
                        width = (int)(width / temp);
                        height = (int)(height / temp);
                    }
                    return ScaleBitmap(bitmap, width, height);
                case ResizeMode.MaxSize://不大于限定值
                    temp = Math.Max(widthByMin, heightByMin);
                    if (temp > 1)
                    {
                        width = (int)(width / temp);
                        height = (int)(height / temp);
                    }
                    return ScaleBitmap(bitmap, width, height);
                case ResizeMode.Custom://强制修正
                    width = Value.setting.LimitWidth;
                    height = Value.setting.LimitHeight;
                    return ScaleBitmap(bitmap, width, height);
                case ResizeMode.Cut://裁剪
                    temp = Math.Min(widthByMin, heightByMin);
                    //缩放图片，使得width和height有一个恰好满足要求，另一个大于等于要求，则下一步仅需要裁剪
                    return CenterCutBitmap(bitmap, temp);
                default://无修正
                    //如果运行到这里说明图片位数不符，无需调整尺寸
                    Bitmap output = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format24bppRgb);
                    bitmap.Dispose();
                    return output;
            }
        }

        /// <summary>
        /// 居中裁剪图片
        /// </summary>
        private static Bitmap CenterCutBitmap(Bitmap bitmap, float scale)
        {
            //width和height是bitmap需要裁剪的区域
            int final_width = (int)(Value.setting.LimitWidth * scale);
            int final_height = (int)(Value.setting.LimitHeight * scale);
            //bitmap的裁剪区域左上角位置
            int left = (bitmap.Width - final_width) / 2;
            int top = (bitmap.Height - final_height) / 2;
            Bitmap newBitmap = new Bitmap(Value.setting.LimitWidth, Value.setting.LimitHeight, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(newBitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(bitmap,
                new Rectangle(0, 0, Value.setting.LimitWidth, Value.setting.LimitHeight),
                new Rectangle(left, top, final_width, final_height),
                GraphicsUnit.Pixel);
            g.Dispose();
            bitmap.Dispose();
            return newBitmap;
        }

        /// <summary>
        /// 缩放图片
        /// </summary>
        private static Bitmap ScaleBitmap(Bitmap bitmap, int width, int height)
        {
            //缩放图片
            Bitmap newBitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(newBitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(bitmap,
                new Rectangle(0, 0, width, height), //画在新Bitmap上的区域
                new Rectangle(0, 0, bitmap.Width, bitmap.Height), //老Bitmap截取的区域
                GraphicsUnit.Pixel);
            g.Dispose();//摧毁
            bitmap.Dispose();//摧毁
            return newBitmap;
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
            ImageFormat imageFormat = FileCheck.GetFileExportFormat(file);
            //压缩为JPEG,
            if (imageFormat == ImageFormat.Jpeg)
            {
                //压缩到指定大小
                if(Value.setting.compressionMode == CompressionMode.SizeFirst)
                {
                    return CompressionBySize_QualityFirst(file, Encoder._Info_JPEG);
                }
                //压缩到指定画质
                else
                {
                    return CompressionByValue(file);
                }
                
            }
            //压缩为ICON
            else if(imageFormat == ImageFormat.Icon)
            {
                return CompressionBySize_ScaleAndPixelDeep(file);
            }
            //其它
            else
            {
                if (Value.setting.nonJEPGCompressMethod == NonJEPGCompressMethod.PixelDeepBased)
                {
                    return CompressionBySize_PixelDeepFirst(file, imageFormat);
                }
                else
                {
                    return CompressionBySize_ScaleFirst(file, imageFormat);
                }
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
        public static bool CompressionByValue(string file)
        {
            Bitmap bitmap = null;
            try
            {
                bitmap = ResizeBitmap(new Bitmap(file));
                BmpProc.SetBrightness(bitmap);
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = Encoder.GetParameter(Value.setting.CompressionValue);
                string result = FileCheck.GetResultFileName(file, ThreadsPool.OutputDir, ThreadsPool.GetPicNum());
                bitmap.Save(result, Encoder._Info_JPEG, encoderParameters);
                return true;
            }
            finally
            {
                bitmap?.Dispose();
            }
        }

        /// <summary>
        /// 基于大小压缩,依照画质区分(仅限JPEG)
        /// </summary>
        private static bool CompressionBySize_QualityFirst(string file, ImageCodecInfo image_type)
        {
            using (Bitmap bitmap = ResizeBitmap(new Bitmap(file)))
            {
                BmpProc.SetBrightness(bitmap);
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
                    string result = FileCheck.GetResultFileName(file, ThreadsPool.OutputDir, ThreadsPool.GetPicNum());
                    bitmap.Save(result, image_type, encoderParameters);
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
        private static bool CompressionBySize_ScaleFirst(string file, ImageFormat imageFormat)
        {
            using (Bitmap bitmap = ResizeBitmap(new Bitmap(file)))
            {
                BmpProc.SetBrightness(bitmap);
                int left = 1, right = 100, mid = 50;
                long[] sizeList = new long[101];
                while (left < right - 1)
                {
                    mid = (left + right) / 2;
                    sizeList[mid] = GetBitmapSizeByScale(bitmap, imageFormat, bitmap.Width * mid / 100, bitmap.Height * mid / 100);
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
                    sizeList[left] = GetBitmapSizeByScale(bitmap, imageFormat, bitmap.Width * mid / 100, bitmap.Height * mid / 100);
                }
                //如果文件大小符合要求或者接受超出限制的文件就输出
                if (sizeList[left] <= Value.setting.LimitSize || Value.setting.AcceptExceedPicture)
                {
                    using (Bitmap output = ScaleBitmap(bitmap, bitmap.Width * left / 100, bitmap.Height * left / 100))
                    {
                        string result = FileCheck.GetResultFileName(file, ThreadsPool.OutputDir, ThreadsPool.GetPicNum());
                        BitmapSave.SaveBitmapToFile(output, result, imageFormat);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 基于大小压缩,依照分辨率区分(非JPEG)
        /// </summary>
        private static bool CompressionBySize_DpiFirst(string file, ImageFormat imageFormat)
        {
            using (Bitmap bitmap = ResizeBitmap(new Bitmap(file)))
            {
                BmpProc.SetBrightness(bitmap);
                int maxDpi = (int)Math.Max(bitmap.HorizontalResolution, bitmap.VerticalResolution);
                int left = 1, right = maxDpi, mid = 0;
                long[] sizeList = new long[right + 1];
                while (left < right - 1)
                {
                    mid = (left + right) / 2;
                    sizeList[mid] = GetBitmapSizeByDpi(bitmap, imageFormat, bitmap.HorizontalResolution * mid / maxDpi, bitmap.VerticalResolution * mid / maxDpi);
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
                    sizeList[left] = GetBitmapSizeByDpi(bitmap, imageFormat, bitmap.HorizontalResolution * left / maxDpi, bitmap.VerticalResolution * left / maxDpi);
                }
                //如果文件大小符合要求或者接受超出限制的文件就输出
                if (sizeList[left] <= Value.setting.LimitSize || Value.setting.AcceptExceedPicture)
                {
                    bitmap.SetResolution(bitmap.HorizontalResolution * left / maxDpi, bitmap.VerticalResolution * left / maxDpi);
                    string result = FileCheck.GetResultFileName(file, ThreadsPool.OutputDir, ThreadsPool.GetPicNum());
                    BitmapSave.SaveBitmapToFile(bitmap, result, imageFormat);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 基于大小压缩,依照位深度区分(非JPEG)
        /// </summary>
        private static bool CompressionBySize_PixelDeepFirst(string file, ImageFormat imageFormat)
        {
            using (Bitmap bitmap = ResizeBitmap(new Bitmap(file)))
            {
                BmpProc.SetBrightness(bitmap);
                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                int left = 0, right = Encoder.pixelFormats.Length - 1, mid = 0;
                long[] sizeList = new long[Encoder.pixelFormats.Length];
                while (left < right - 1)
                {
                    mid = (left + right) / 2;
                    sizeList[mid] = GetBitmapSizeByPixelDeep(bitmap, imageFormat, rect, Encoder.pixelFormats[mid]);
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
                    sizeList[left] = GetBitmapSizeByPixelDeep(bitmap, imageFormat, rect, Encoder.pixelFormats[left]);
                }
                //如果文件大小符合要求或者接受超出限制的文件就输出
                if (sizeList[left] <= Value.setting.LimitSize || Value.setting.AcceptExceedPicture)
                {
                    string result = FileCheck.GetResultFileName(file, ThreadsPool.OutputDir, ThreadsPool.GetPicNum());
                    using(Bitmap output = bitmap.Clone(rect, Encoder.pixelFormats[left]))
                    {
                        BitmapSave.SaveBitmapToFile(output, result, imageFormat);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 基于大小压缩,先缩放再按照位深度区分(仅限ICON)
        /// </summary>
        private static bool CompressionBySize_ScaleAndPixelDeep(string file)
        {
            using (Bitmap bitmap = new Bitmap(new Bitmap(file), Value.setting.IconLimitSize, Value.setting.IconLimitSize))
            {
                BmpProc.SetBrightness(bitmap);
                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                int left = 0, right = Encoder.pixelFormats.Length - 1, mid = 0;
                long[] sizeList = new long[Encoder.pixelFormats.Length];
                while (left < right - 1)
                {
                    mid = (left + right) / 2;
                    sizeList[mid] = GetBitmapSizeByPixelDeep(bitmap, ImageFormat.Png, rect, Encoder.pixelFormats[mid]);
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
                    sizeList[left] = GetBitmapSizeByPixelDeep(bitmap, ImageFormat.Png, rect, Encoder.pixelFormats[left]);
                }
                //如果文件大小符合要求或者接受超出限制的文件就输出
                if (sizeList[left] <= Value.setting.LimitSize || Value.setting.AcceptExceedPicture)
                {
                    string result = FileCheck.GetResultFileName(file, ThreadsPool.OutputDir, ThreadsPool.GetPicNum());
                    using (Bitmap output = bitmap.Clone(rect, Encoder.pixelFormats[left]))
                    {
                        BitmapSave.SaveBitmapToFile(output, result, ImageFormat.Icon);
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
}
