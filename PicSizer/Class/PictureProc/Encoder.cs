using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer
{
    public static class Encoder
    {
        public static System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
        public static EncoderParameters encoderParameters = new EncoderParameters(1);
        public static EncoderParameter[] parameterList = new EncoderParameter[101];

        /// <summary>
        /// 获取编码信息
        /// </summary>
        public static EncoderParameter GetParameter(long value)
        {
            int v = (int)value;
            if (parameterList[v] == null)
            {
                parameterList[v] = new EncoderParameter(encoder, value);
            }
            return parameterList[v];
        }

        /// <summary>
        /// 获取编码信息
        /// </summary>
        public static ImageCodecInfo GetEncoderInfo(string type)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == type)
                {
                    return encoders[j];
                }
            }
            return null;
        }
    }
}
