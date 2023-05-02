using PicSizer.Static;
using PicSizer.Window.Partial;
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
        public void Compress_By_FileSize(long limit, string output)
        {
            Compress_By_FileSize(limit, output, null);
        }

        /// <summary>
        /// 将图片压缩到指定大小之内
        /// </summary>
        /// <param name="bitmap">图片</param>
        /// <param name="limit">输出文件大小(KB)</param>
        /// <param name="output">输出路径</param>
        /// <param name="_x">起始值,具体范围为_x±20</param>
        public void Compress_By_FileSize(long limit, string output, long? _x)
        {
            long left, right, mid;
            // 将画质分为 100 个等级: 1-100
            long[] sizeList = new long[101];
            // 没有指定查找范围,则计算大致值
            if(_x == null)
            {
                // 求出画质为 100 时的图片大小
                sizeList[100] = GetBitmapSize(bitmap, 100);
                // 预估 limit 对应的画质值
                _x = (long)x((double)limit * 1024 / sizeList[100]);
            }
            // 防止范围越界
            if (_x < 20) _x = 20;
            if (_x > 80) _x = 80;
            // 查找起始区间为 _x ± 20
            left = _x.Value - 20;
            right = _x.Value + 20;
            
            // 寻找不大于 limit 的最优值, sizeList[mid] 不一定与 limit 严格相等
            while (left < right - 1)
            {
                // 获取下一个值
                mid = (left + right) / 2;
                // 获取当前画质下的图片大小,并存入数组
                sizeList[mid] = sizeList[mid] == 0
                    ? GetBitmapSize(bitmap, mid)
                    : sizeList[mid];
                // 根据大小往左或往右查找
                if (sizeList[mid] <= (limit - 1) * 1024)
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
            sizeList[left] = sizeList[left] == 0
                    ? GetBitmapSize(bitmap, left)
                    : sizeList[left];
            // 如果文件大小符合要求或者接受超出限制的文件就输出
            if (sizeList[left] <= (limit - 1) * 1024 || (PicSetting.AcceptExceedPicture && _x == 20))
            {
                FileStream fs = null;
                try
                {
                    encoderParameters.Param[0] = Encoder.GetParameter(left);
                    fs = new FileStream(output, FileMode.Create, FileAccess.Write);
                    bitmap.Save(fs, Encoder._Info_JPEG, encoderParameters);
                    fs.Close();
                }
                finally
                {
                    fs?.Dispose();
                }
                return;
            }
            // 如果文件还是太大,则把查找范围左移40位
            if(_x > 20)
            {
                Compress_By_FileSize(limit, output, _x - 40);
            }
            else
            {
                throw new Exception(PicStrings.COMPRESS_CANNOT_RESIZE_TO_LIMIT_SIZE);
            }
        }

        /// <summary>
        /// 图像编码信息
        /// </summary>
        private static EncoderParameters encoderParameters = new EncoderParameters();

        /// <summary>
        /// 计算指定画质下 Bitmap 输出到流后的大小
        /// </summary>
        private static long GetBitmapSize(Bitmap bitmap, long value)
        {
            encoderParameters.Param[0] = Encoder.GetParameter(value);
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                // 将 Bitmap 输入到内存流
                bitmap.Save(memoryStream, Encoder._Info_JPEG, encoderParameters);
                long size = memoryStream.Length;
                return size;
            }
            finally
            {
                // 销毁内存流
                memoryStream?.Dispose();
            }
        }

        /**
         * y = k / (101 - x) + c
         * x : 画质(1 ~ 100)
         * y : 画质为 x 时,输出图片的大小与画质为 100 时的比值
         */

        private const double k = 1.12579407311705;
        private const double c = 0.119686042016207;

        private static double y(double x)
        {
            return k / (101 - x) + c;
        }

        private static double x(double y)
        {
            return 101 - k / (y - c);
        }
    }
}
