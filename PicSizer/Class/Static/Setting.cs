using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using PicSizer.Class.Static;

namespace PicSizer.Class.Partial
{
    [Serializable]
    public class Setting
    {
        /// <summary>
        /// 尺寸修正模式
        /// </summary>
        public ResizeMode resizeMode = ResizeMode.None;

        /// <summary>
        /// JPEG图片的压缩方式
        /// </summary>
        public CompressionMode_JPEG CompressionMode_Jpeg = CompressionMode_JPEG.SizeFirst;

        /// <summary>
        /// ICON图片的压缩方式
        /// </summary>
        public CompressionMode_ICON CompressionMode_Icon = CompressionMode_ICON.ScaleBased;

        /// <summary>
        /// 其它图片的压缩方式
        /// </summary>
        public CompressionMode_Other CompressionMode_Other = CompressionMode_Other.ScaleBased;

        /// <summary>
        /// 命名方式
        /// </summary>
        public RenameMode renameMode = RenameMode.Number;

        /// <summary>
        /// 文件后缀
        /// </summary>
        public ExtensionMode extensionMode = ExtensionMode.JPEG;

        /// <summary>
        /// 限制宽度
        /// </summary>
        public int LimitWidth = 1920;

        /// <summary>
        /// 限制高度
        /// </summary>
        public int LimitHeight = 1080;

        /// <summary>
        /// ICON图标的尺寸，宽
        /// </summary>
        public byte IconLimitWidth = 64;

        /// <summary>
        /// ICON图标的尺寸，高
        /// </summary>
        public byte IconLimitHeight = 64;

        /// <summary>
        /// 指定大小(KB)
        /// </summary>
        public long LimitSize = 200;

        /// <summary>
        /// 指定画质
        /// </summary>
        public long Quality = 80L;

        /// <summary>
        /// 起始下标
        /// </summary>
        public int StartIndex = 1;

        /// <summary>
        /// 混合方式命名
        /// </summary>
        public string CustomRenameStr;

        /// <summary>
        /// 错误处理
        /// </summary>
        public DoWhenException doWhenException = DoWhenException.IgnoreAndContinue;

        /// <summary>
        /// 当无法最小文件大小超过限制时，是否接受最小的文件大小
        /// </summary>
        public bool AcceptExceedPicture = false;

        /// <summary>
        /// 允许任意后缀
        /// </summary>
        public bool AllowAnyExtension = false;

        /// <summary>
        /// 亮度
        /// </summary>
        public byte brightness = 100; // 0表示完全黑暗，100表示不变暗

        /// <summary>
        /// 最大线程数
        /// </summary>
        public int maxThreads = 2;

        /// <summary>
        /// 永远置顶
        /// </summary>
        public bool topMost = false;

        /// <summary>
        /// 是否启动GPU加速
        /// </summary>
        public bool useGPU = false;

        public byte[] backgroundColor = { 255, 255, 255 };

        /// <summary>
        /// 水印模式
        /// </summary>
        public WatermarkMode watermarkMode = WatermarkMode.Non;

        /// <summary>
        /// 水印透明度,范围: 0~100,0表示完全没有,100表示明显水印
        /// </summary>
        public byte watermarkAlpha = 100;

        /// <summary>
        /// 水印字体
        /// </summary>
        public Font watermarkFont = SystemFonts.DefaultFont;

        /// <summary>
        /// 水印颜色
        /// </summary>
        public byte[] watermarkColor = {0, 0, 0};

        /// <summary>
        /// 水印字符
        /// </summary>
        public string watermarkText = null;
    }

    public static class SettingIO
    {
        [Serializable]
        public class SettingFilePrefix
        {
            public Unit.Version PicSizerVersion;
            public Unit.Version SettingVersion;
        }

        public static void WriteSettingToFile(Setting setting, string filename)
        {
            try
            {
                SettingFilePrefix settingFilePrefix = new SettingFilePrefix()
                {
                    PicSizerVersion = Info.ProjectVersion,
                    SettingVersion = Info.SettingVersion
                };
                using (FileStream fileStream = new FileStream(filename, FileMode.Create))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(fileStream, settingFilePrefix);
                    binaryFormatter.Serialize(fileStream, setting);
                }
            }
            catch (Exception)
            {
                Dialog.ShowDialog_SavingFailed();
            }
        }

        public static Setting ReadSettingFromFile(string filename)
        {
            try
            {
                using (FileStream fileStream = new FileStream(filename, FileMode.Open))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    SettingFilePrefix settingFilePrefix = binaryFormatter.Deserialize(fileStream) as SettingFilePrefix;
                    if (settingFilePrefix.SettingVersion.CompareTo(Info.SettingVersion) != 0)
                    {
                        if (!Dialog.ShowDialog_VersionError(settingFilePrefix))
                        {
                            return null;
                        }
                    }
                    Setting setting = binaryFormatter.Deserialize(fileStream) as Setting;
                    return setting;
                }
            }
            catch (Exception)
            {
                Dialog.ShowDialog_OpenSettingFailed();
            }
            return null;
        }
    }
}
