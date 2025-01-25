#region

using System;
using System.Drawing;
using System.IO;
using PicSizer.Program.FileIO;
using PicSizer.Program.Static;

#endregion

namespace PicSizer.Program.Logic.Compress
{
    public class CompressItem : IDisposable
    {
        /// <summary>
        /// 图片对象
        /// </summary>
        internal Bitmap Img;

        /// <summary>
        /// 输出文件名
        /// </summary>
        private string _outputFilename;

        /// <summary>
        /// 最大画质
        /// </summary>
        protected int MaxQuality = 100;

        protected CompressItem()
        {
        }

        protected void Init(string imgPath, string outputFilename, int maxQuality = 100)
        {
            Img = FileProc.GetBitmap(imgPath);
            _outputFilename = outputFilename;
            MaxQuality = maxQuality;
            Graph.Graph.InitGraph(ref Img);
        }

        public static PicResult Compress(string input, string output)
        {
            string ext = FileProc.GetExtension(output);
            CompressItem item;
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                    item = new Jpeg(input, output);
                    break;
                case ".png":
                    item = new Png(input, output);
                    break;
                case ".webp":
                    item = new Webp(input, output);
                    break;
                case ".ico":
                    item = new Icon(input, output);
                    break;
                default:
                    return PicResult.GetError($"\"{ext}\"格式不受支持.");
            }

            PicResult result;
            switch (PicSetting.CompressType)
            {
                case PicUnit.CompressType.Quality:
                    result = item.CompressByQuality(PicSetting.Quality);
                    break;
                case PicUnit.CompressType.FileSize:
                    result = item.CompressByFilesize(PicSetting.LimitSize);
                    break;
                default:
                    return PicResult.GetError(PicStrings.NotImplementedError);
            }

            item.Dispose();
            return result;
        }

        private PicResult CompressByFilesize(long limit)
        {
            int left = 0, right = MaxQuality;
            var sizeList = new long[MaxQuality + 1];
            limit = (limit - 1L) * 1024L;

            // 寻找不大于 limit 的最优值, sizeList[mid] 不一定与 limit 严格相等
            while (left < right - 1L)
            {
                var mid = (left + right) / 2;
                // 获取当前画质下的图片大小,并存入数组
                if (sizeList[mid] == 0L)
                {
                    sizeList[mid] = GetBitmapSizeWithQuality(mid);
                }

                // 根据大小往左或往右查找
                if (sizeList[mid] <= limit)
                {
                    left = mid;
                }
                else
                {
                    right = mid;
                }
            }

            // 退出循环时，sizeList[left] 就是不大于 limit 的最大值
            // 获取画质为 left 时的大小，即是不大于 LimitSize 的最高画质
            if (sizeList[left] == 0L)
            {
                sizeList[left] = GetBitmapSizeWithQuality(left);
            }

            // 如果文件大小符合要求或者接受超出限制的文件就输出
            if (sizeList[left] <= limit || PicSetting.AcceptExceedPicture)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(_outputFilename, FileMode.Create, FileAccess.Write);
                    WriteToStreamWithQuality(fs, left);
                    fs.Close();
                    return PicResult.GetOk();
                }
                catch (Exception ex)
                {
                    return PicResult.GetError(ex);
                }
                finally
                {
                    fs?.Dispose();
                }
            }

            return PicResult.GetOutOfLimit();
        }

        private PicResult CompressByQuality(int quality)
        {
            if (quality < 0 || quality > 100)
            {
                return PicResult.GetError(PicStrings.ArgumentOutOfRange);
            }

            FileStream fs = null;
            try
            {
                fs = new FileStream(_outputFilename, FileMode.Create, FileAccess.Write);
                WriteToStreamWithQuality(fs, quality);
                fs.Close();
                return PicResult.GetOk();
            }
            catch (Exception ex)
            {
                return PicResult.GetError(ex);
            }
            finally
            {
                fs?.Dispose();
            }
        }

        /// <summary>
        /// 获取指定画质下的图像大小
        /// </summary>
        /// <param name="quality">画质, 0 - 100</param>
        private long GetBitmapSizeWithQuality(int quality)
        {
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                WriteToStreamWithQuality(memoryStream, quality);
                return memoryStream.Length;
            }
            finally
            {
                memoryStream?.Dispose();
            }
        }

        /// <summary>
        /// 写入到流
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="quality">画质</param>
        /// <returns>流的大小</returns>
        protected virtual void WriteToStreamWithQuality(Stream stream, int quality)
        {
            throw new Exception(PicStrings.NotImplementedError);
        }

        public void Dispose()
        {
            Img.Dispose();
        }

        ~CompressItem()
        {
            Dispose();
        }
    }
}