using PicSizer.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Logic
{
    public partial class PNG : CompressTaskItem, Compress_Based, ITaskBased
    {
        public PNG(Bitmap bitmap) : base(bitmap) { }
    }
}
