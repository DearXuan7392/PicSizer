using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;

namespace PicSizer.Unit
{
    public static class Extension
    {
        /// <summary>
        /// Bitmap支持的格式的集合(小写,包括点号)
        /// </summary>
        public static HashSet<string> BitmapSupportExtension = new HashSet<string>();

        /// <summary>
        /// 后缀名列表
        /// </summary>
        public static ExtensionTypeList[] extensionTypeLists = new ExtensionTypeList[]
        {
            new ExtensionTypeList()
            {
                name = "位图",
                list = new string[]
                {
                    ".jpg",".jpeg",".png",".bmp",".tif",".tiff",".pcx",".ico"
                }
            },
            new ExtensionTypeList()
            {
                name = "矢量图",
                list = new string[]
                {
                    ".dxf",".cgm",".cdr",".wmf",".eps",".emf"
                }
            }
        };

        public class ExtensionTypeList
        {
            public string name;
            public string[] list;
        }

        /// <summary>
        /// 将后缀名列表转化成文件选择窗口的Filter
        /// </summary>
        public static string ExtensionTypeListToFilter()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (ExtensionTypeList typeList in extensionTypeLists)
            {
                stringBuilder
                    .Append("|")
                    .Append(typeList.name)
                    .Append("|");
                foreach (string extension in typeList.list)
                {
                    stringBuilder
                        .Append("*")
                        .Append(extension)
                        .Append(";");
                }
            }
            return stringBuilder.ToString();
        }
    }
}
