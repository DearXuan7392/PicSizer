using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSizer.PictureProc
{
    public static class ThreadsPool
    {
        /// <summary>
        /// 图片下标
        /// </summary>
        private static int PicIndex = 0;

        /// <summary>
        /// 输出目录
        /// </summary>
        public static string OutputDir;

        /// <summary>
        /// 图片集合的下标
        /// </summary>
        private static int CollectionIndex = -1;

        /// <summary>
        /// 待压缩文件集合
        /// </summary>
        public static ListBox.ObjectCollection FileCollection;

        static List<WaitHandle> waitHandles = new List<WaitHandle>();

        public static void StartThreadsPool()
        {
            waitHandles.Clear();
            PicIndex = SharedVariable.setting.StartIndex - 1;
            CollectionIndex = -1;
            for(int i = 0; i < SharedVariable.setting.maxThreads; i++)
            {
                ManualResetEvent manual = new ManualResetEvent(false);
                waitHandles.Add(manual);
                Thread thread = new Thread(() =>
                {
                    DoInThread(manual);
                })
                {
                    Priority = ThreadPriority.Normal
                };
                thread.Start();
            }
            WaitHandle.WaitAll(waitHandles.ToArray());
            Resize.OnExit();
        }

        public static void DoInThread(ManualResetEvent manualResetEvent)
        {
            int index;
            string filename;
            while(!SharedVariable.ThreadExitNow && (index = GetNextFileIndex()) != -1)
            {
                filename = (string)FileCollection[index];
                Resize.ResizeOnePicture(filename);
            }
            manualResetEvent.Set();
            return;
        }

        /// <summary>
        /// 获取当前图片的下标，同时仅限一个线程使用
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int GetPicNum()
        {
            PicIndex += 1;
            return PicIndex;
        }

        /// <summary>
        /// 获取下一个图片在集合里的下标
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int GetNextFileIndex()
        {
            CollectionIndex += 1;
            if (CollectionIndex < FileCollection.Count)
            {
                return CollectionIndex;
            }
            else
            {
                return -1;
            }
        }
    }
}
