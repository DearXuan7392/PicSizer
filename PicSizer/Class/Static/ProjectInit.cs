using PicSizer.Class.Partial;
using System.Drawing.Imaging;
using PicSizer.Class.Unit;

namespace PicSizer.Class.Static
{
    public static class ProjectInit
    {
        /// <summary>
        /// 程序运行时的初始化任务
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
        }
    }
}
