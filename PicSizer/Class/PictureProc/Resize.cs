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
            if (SharedVariable.setting.resizeMode == ResizeMode.None) return bitmap;
            int width = bitmap.Width;
            int height = bitmap.Height;
            //求出比值
            float widthByMin = (float)width / SharedVariable.setting.LimitWidth;
            float heightByMin = (float)height / SharedVariable.setting.LimitHeight;
            //重新设定边长
            if (SharedVariable.setting.resizeMode == ResizeMode.MinSize)//不小于限定值
            {
                float min = Math.Min(widthByMin, heightByMin);
                if(min > 1)
                {
                    width = (int)(width / min);
                    height = (int)(height / min);
                }
            }
            else if(SharedVariable.setting.resizeMode == ResizeMode.MaxSize)//不大于限定值
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
                width = SharedVariable.setting.LimitWidth;
                height = SharedVariable.setting.LimitHeight;
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

        /// <summary>
        /// 开始压缩
        /// </summary>
        public static void StartResizer(ListBox.ObjectCollection collection, string resDir)
        {
            ThreadsPool.OutputDir = resDir;
            ThreadsPool.FileCollection = collection;

            ThreadsPool.StartThreadsPool();
        }

        public static void ResizeOnePicture(string path)
        {
            try
            {
                if (SharedVariable.setting.compressionMode == CompressionMode.SizeFirst)
                {
                    if (!CompressionBySize(path)) throw new Exception("图片:" + path + "压缩失败");
                }
                else
                {
                    if (!CompressionByValue(path)) throw new Exception("图片:" + path + "压缩失败");
                }
                Update(true); // 压缩成功，进度条加一
            }
            catch(Exception e)
            {
                Update(false); // 压缩失败，错误加一
                switch (SharedVariable.setting.doWhenException)
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
                        SharedVariable.ThreadExitNow = true;
                        return;
                }
            }
        }

        /// <summary>
        /// 计算Bitmap输出到流后的大小
        /// </summary>
        public static long GetBitmapSize(Bitmap bitmap, long value)
        {
            Encoder.encoderParameters.Param[0] = Encoder.GetParameter(value);
            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, Encoder.imageCodecInfo, Encoder.encoderParameters);
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
                long left = 0L;
                long right = 100L;
                long mid = 0L;
                long size = 0L;
                while(left < right - 1)
                {
                    mid = (left + right) / 2;
                    size = GetBitmapSize(bitmap, mid);
                    if(size <= SharedVariable.setting.LimitSize)
                    {
                        left = mid;
                    }
                    else
                    {
                        right = mid;
                    }
                }
                size = GetBitmapSize(bitmap, left);
                //如果文件大小符合要求就输出
                if(size <= SharedVariable.setting.LimitSize)
                {
                    Encoder.encoderParameters.Param[0] = Encoder.GetParameter(left);
                    string result = GetResultFileName(file, ThreadsPool.OutputDir, ThreadsPool.GetPicNum());
                    bitmap.Save(result, Encoder.imageCodecInfo, Encoder.encoderParameters);
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
                Encoder.encoderParameters.Param[0] = Encoder.GetParameter(SharedVariable.setting.CompressionValue);
                string result = GetResultFileName(file, ThreadsPool.OutputDir, ThreadsPool.GetPicNum());
                bitmap.Save(result, Encoder.imageCodecInfo, Encoder.encoderParameters);
                return true;
            }
        }

        /// <summary>
        /// 从给定的源文件，生成路径，序号获取文件名
        /// </summary>
        public static string GetResultFileName(string ori, string dir, int num)
        {
            string extension;
            if (SharedVariable.setting.extensionMode == ExtensionMode.Original)
            {
                extension = Path.GetExtension(ori);
            }
            else
            {
                extension = SharedVariable.setting.extensionMode.ToFormat();
            }
            switch (SharedVariable.setting.renameMode)
            {
                case RenameMode.Number://纯数字
                    return Path.Combine(dir, num + extension);
                case RenameMode.Original://原名
                    return Path.Combine(dir, Path.GetFileNameWithoutExtension(ori) + extension);
                case RenameMode.Custom://混合命名
                    string oriStr = Path.GetFileNameWithoutExtension(ori);//文件原名
                    string numStr = num.ToString();//序号
                    return Path.Combine(dir, SharedVariable.setting.CustomRenameStr.Replace("{ori}",oriStr).Replace("{num}",numStr) + extension);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 立即退出线程
        /// </summary>
        public static void OnExit()
        {
            SharedVariable.progressForm.PrepareToHide();
        }

        /// <summary>
        /// 更新进度条
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Update(bool flag)
        {
            SharedVariable.progressForm.AddOne(flag);
        }
    }
}
