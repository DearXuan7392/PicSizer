#region

using System.Collections.Generic;
using System.Text;

#endregion

namespace PicSizer.Program.FileIO
{
    public static class Extension
    {
        /// <summary>
        /// 后缀名列表
        /// </summary>
        private static readonly ExtensionTypeList[] ExtensionTypeLists = {
            new ExtensionTypeList
            {
                Name = "位图",
                List = new[]
                {
                    ".jpg", ".jpeg", ".png", ".bmp", ".tif", ".tiff", ".pcx", ".ico"
                }
            },
            new ExtensionTypeList
            {
                Name = "矢量图",
                List = new[]
                {
                    ".dxf", ".cgm", ".cdr", ".wmf", ".eps", ".emf"
                }
            }
        };

        private class ExtensionTypeList
        {
            public string Name;
            public string[] List;
        }

        /// <summary>
        /// 将后缀名列表转化成文件选择窗口的Filter
        /// </summary>
        public static string ExtensionTypeListToFilter()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (ExtensionTypeList typeList in ExtensionTypeLists)
            {
                stringBuilder
                    .Append("|")
                    .Append(typeList.Name)
                    .Append("|");
                foreach (string extension in typeList.List)
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