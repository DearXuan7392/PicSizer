using PicSizer_ControlLibrary;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using PicSizer.Class.Static;

namespace PicSizer.Class.PictureProc
{
    /// <summary>
    /// 线程池
    /// </summary>
    public static class ThreadsPool
    {
        /// <summary>
        /// 图片下标
        /// </summary>
        private static int PicIndex = 0;

        /// <summary>
        /// 图片集合的下标
        /// </summary>
        private static int CollectionIndex = -1;

        /// <summary>
        /// 待压缩文件集合
        /// </summary>
        public static PicListView listView = Value.mainForm.listView1;

        static List<WaitHandle> waitHandles = new List<WaitHandle>();

        public static void StartThreadsPool()
        {
            waitHandles.Clear();
            PicIndex = Value.setting.StartIndex - 1;
            CollectionIndex = -1;
            for (int i = 0; i < Value.setting.maxThreads; i++)
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
            PicSizer_ControlLibrary.Partial.PicListViewItem item;
            while (!Value.ThreadExitNow && (index = GetNextFileIndex()) != -1)
            {
                item = listView[index];
                filename = item.SubItems[1].Text;
                if (Resize.ResizeOnePicture(filename))
                {
                    listView.UpdateStateToSuccess(item);
                }
                else
                {
                    listView.UpdateStateToError(item);
                }
                listView.EnsureVisible(index);
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
            if (CollectionIndex < listView.Items.Count)
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
