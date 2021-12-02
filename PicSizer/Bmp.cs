using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSizer
{
    public static class Bmp
    {
        static ImageCodecInfo imageCodecInfo = ImageInfo.Info_JPEG;
        static System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
        static EncoderParameters encoderParameters = new EncoderParameters(1);

        static EncoderParameter[] parameterList = new EncoderParameter[101];

        public static EncoderParameter GetParameter(long value)
        {
            int v = (int)value;
            if(parameterList[v] == null)
            {
                parameterList[v] = new EncoderParameter(encoder, value);
            }
            return parameterList[v];
        }

        /// <summary>
        /// 调整图片像素
        /// </summary>
        public static Bitmap Resize(Bitmap bitmap)
        {
            if (Setting.resizeMode == ResizeMode.None) return bitmap;
            int width = bitmap.Width;
            int height = bitmap.Height;
            //求出比值
            float widthByMin = (float)width / Setting.LimitWidth;
            float heightByMin = (float)height / Setting.LimitHeight;
            //重新设定边长
            if (Setting.resizeMode == ResizeMode.MinSize)
            {
                float min = Math.Min(widthByMin, heightByMin);
                if(min > 1)
                {
                    width = (int)(width / min);
                    height = (int)(height / min);
                }
            }
            else
            {
                float max = Math.Max(widthByMin, heightByMin);
                if(max > 1)
                {
                    width = (int)(width / max);
                    height = (int)(height / max);
                }
            }
            Bitmap newBitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(newBitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.DrawImage(bitmap, new Rectangle(0, 0, width, height), new Rectangle(0, 0, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);
            g.Dispose();
            bitmap.Dispose();
            return newBitmap;
        }

        public static void StartResizer(List<string> files, string resDir)
        {
            int num = Setting.StartIndex;
            int now = 1;
            string result;
            ExtensionMode format = Setting.extensionMode;
            if (Setting.compressionMode == CompressionMode.SizeFirst)
            {
                foreach (string path in files)
                {
                    if (Setting.ThreadExitNow)
                    {
                        OnExit();
                        return;
                    }
                    result = GetResultFileName(path, resDir, num);
                    if(CompressionBySize(path, result))
                    {
                        num++;
                    }
                    Update(now);
                    now++;
                }
            }
            else
            {
                foreach (string path in files)
                {
                    if (Setting.ThreadExitNow)
                    {
                        OnExit();
                        return;
                    }
                    result = GetResultFileName(path, resDir, num);
                    if(CompressionByValue(path, result))
                    {
                        num++;
                    }
                    Update(now);
                    now++;
                }
            }
            
        }

        /// <summary>
        /// 计算Bitmap输出到流后的大小
        /// </summary>
        public static long GetBitmapSize(Bitmap bitmap, long value)
        {
            encoderParameters.Param[0] = GetParameter(value);
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, imageCodecInfo, encoderParameters);
            long size = memoryStream.Length;
            memoryStream.Dispose();//立即摧毁MemoryStream防止内存占用过多
            return size >> 10;
        }

        public static bool CompressionBySize(string file, string result)
        {
            try
            {
                using (Bitmap bitmap = Resize(new Bitmap(file)))
                {
                    long left = 0L;
                    long right = 100L;
                    long mid = 0L;
                    long size = 0L;
                    while(left < right - 1)
                    {
                        mid = (left + right) / 2;
                        size = GetBitmapSize(bitmap, mid);
                        if(size <= Setting.LimitSize)
                        {
                            left = mid;
                        }
                        else
                        {
                            right = mid;
                        }
                    }
                    size = GetBitmapSize(bitmap, left);
                    if(size <= Setting.LimitSize)
                    {
                        encoderParameters.Param[0] = GetParameter(left);
                        bitmap.Save(result, imageCodecInfo, encoderParameters);
                        return true;
                    }
                    return false;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        public static bool CompressionByValue(string file, string result)
        {
            try
            {
                using(Bitmap bitmap = Resize(new Bitmap(file)))
                {
                    encoderParameters.Param[0] = GetParameter(Setting.CompressionValue);
                    bitmap.Save(result, imageCodecInfo, encoderParameters);
                    return true;
                }
            }
            catch(Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 从给定的源文件，生成路径，序号获取文件名
        /// </summary>
        public static string GetResultFileName(string ori, string dir, int num)
        {
            string extension;
            if (Setting.extensionMode == ExtensionMode.Original)
            {
                extension = Path.GetExtension(ori);
            }
            else
            {
                extension = Setting.extensionMode.ToFormat();
            }
            switch (Setting.renameMode)
            {
                case RenameMode.Number://纯数字
                    return Path.Combine(dir, num + extension);
                case RenameMode.Original://原名
                    return Path.Combine(dir, Path.GetFileNameWithoutExtension(ori) + extension);
                case RenameMode.Custom://混合命名
                    return Path.Combine(dir, num + extension);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 立即退出线程
        /// </summary>
        public static void OnExit()
        {
            ProgressForm.form.PrepareToHide();
        }

        /// <summary>
        /// 更新进度条
        /// </summary>
        public static void Update(int now)
        {
            Form1.progressForm.SetNow(now);
        }
    }
}
