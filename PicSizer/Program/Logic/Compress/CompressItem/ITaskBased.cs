using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Logic
{
    public interface ITaskBased
    {
        /// <summary>
        /// 判断是否含有透明像素
        /// </summary>
        /// <returns></returns>
        bool HasTransparentPixel();
    }
}
