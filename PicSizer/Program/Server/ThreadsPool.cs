#region

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using PicSizer.Program.Deliver;
using PicSizer.Program.Logic.Compress;
using PicSizer.Program.Static;
using PicSizer.Program.Window;
using PicSizer.Program.Window.Assemble;

#endregion

namespace PicSizer.Program.Server
{
    /// <summary>
    /// 线程池
    /// </summary>
    public static partial class ThreadsPool
    {
        /// <summary>
        /// 等待句柄,用于阻碍线程推进
        /// </summary>
        private static readonly List<WaitHandle> WaitHandles = new List<WaitHandle>();

        /// <summary>
        /// 待压缩图片总数
        /// </summary>
        private static int _totalNum;

        /// <summary>
        /// 已压缩图片总数
        /// </summary>
        private static int _currentNum;

        /// <summary>
        /// 出错的图片数量
        /// </summary>
        private static int _errorNum;

        /// <summary>
        /// 开始多线程压缩
        /// </summary>
        public static void StartThreadsPool()
        {
            //初始化数据
            _totalNum = PicValue.PicListView.Items.Count;
            _currentNum = 0;
            //初始化下标
            _indexOfPicListView = 0;
            _indexOfOutput = PicSetting.OutputIndex;
            //初始化等待句柄
            PicValue.ExitNow = false;
            WaitHandles.Clear();
            //创建异步任务
            for (int i = 0; i < PicSetting.MaxThreads; i++)
            {
                //新建等待句柄并加入数组
                ManualResetEvent manual = new ManualResetEvent(false);
                WaitHandles.Add(manual);
                Thread thread = new Thread(() => { DoInThread(manual); });
                thread.Priority = ThreadPriority.Highest;
                thread.Start();
            }

            //等待所有句柄完成
            WaitHandle.WaitAll(WaitHandles.ToArray());
            Task.OnTaskEnd(_currentNum, _errorNum, _totalNum);
        }

        private static void DoInThread(ManualResetEvent manualResetEvent)
        {
            PicListViewItem item;
            string input;
            string output;
            //没有点击退出且后续还有图片
            while (!PicValue.ExitNow && GetNextOutputPictureInfo(out item, out input, out output))
            {
                item.State = PicUnit.PicItemState.Compression;
                PicResult result = CompressItem.Compress(input, output);
                UpdateResult(result.Ok);

                if (result.CompressResult == PicResult.CompressResultEnum.Ok)
                {
                    item.State = PicUnit.PicItemState.Success;
                }
                else if (result.CompressResult == PicResult.CompressResultEnum.OutOfLimit)
                {
                    item.State = PicUnit.PicItemState.OutOfLimit;
                }
                else
                {
                    item.State = PicUnit.PicItemState.Error;
                    item.Message = result.Message;
                }
            }

            manualResetEvent.Set();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void UpdateResult(bool result)
        {
            if (result)
            {
                _currentNum++;
            }
            else
            {
                _errorNum++;
            }

            FormsControl.ProgressForm?.UpdateProgress(_currentNum, _errorNum, _totalNum);
        }
    }
}