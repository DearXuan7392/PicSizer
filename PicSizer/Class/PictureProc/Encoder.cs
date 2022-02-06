using System.Drawing.Imaging;

namespace PicSizer.Class.PictureProc
{
    public static class Encoder
    {
        public static ImageCodecInfo _Info_JPEG = Encoder.GetEncoderInfo("image/jpeg");

        public static System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
        public static EncoderParameter[] parameterList = new EncoderParameter[101];

        /// <summary>
        /// 像素位数数组
        /// </summary>
        public static PixelFormat[] pixelFormats = new PixelFormat[]
        {
            PixelFormat.Format8bppIndexed,//901
            PixelFormat.Format16bppArgb1555,//4536
            PixelFormat.Format32bppArgb,//11474
            PixelFormat.Format64bppArgb//16051
        };

        /// <summary>
        /// 获取Bitmap编码数组
        /// </summary>
        public static EncoderParameters GetEncoderParameters(long value)
        {
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = GetParameter(value);
            return encoderParameters;
        }

        /// <summary>
        /// 获取Bitmap编码信息
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
        /// 获取图像编码信息
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
