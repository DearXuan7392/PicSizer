using PicSizer.Partial;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSizer.PictureProc
{
    public static class Resize
    {

        /// <summary>
        /// 调整图片像素
        /// </summary>
        public static Bitmap ResizeBitmap(Bitmap bitmap)
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
        /// 开始压缩
        /// </summary>
        public static void StartResizer(string resDir)
        {
            FileCheck.SetImageCodeInfo(Value.setting.extensionMode);
            ThreadsPool.OutputDir = resDir;
            ThreadsPool.StartThreadsPool();
        }

        public static bool ResizeOnePicture(string path)
        {
            try
            {
                if (Value.setting.compressionMode == CompressionMode.SizeFirst)
                {
                    if (!CompressionBySize(path)) throw new Exception("图片:" + path + "压缩失败");
                }
                else
                {
                    if (!CompressionByValue(path)) throw new Exception("图片:" + path + "压缩失败");
                }
                Update(true); // 压缩成功，进度条加一
                return true;
            }
            catch(Exception e)
            {
                Update(false); // 压缩失败，错误加一
                switch (Value.setting.doWhenException)
                {
                    case DoWhenException.IgnoreAndContinue:
                        break;
                    case DoWhenException.IgnoreAndJump:
                        ThreadsPool.GetPicNum();
                        break;
                    case DoWhenException.ShowAndContinue:
                        Dialog.ShowDialog_Exception(e);
                        break;
                    case DoWhenException.ShowAndJump:
                        Dialog.ShowDialog_Exception(e);
                        ThreadsPool.GetPicNum();
                        break;
                    default:
                        Dialog.ShowDialog_Exception(e);
                        Value.ThreadExitNow = true;
                        break;
                }
                return false;
            }
        }

        /// <summary>
        /// 计算Bitmap输出到流后的大小
        /// </summary>
        public static long GetBitmapSize(Bitmap bitmap, ImageCodecInfo info, long value)
        {
            Encoder.encoderParameters.Param[0] = Encoder.GetParameter(value);
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, info, Encoder.encoderParameters);
            long size = memoryStream.Length;
            memoryStream.Dispose();//立即摧毁MemoryStream防止内存占用过多
            return size >> 10;
        }

        /// <summary>
        /// 基于大小压缩
        /// </summary>
        public static bool CompressionBySize(string file)
        {
            using (Bitmap bitmap = ResizeBitmap(new Bitmap(file)))
            {
                BmpProc.SetBrightness(bitmap);
                ImageCodecInfo image_type = FileCheck.GetImageInfoByFilename(file);
                long left = 0L;
                long right = 100L;
                long mid = 0L;
                long size = 0L;
                while(left < right - 1)
                {
                    mid = (left + right) / 2;
                    size = GetBitmapSize(bitmap, image_type, mid);
                    if(size <= Value.setting.LimitSize)
                    {
                        left = mid;
                    }
                    else
                    {
                        right = mid;
                    }
                }
                size = GetBitmapSize(bitmap, image_type, left);
                //如果文件大小符合要求就输出
                if(size <= Value.setting.LimitSize)
                {
                    Encoder.encoderParameters.Param[0] = Encoder.GetParameter(left);
                    string result = GetResultFileName(file, ThreadsPool.OutputDir, ThreadsPool.GetPicNum());
                    bitmap.Save(result, image_type, Encoder.encoderParameters);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 基于画质压缩
        /// </summary>
        public static bool CompressionByValue(string file)
        {
            using(Bitmap bitmap = ResizeBitmap(new Bitmap(file)))
            {
                BmpProc.SetBrightness(bitmap);
                Encoder.encoderParameters.Param[0] = Encoder.GetParameter(Value.setting.CompressionValue);
                string result = GetResultFileName(file, ThreadsPool.OutputDir, ThreadsPool.GetPicNum());
                bitmap.Save(result, FileCheck.GetImageInfoByFilename(file), Encoder.encoderParameters);
                return true;
            }
        }

        /// <summary>
        /// 从给定的源文件，生成路径，序号获取文件名
        /// </summary>
        public static string GetResultFileName(string ori, string dir, int num)
        {
            //如果选择“覆盖源文件”，则直接返回源文件路径
            if (Value.CoverOriginalFile)
            {
                return ori;
            }
            //求出后缀名
            string extension;
            if (Value.setting.extensionMode == ExtensionMode.Original)
            {
                //原格式
                extension = Path.GetExtension(ori);
            }
            else
            {
                //自定义格式
                extension = Value.setting.extensionMode.ToFormat();
            }
            switch (Value.setting.renameMode)
            {
                case RenameMode.Number://纯数字
                    return Path.Combine(dir, num + extension);
                case RenameMode.Original://原名
                    return Path.Combine(dir, Path.GetFileNameWithoutExtension(ori) + extension);
                case RenameMode.Custom://混合命名
                    string oriStr = Path.GetFileNameWithoutExtension(ori);//文件原名
                    string numStr = num.ToString();//序号
                    return Path.Combine(dir, Value.setting.CustomRenameStr.Replace("{ori}", oriStr).Replace("{num}", numStr) + extension);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 立即退出线程
        /// </summary>
        public static void OnExit()
        {
            Value.progressForm.PrepareToHide();
        }

        /// <summary>
        /// 更新进度条
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Update(bool flag)
        {
            Value.progressForm.AddOne(flag);
        }
    }
}
