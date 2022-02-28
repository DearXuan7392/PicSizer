using System;
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
        /// 压缩模式
        /// </summary>
        public CompressionMode compressionMode = CompressionMode.SizeFirst;

        /// <summary>
        /// 非JPEG图片的压缩方式
        /// </summary>
        public NonJEPGCompressMethod nonJEPGCompressMethod = NonJEPGCompressMethod.ScaleBased;

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
        /// ICON图标的尺寸，宽==高
        /// </summary>
        public byte IconLimitSize = 64;

        /// <summary>
        /// 指定大小(KB)
        /// </summary>
        public long LimitSize = 200;

        /// <summary>
        /// 指定画质
        /// </summary>
        public long CompressionValue = 80L;

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
        public bool topMost = true;

        /// <summary>
        /// 是否启动GPU加速
        /// </summary>
        public bool useGPU = false;
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
