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
                    if (!Compress.CompressionBySize(path)) throw new Exception("图片:" + path + "压缩失败");
                }
                else
                {
                    if (!Compress.CompressionByValue(path)) throw new Exception("图片:" + path + "压缩失败");
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
