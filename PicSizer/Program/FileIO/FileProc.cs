using PicSizer.Window.Unit;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using PicSizer.Window.Partial;

namespace PicSizer.FileIO
{
    public static class FileProc
    {
        /// <summary>
        /// 获取文件后缀名(包括点号,小写字母)
        /// </summary>
        public static string GetExtension(string path)
        {
            //查找点号位置
            int i = path.LastIndexOf('.');
            //没有后缀名，直接返回非法
            if (i == -1) return string.Empty;
            //截取后缀名
            return path.Substring(i).ToLower();
        }

        /// <summary>
        /// 把文件大小转换成字符串(单位:KB)
        /// </summary>
        public static string FileSizeToString(long size)
        {
            if (size < 1024)
            {
                return size.ToString() + " KB";
            }
            else
            {
                return (size / 1024.0).ToString("#0.00") + " MB";
            }
        }
    }
}
