using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Partial
{
    [Serializable]
    public class Setting
    {
        /// <summary>
        /// 尺寸修正模式
        /// </summary>
        public ResizeMode resizeMode = ResizeMode.MinSize;

        /// <summary>
        /// 压缩模式
        /// </summary>
        public CompressionMode compressionMode = CompressionMode.SizeFirst;

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
    }

    public static class SettingIO
    {
        [Serializable]
        public class SettingFilePrefix
        {
            public Version PicSizerVersion;
            public Version SettingVersion;
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
                    if(settingFilePrefix.SettingVersion.CompareTo(Info.SettingVersion) != 0)
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
