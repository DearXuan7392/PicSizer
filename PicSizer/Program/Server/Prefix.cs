using PicSizer.Logic;
using PicSizer.Window;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Server
{
    public static class Prefix
    {
        /// <summary>
        /// 从文件里读取图片
        /// </summary>
        public static Bitmap GetBitmap(string input)
        {
            Bitmap ori = null;
            //原始图片
            if (input.EndsWith(".webp"))
            {
                ori = WEBP.GetBitmap(input);
            }
            else
            {
                ori = new Bitmap(input);
            }
            Bitmap _bitmap = null;
            //克隆到新的bitmap
            switch (ori.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format16bppArgb1555:
                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format64bppPArgb:
                    //含有Alpha通道,设置为32位ARGB格式
                    _bitmap = ori.Clone(
                        new Rectangle(0, 0, ori.Width, ori.Height),
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    break;
                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb555:
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb565:
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                case System.Drawing.Imaging.PixelFormat.Format48bppRgb:
                    //不含Alpha通道,设置为32位ARGB格式
                    _bitmap = ori.Clone(
                        new Rectangle(0, 0, ori.Width, ori.Height),
                        System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    //已经是24位RGB或32位ARGB,则什么也不做
                    break;
                default:
                    throw new Exception("未知的像素格式");
            }
            //销毁原始图像
            if(_bitmap == null)
            {
                _bitmap = ori;
            }
            else
            {
                ori.Dispose();
            }
            //裁剪
            Logic.Graph.ResizeBitmap(ref _bitmap);
            //水印
            Logic.Graph.AddWatermark(ref _bitmap);
            //亮度
            Logic.Graph.SetBrightness(ref _bitmap);
            //返回复制后的图像
            return _bitmap;
        }
    }
}
