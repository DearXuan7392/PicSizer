using PicSizer.Deliver;
using PicSizer.Logic;
using PicSizer.Static;
using PicSizer.Window;
using PicSizer.Window.Assemble;
using PicSizer.Window.Partial;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PicSizer.Server
{
    /// <summary>
    /// 线程池
    /// </summary>
    public static partial class ThreadsPool
    {
        /// <summary>
        /// 等待句柄,用于阻碍线程推进
        /// </summary>
        private static List<WaitHandle> waitHandles = new List<WaitHandle>();

        /// <summary>
        /// 待压缩图片总数
        /// </summary>
        private static int totalNum = 0;

        /// <summary>
        /// 已压缩图片总数
        /// </summary>
        private static int currentNum = 0;

        /// <summary>
        /// 出错的图片数量
        /// </summary>
        private static int errorNum = 0;

        /// <summary>
        /// 开始多线程压缩
        /// </summary>
        public static void StartThreadsPool()
        {
            //初始化数据
            totalNum = PicValue.picListView.Items.Count;
            currentNum = 0;
            //初始化下标
            Index_Of_PicListView = 0;
            Index_Of_Output = PicSetting.OutputIndex;
            //初始化等待句柄
            PicValue.ExitNow = false;
            waitHandles.Clear();
            //创建异步任务
            for (int i = 0; i < PicSetting.MaxThreads; i++)
            {
                //新建等待句柄并加入数组
                ManualResetEvent manual = new ManualResetEvent(false);
                waitHandles.Add(manual);
                Thread thread = new Thread(() =>
                {
                    DoInThread(manual);
                });
                thread.Priority = ThreadPriority.Highest;
                thread.Start();
            }
            //等待所有句柄完成
            WaitHandle.WaitAll(waitHandles.ToArray());
            Task.OnTaskEnd(currentNum, errorNum, totalNum);
        }

        private static void DoInThread(ManualResetEvent manualResetEvent)
        {
            PicListViewItem item;
            string input;
            string output;
            //没有点击退出且后续还有图片
            while (!PicValue.ExitNow && GetNextOutputPictureInfo(out item, out input, out output))
            {
                Bitmap bitmap = null;
                bool result = false;
                item.State = PicUnit.PicItemState.Compression; 
                try
                {
                    //获取经过预处理之后的图片
                    bitmap = Prefix.GetBitmap(input);
                    //对图片进行压缩,并返回结果
                    Logic.Compress.CompressPicture(bitmap, output);
                    result = true;
                }
                catch(Exception e)
                {
                    result = false;
                    Dialog.ShowDialog_Exception(e);
                }
                finally
                {
                    bitmap?.Dispose();
                }
                UpdateResult(result);
                item.State = result
                    ? PicUnit.PicItemState.Success
                    : PicUnit.PicItemState.Error;
            }
            manualResetEvent.Set();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void UpdateResult(bool result)
        {
            if(result)
            {
                currentNum++;
            }
            else
            {
                errorNum++;
            }
            FormsControl.ProgressForm?.UpdateProgress(currentNum, errorNum, totalNum);
        }
    }
}
