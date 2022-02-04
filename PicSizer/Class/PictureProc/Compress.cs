using PicSizer.Partial;
using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PicSizer.Unit;

namespace PicSizer.PictureProc
{
    /// <summary>
    /// 图像裁剪，缩放
    /// </summary>
    public static partial class Compress
    {
        /// <summary>
        /// 调整图片像素
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
                    return bitmap;
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
        /// 计算指定画质下Bitmap输出到流后的大小
        /// </summary>
        private static long GetBitmapSizeByQuality(Bitmap bitmap, ImageCodecInfo info, long value)
        {
            Encoder.encoderParameters.Param[0] = Encoder.GetParameter(value);
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, info, Encoder.encoderParameters);
            long size = memoryStream.Length >> 10;
            memoryStream.Dispose();//立即摧毁MemoryStream防止内存占用过多
            return size;
        }

        /// <summary>
        /// 计算指定缩放比例下Bitmap输出到流后的大小
        /// </summary>
        private static long GetBitmapSizeByScale(Bitmap bitmap, ImageCodecInfo info, int width, int height)
        {
            Encoder.encoderParameters.Param[0] = Encoder.GetParameter(100L);//选定最高画质
            MemoryStream memoryStream = new MemoryStream();
            Bitmap output = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(output);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(bitmap,
                new Rectangle(0, 0, width, height), //画在新Bitmap上的区域
                new Rectangle(0, 0, bitmap.Width, bitmap.Height), //老Bitmap截取的区域
                GraphicsUnit.Pixel);
            g.Dispose();//摧毁
            output.Save(memoryStream, info, Encoder.encoderParameters);//输出到内存
            long size = memoryStream.Length >> 10;
            memoryStream.Dispose();
            output.Dispose();
            return size;
        }

        /// <summary>
        /// 基于画质压缩
        /// </summary>
        public static bool CompressionByValue(string file)
        {
            using (Bitmap bitmap = ResizeBitmap(new Bitmap(file)))
            {
                BmpProc.SetBrightness(bitmap);
                Encoder.encoderParameters.Param[0] = Encoder.GetParameter(Value.setting.CompressionValue);
                string result = FileCheck.GetResultFileName(file, ThreadsPool.OutputDir, ThreadsPool.GetPicNum());
                bitmap.Save(result, FileCheck.GetImageInfoByFilename(file), Encoder.encoderParameters);
                return true;
            }
        }

        /// <summary>
        /// 根据生成的类型自动选择压缩方式
        /// </summary>
        public static bool CompressionBySize(string file)
        {
            ImageCodecInfo image_type = FileCheck.GetImageInfoByFilename(file);
            //JPEG使用画质压缩
            if (image_type == Extension.BitmapExportExtension[".jpg"]){
                return CompressionBySize_QualityFirst(file, image_type);
            }
            //其它格式使用缩放压缩
            else
            {
                return CompressionBySize_ScaleFirst(file, image_type);
            }
        }

        /// <summary>
        /// 基于大小压缩,依照画质区分
        /// </summary>
        private static bool CompressionBySize_QualityFirst(string file,ImageCodecInfo image_type)
        {
            using (Bitmap bitmap = ResizeBitmap(new Bitmap(file)))
            {
                BmpProc.SetBrightness(bitmap);
                long left = 0L, right = 100L, mid = 0L;
                long[] sizeList = new long[101];
                while (left < right - 1)
                {
                    mid = (left + right) / 2;
                    sizeList[mid] = GetBitmapSizeByQuality(bitmap, image_type, mid);
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
                if(sizeList[left] == 0)
                {
                    sizeList[left] = GetBitmapSizeByQuality(bitmap, image_type, left);
                }
                //如果文件大小符合要求就输出
                if (sizeList[left] <= Value.setting.LimitSize)
                {
                    Encoder.encoderParameters.Param[0] = Encoder.GetParameter(left);
                    string result = FileCheck.GetResultFileName(file, ThreadsPool.OutputDir, ThreadsPool.GetPicNum());
                    bitmap.Save(result, image_type, Encoder.encoderParameters);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 基于大小压缩,依照缩放比例区分
        /// </summary>
        private static bool CompressionBySize_ScaleFirst(string file, ImageCodecInfo image_type)
        {
            using (Bitmap bitmap = ResizeBitmap(new Bitmap(file)))
            {
                BmpProc.SetBrightness(bitmap);
                int left = 0, right = 100, mid = 0;
                long[] sizeList = new long[101];
                while (left < right - 1)
                {
                    mid = (left + right) / 2;
                    sizeList[mid] = GetBitmapSizeByScale(bitmap, image_type, bitmap.Width * mid / 100, bitmap.Height * mid / 100);
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
                    sizeList[left] = GetBitmapSizeByScale(bitmap, image_type, bitmap.Width * mid / 100, bitmap.Height * mid / 100);
                }
                //如果文件大小符合要求就输出
                if (sizeList[left] <= Value.setting.LimitSize)
                {
                    using (Bitmap output = ScaleBitmap(bitmap, bitmap.Width * left / 100, bitmap.Height * left / 100))
                    {
                        Encoder.encoderParameters.Param[0] = Encoder.GetParameter(100L);
                        string result = FileCheck.GetResultFileName(file, ThreadsPool.OutputDir, ThreadsPool.GetPicNum());
                        output.Save(result, image_type, Encoder.encoderParameters);
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
