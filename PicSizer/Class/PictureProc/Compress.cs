using PicSizer.Class.Partial;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using PicSizer.Class.Static;

namespace PicSizer.Class.PictureProc
{

    /// <summary>
    /// 图像压缩
    /// </summary>
    public static class Compress
    {
        /// <summary>
        /// 根据生成的类型自动选择压缩方式
        /// </summary>
        public static bool CompressAtomPic(AtomPic atomPic)
        {
            switch (atomPic.Extension)
            {
                case ExtensionMode.JPEG:
                    //指定画质
                    if (Value.setting.CompressionMode_Jpeg == CompressionMode_JPEG.QualityFirst)
                    {
                        return CompressionByQuality(atomPic);
                    }
                    //大小优先
                    else
                    {
                        return CompressionBySize_QualityFirst(atomPic);
                    }
                case ExtensionMode.ICON:
                    return CompressionBySize_ScaleFirst(atomPic);
                default:
                    //基于缩放的压缩
                    if (Value.setting.CompressionMode_Other == CompressionMode_Other.ScaleBased)
                    {
                        return CompressionBySize_ScaleFirst(atomPic);
                    }
                    //基于位深度的压缩
                    else
                    {
                        return CompressionBySize_BitDepthFirst(atomPic);
                    }
            }
        }

        /// <summary>
        /// 基于画质压缩(仅限JPEG)
        /// </summary>
        private static bool CompressionByQuality(AtomPic atomPic)
        {
            atomPic.SaveToFileByQuality(Value.setting.Quality);
            return true;
        }

        /// <summary>
        /// 基于大小压缩,依照画质区分(仅限JPEG)
        /// </summary>
        private static bool CompressionBySize_QualityFirst(AtomPic atomPic)
        {
            EncoderParameters encoderParameters = new EncoderParameters(1);
            long left = 0L, right = 100L, mid = 0L;
            long[] sizeList = new long[101];
            while (left < right - 1)
            {
                mid = (left + right) / 2;
                sizeList[mid] = BitmapStream.GetBitmapSizeByQuality(atomPic.bitmap, encoderParameters, Encoder._Info_JPEG, mid);
                if (IsLengthLegal(sizeList[mid]))
                {
                    left = mid;
                }
                else
                {
                    right = mid;
                }
            }
            //获取画质为left时的大小，即是不大于LimitSize的最高画质
            if (sizeList[left] == 0)
            {
                sizeList[left] = BitmapStream.GetBitmapSizeByQuality(atomPic.bitmap, encoderParameters, Encoder._Info_JPEG, left);
            }
            //如果文件大小符合要求或者接受超出限制的文件就输出
            if (IsLengthLegal(sizeList[left]) || Value.setting.AcceptExceedPicture)
            {
                atomPic.SaveToFileByQuality(left);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 基于大小压缩,依照缩放比例区分(非JPEG)
        /// </summary>
        private static bool CompressionBySize_ScaleFirst(AtomPic atomPic)
        {
            int left = 1, right = 100, mid = 50;
            long[] sizeList = new long[101];
            while (left < right - 1)
            {
                mid = (left + right) / 2;
                sizeList[mid] = BitmapStream.GetBitmapSizeByScale(atomPic.bitmap, atomPic.ExportImageFormat, atomPic.bitmap.Width * mid / 100, atomPic.bitmap.Height * mid / 100);
                if (IsLengthLegal(sizeList[mid]))
                {
                    left = mid;
                }
                else
                {
                    right = mid;
                }
            }
            //获取缩放为left时的大小，即是不大于LimitSize的最高画质
            if (sizeList[left] == 0)
            {
                sizeList[left] = BitmapStream.GetBitmapSizeByScale(atomPic.bitmap, atomPic.ExportImageFormat, atomPic.bitmap.Width * mid / 100, atomPic.bitmap.Height * mid / 100);
            }
            //如果文件大小符合要求或者接受超出限制的文件就输出
            if (IsLengthLegal(sizeList[left]) || Value.setting.AcceptExceedPicture)
            {
                atomPic.SaveToFileBySize(atomPic.bitmap.Width * left / 100, atomPic.bitmap.Height * left / 100);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 基于大小压缩,依照分辨率区分(非JPEG)
        /// </summary>
        private static bool CompressionBySize_DpiFirst(AtomPic atomPic)
        {
            int maxDpi = (int)Math.Max(atomPic.bitmap.HorizontalResolution, atomPic.bitmap.VerticalResolution);
            int left = 1, right = maxDpi, mid = 0;
            long[] sizeList = new long[right + 1];
            while (left < right - 1)
            {
                mid = (left + right) / 2;
                sizeList[mid] = BitmapStream.GetBitmapSizeByDpi(atomPic.bitmap, atomPic.ExportImageFormat, atomPic.bitmap.HorizontalResolution * mid / maxDpi, atomPic.bitmap.VerticalResolution * mid / maxDpi);
                if (IsLengthLegal(sizeList[mid]))
                {
                    left = mid;
                }
                else
                {
                    right = mid;
                }
            }
            //获取Dpi为left时的大小，即是不大于LimitSize的最高画质
            if (sizeList[left] == 0)
            {
                sizeList[left] = BitmapStream.GetBitmapSizeByDpi(atomPic.bitmap, atomPic.ExportImageFormat, atomPic.bitmap.HorizontalResolution * left / maxDpi, atomPic.bitmap.VerticalResolution * left / maxDpi);
            }
            //如果文件大小符合要求或者接受超出限制的文件就输出
            if (IsLengthLegal(sizeList[left]) || Value.setting.AcceptExceedPicture)
            {
                atomPic.SaveToFileByResolution(atomPic.bitmap.HorizontalResolution * left / maxDpi, atomPic.bitmap.VerticalResolution * left / maxDpi);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 基于大小压缩,依照位深度区分(非JPEG)
        /// </summary>
        private static bool CompressionBySize_BitDepthFirst(AtomPic atomPic)
        {
            Rectangle rect = new Rectangle(0, 0, atomPic.bitmap.Width, atomPic.bitmap.Height);
            int left = 0, right = Encoder.pixelFormats.Length - 1, mid = 0;
            long[] sizeList = new long[Encoder.pixelFormats.Length];
            while (left < right - 1)
            {
                mid = (left + right) / 2;
                sizeList[mid] = BitmapStream.GetBitmapSizeByPixelDeep(atomPic.bitmap, atomPic.ExportImageFormat, rect, Encoder.pixelFormats[mid]);
                if (IsLengthLegal(sizeList[mid]))
                {
                    left = mid;
                }
                else
                {
                    right = mid;
                }
            }
            //获取画质为left时的大小，即是不大于LimitSize的最高画质
            if (sizeList[left] == 0)
            {
                sizeList[left] = BitmapStream.GetBitmapSizeByPixelDeep(atomPic.bitmap, atomPic.ExportImageFormat, rect, Encoder.pixelFormats[left]);
            }
            //如果文件大小符合要求或者接受超出限制的文件就输出
            if (IsLengthLegal(sizeList[left]) || Value.setting.AcceptExceedPicture)
            {
                atomPic.SaveToFileByBitDeep(rect, Encoder.pixelFormats[left]);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsLengthLegal(long length)
        {
            //留出 1 KB用于保存文件时的附加信息
            return length <= (Value.setting.LimitSize - 1) * 1024;
        }
    }
}
