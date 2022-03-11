using System;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using PicSizer.Class.Partial;
using PicSizer.Class.Static;

namespace PicSizer.Class.PictureProc
{
    public static class Resize
    {
        /// <summary>
        /// 开始压缩
        /// </summary>
        public static void StartResizer(string resDir)
        {
            Value.OutputDir = resDir;
            ThreadsPool.StartThreadsPool();
        }

        public static bool ResizeOnePicture(string path)
        {
            AtomPic atomPic = null;
            try
            {
                atomPic = new AtomPic(path);
                if(!Compress.CompressAtomPic(atomPic)) throw new Exception("图片\"" + path + "\"压缩后仍较大");
                Update(true); // 压缩成功，进度条加一
                return true;
            }
            catch (Exception e)
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
            finally
            {
                atomPic?.Dispose();
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
