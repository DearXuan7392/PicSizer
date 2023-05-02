using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Logic
{
    public abstract class CompressTaskItem : IDisposable, ITaskBased
    {
        /// <summary>
        /// 图片对象
        /// </summary>
        internal Bitmap bitmap;

        /// <summary>
        /// 是否含有Alpha通道
        /// </summary>
        internal bool HasAlphaChannel = false;

        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="_bitmap"></param>
        public CompressTaskItem(Bitmap _bitmap)
        {
            this.bitmap = _bitmap;
        }

        /// <summary>
        /// 判断是否含有透明像素
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool HasTransparentPixel()
        {
            return HasAlphaChannel;
        }

        public void Dispose()
        {
            this.bitmap.Dispose();
        }

        ~ CompressTaskItem()
        {
            this.Dispose();
        }
    }
}
