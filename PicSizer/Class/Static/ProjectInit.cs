using PicSizer.Class.Partial;
using System.Drawing.Imaging;
using System.Threading;
using PicSizer.Class.Unit;

namespace PicSizer.Class.Static
{
    public static class ProjectInit
    {
        /// <summary>
        /// 程序运行之前的初始化任务
        /// </summary>
        public static void Init()
        {
            //载入dll
            DllExtern.DoInFirst();

            //添加扩展名
            foreach (Extension.ExtensionTypeList typeList in Extension.extensionTypeLists)
            {
                foreach (string extension in typeList.list)
                {
                    Extension.BitmapSupportExtension.Add(extension);
                }
            }
            
            //检查更新信息
            Thread thread = new Thread(() =>
            {
                Update.GetLatestVersions();
            })
            {
                Priority = ThreadPriority.Normal
            };
            thread.Start();
        }

        /// <summary>
        /// 程序运行之后的初始化
        /// </summary>
        public static void After()
        {
            //设置置顶
            Forms.FormsSupport.SetTopMost(Value.setting.topMost);
        }
    }
}
