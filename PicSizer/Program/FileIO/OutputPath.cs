using PicSizer.Static;
using PicSizer.Window.Assemble;
using PicSizer.Window.Partial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.FileIO
{
    public static partial class OutputPath
    {
        public static string GetOutputPath(string input, int index)
        {
            //根据不同的输出方式生成输出文件名
            switch (PicSetting.OutputType)
            {
                //覆盖源文件
                case PicUnit.OutputType.CoverOrigin:
                    return input;
                //保留文件夹结构
                case PicUnit.OutputType.OutputStructure:
                    //将公共目录裁去
                    string relativePath = input.Substring(PicValue.PublicDirectory.Length + 1);
                    //组合成最终路径
                    string finalPath = Path.Combine(PicValue.OutputDirection, relativePath);
                    //获取最终路径对应的文件夹,如不存在则创建
                    DirectoryInfo info = new DirectoryInfo(Path.GetDirectoryName(finalPath));
                    if(!info.Exists)
                    {
                        info.Create();
                    }
                    return finalPath;
                //输出到统一文件夹,不保存结构
                case PicUnit.OutputType.OutputDirection:
                    string output = PicSetting.OutputFilename
                        //替换下标
                        .Replace("{index}", index.ToString())
                        //替换文件名
                        .Replace("{name}", Path.GetFileNameWithoutExtension(input))
                        .Replace("{ext}", GetExtension(input));
                    return Path.Combine(PicValue.OutputDirection, output);
                //此代码永远不会执行,仅用于通过编译
                default:
                    return null;
            }
        }

        public static string GetRootDirectory(IList list)
        {
            string root = Path.GetDirectoryName(((PicListViewItem)list[0]).FullPath);
            foreach (PicListViewItem item in list)
            {
                // 当前图片不在目录 root 中，则查找新的公共目录
                if (!item.FullPath.StartsWith(root))
                {
                    int min = Math.Min(item.FullPath.Length, root.Length);
                    int i = 0;
                    while (item.FullPath[i] == root[i] && i < min)
                    {
                        ++i;
                    }
                    if (i == 0)
                    {
                        return "";
                    }
                    else
                    {
                        root = root.Substring(0, i);
                    }
                }
            }
            return root;
        }

        private static string GetExtension(string input)
        {
            int dot = input.LastIndexOf('.');
            return input.Substring(dot + 1);
        }
    }
}
