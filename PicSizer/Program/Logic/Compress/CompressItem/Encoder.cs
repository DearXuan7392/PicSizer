using System.Drawing.Imaging;

namespace PicSizer.Logic
{
    public static class Encoder
    {
        public static ImageCodecInfo _Info_JPEG = GetEncoder(ImageFormat.Jpeg);
        public static ImageCodecInfo _Info_PNG = GetEncoder(ImageFormat.Png);

        private static EncoderParameter[] parameterList = new EncoderParameter[101];

        /// <summary>
        /// 获取Bitmap编码信息
        /// </summary>
        public static EncoderParameter GetParameter(long value)
        {
            int v = (int)value;
            if (parameterList[v] == null)
            {
                parameterList[v] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, value);
            }
            return parameterList[v];
        }

        /// <summary>
        /// 获取图像编码信息
        /// </summary>
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }
    }
}
