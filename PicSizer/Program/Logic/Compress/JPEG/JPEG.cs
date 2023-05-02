using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Logic
{
    public partial class JPEG : CompressTaskItem, Compress_Based, ITaskBased
    {
        public JPEG(Bitmap bitmap) : base(bitmap) { }
    }
}
