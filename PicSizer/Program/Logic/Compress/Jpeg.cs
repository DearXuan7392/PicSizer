#region

using System.Drawing.Imaging;
using System.IO;

#endregion

namespace PicSizer.Program.Logic.Compress
{
    public class Jpeg : CompressItem
    {
        /// <summary>`
        /// 图像编码信息
        /// </summary>
        private EncoderParameters _encoderParameters = new EncoderParameters();

        public Jpeg(string imgPath, string outputFilename)
        {
            Init(imgPath, outputFilename);
        }

        protected override void WriteToStreamWithQuality(Stream stream, int quality)
        {
            _encoderParameters.Param[0] = Encoder.GetParameter(quality);
            Img.Save(stream, Encoder.InfoJpeg, _encoderParameters);
        }
    }
}