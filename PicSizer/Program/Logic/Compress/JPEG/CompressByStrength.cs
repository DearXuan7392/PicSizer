using PicSizer.Static;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Logic
{
    public partial class JPEG
    {
        /// <summary>
        /// 以指定压缩强度输出JPEG图片
        /// </summary>
        /// <param name="strength">压缩强度</param>
        /// <param name="output">输出路径</param>
        public void Compress_By_Strength(int strength, string output)
        {
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = Encoder.GetParameter(strength);
            FileStream fs = null;
            try
            {
                fs = new FileStream(output, FileMode.Create, FileAccess.Write);
                bitmap.Save(fs, Encoder._Info_JPEG, encoderParameters);
                fs.Close();
            }
            finally
            {
                fs?.Dispose();
            }
            
        }
    }
}
