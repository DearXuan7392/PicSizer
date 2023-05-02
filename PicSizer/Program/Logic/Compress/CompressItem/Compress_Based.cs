using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Logic
{
    /// <summary>
    /// 压缩接口,需实现按强度压缩和按文件大小压缩
    /// </summary>
    public interface Compress_Based
    {
        /// <summary>
        /// 将图片压缩到指定的大小之内
        /// </summary>
        /// <param name="bitmap">图像对象</param>
        /// <param name="limit">限制的大小，单位为 KB</param>
        /// <param name="output">输出路径</param>
        void Compress_By_FileSize(long limit, string output);

        /// <summary>
        /// 以指定压缩强度输出图片
        /// </summary>
        /// <param name="bitmap">图像对象</param>
        /// <param name="strength">压缩强度</param>
        /// <param name="output">输出路径</param>
        void Compress_By_Strength(int strength, string output);
    }
}
