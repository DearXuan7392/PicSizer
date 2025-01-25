#region

using System.Threading;
using PicSizer.Program.FileIO;
using PicSizer.Program.Server;
using PicSizer.Program.Static;
using PicSizer.Program.Window;
using PicSizer.Program.Window.Partial;

#endregion

namespace PicSizer.Program.Deliver
{
    public static class Task
    {
        /// <summary>
        /// 点击开始压缩按钮
        /// </summary>
        public static void StartCompressTask()
        {
            // 检查有没有图片
            if (PicValue.PicListView.Items.Count == 0)
            {
                Dialog.ShowDialog_Warning("没有待压缩的图片.");
                return;
            }

            // 如果指定输出目录,则判断目录是否合法
            if (PicSetting.OutputType != PicUnit.OutputType.CoverOrigin
                && string.IsNullOrWhiteSpace(PicValue.OutputDirection))
            {
                Dialog.ShowDialog_Warning("输出目录有误.");
                return;
            }

            // 如果保留目录结构，则查找公共目录
            if (PicSetting.OutputType == PicUnit.OutputType.OutputStructure)
            {
                PicValue.PublicDirectory = OutputPath.GetRootDirectory(PicValue.PicListView.Items);
            }

            //开始线程池
            (new Thread(ThreadsPool.StartThreadsPool)
            {
                Priority = ThreadPriority.Lowest
            }).Start();

            //显示进度条窗体
            FormsControl.ShowProgressForm(PicValue.PicListView.Items.Count);
        }

        /// <summary>
        /// 任务结束
        /// </summary>
        public static void OnTaskEnd(int success, int error, int total)
        {
            FormsControl.ProgressForm.Close();
            Dialog.ShowDialog_ResizeFinish(total, success);
        }
    }
}