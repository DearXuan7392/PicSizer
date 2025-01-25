#region

using System.Drawing.Imaging;

#endregion

namespace PicSizer.Program.Logic.Compress
{
    public static class Encoder
    {
        public static readonly ImageCodecInfo InfoJpeg = GetEncoder(ImageFormat.Jpeg);
        public static readonly ImageCodecInfo InfoPng = GetEncoder(ImageFormat.Png);

        private static readonly EncoderParameter[] ParameterList = new EncoderParameter[101];

        /// <summary>
        /// 获取Bitmap编码信息
        /// </summary>
        public static EncoderParameter GetParameter(int value)
        {
            if (ParameterList[value] == null)
            {
                ParameterList[value] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, value);
            }
            return ParameterList[value];
        }

        /// <summary>
        /// 获取图像编码信息
        /// </summary>
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            foreach (var codec in ImageCodecInfo.GetImageEncoders())
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