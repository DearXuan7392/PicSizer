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
    public static class Resize
    {
        static ImageCodecInfo imageCodecInfo = ImageInfo.Info_JPEG;
        static System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
        static EncoderParameters encoderParameters = new EncoderParameters(1);

        static EncoderParameter[] parameterList = new EncoderParameter[101];

        /// <summary>
        /// 获取编码信息
        /// </summary>
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
        public static Bitmap ResizeBitmap(Bitmap bitmap)
        {
            if (Setting.resizeMode == ResizeMode.None) return bitmap;
            int width = bitmap.Width;
            int height = bitmap.Height;
            //求出比值
            float widthByMin = (float)width / Setting.LimitWidth;
            float heightByMin = (float)height / Setting.LimitHeight;
            //重新设定边长
            if (Setting.resizeMode == ResizeMode.MinSize)//不小于限定值
            {
                float min = Math.Min(widthByMin, heightByMin);
                if(min > 1)
                {
                    width = (int)(width / min);
                    height = (int)(height / min);
                }
            }
            else if(Setting.resizeMode == ResizeMode.MaxSize)//不大于限定值
            {
                float max = Math.Max(widthByMin, heightByMin);
                if(max > 1)
                {
                    width = (int)(width / max);
                    height = (int)(height / max);
                }
            }
            else//强制修正
            {
                width = Setting.LimitWidth;
                height = Setting.LimitHeight;
            }
            //裁剪
            Bitmap newBitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(newBitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.DrawImage(bitmap, new Rectangle(0, 0, width, height), new Rectangle(0, 0, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);
            g.Dispose();//摧毁
            bitmap.Dispose();//摧毁
            return newBitmap;
        }

        

        public static void StartResizer(ListBox.ObjectCollection files, string resDir)
        {
            int num = Setting.StartIndex;
            string result;
            //遍历所有图片
            foreach (string path in files)
            {
                try
                {
                    if (Setting.ThreadExitNow)
                    {
                        OnExit();
                        return;
                    }
                    result = GetResultFileName(path, resDir, num);
                    if (Setting.compressionMode == CompressionMode.SizeFirst)
                    {
                        if (!CompressionBySize(path, result)) throw new Exception("图片:" + path + "压缩失败");
                    }
                    else
                    {
                        if (!CompressionByValue(path, result)) throw new Exception("图片:" + path + "压缩失败");
                    }
                    num++;
                    Update(true); // 压缩成功，进度条加一
                }
                catch(Exception ex)
                {
                    Update(false); // 压缩失败，错误加一
                    switch (Setting.doWhenException)
                    {
                        case DoWhenException.IgnoreAndContinue:
                            break;
                        case DoWhenException.IgnoreAndJump:
                            num++;
                            break;
                        default:
                            Dialog.ShowDialog_Exception(ex);
                            OnExit();
                            return;
                    }
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

        /// <summary>
        /// 基于大小压缩
        /// </summary>
        public static bool CompressionBySize(string file, string result)
        {
            using (Bitmap bitmap = ResizeBitmap(new Bitmap(file)))
            {
                BmpProc.SetBrightness(bitmap);
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

        /// <summary>
        /// 基于画质压缩
        /// </summary>
        public static bool CompressionByValue(string file, string result)
        {
            using(Bitmap bitmap = ResizeBitmap(new Bitmap(file)))
            {
                BmpProc.SetBrightness(bitmap);
                encoderParameters.Param[0] = GetParameter(Setting.CompressionValue);
                bitmap.Save(result, imageCodecInfo, encoderParameters);
                return true;
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
                    return Path.Combine(dir, string.Format(Setting.CustomRenameStr,num) + extension);
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
        public static void Update(bool flag)
        {
            Form1.progressForm.AddOne(flag);
        }
    }
}
