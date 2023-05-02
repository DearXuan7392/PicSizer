using PicSizer.FileIO;
using PicSizer.Static;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PicSizer.Window.Partial;

namespace PicSizer.Logic
{
    public partial class WEBP
    {
        /// <summary>
        /// 以指定压缩强度输出WEBP图片
        /// </summary>
        /// <param name="quality">画质</param>
        /// <param name="output">输出路径</param>
        public void Compress_By_Strength(int quality, string output)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(output, FileMode.Create, FileAccess.ReadWrite);
                long length = BitmapToStream(bitmap, fs, quality);
                fs.Close();
            }
            finally
            {
                fs?.Dispose();
            }
        }
    }
}
