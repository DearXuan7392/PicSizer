using PicSizer.Static;
using PicSizer.Window;
using PicSizer.Window.Assemble;
using PicSizer.Window.Partial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PicSizer.Deliver
{
    public static class Task
    {
        /// <summary>
        /// 点击开始压缩按钮
        /// </summary>
        public static void StartCompressTask()
        {
            // 检查有没有图片
            if(PicValue.picListView.Items.Count == 0)
            {
                
                Dialog.ShowDialog_Warning("没有待压缩的图片.");
                return;
            }

            // 如果指定输出目录,则判断目录是否合法
            if(PicSetting.OutputType != PicUnit.OutputType.CoverOrigin 
                && string.IsNullOrWhiteSpace(PicValue.OutputDirection))
            {
                Dialog.ShowDialog_Warning("输出目录有误.");
                return;
            }

            // 如果保留目录结构，则查找公共目录
            if(PicSetting.OutputType == PicUnit.OutputType.OutputStructure)
            {
                PicValue.PublicDirectory = FileIO.OutputPath.GetRootDirectory(PicValue.picListView.Items);
            }

            //开始线程池
            (new Thread(Server.ThreadsPool.StartThreadsPool)
            {
                Priority= ThreadPriority.Lowest
            }).Start();

            //显示进度条窗体
            FormsControl.ShowProgressForm(PicValue.picListView.Items.Count);
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
