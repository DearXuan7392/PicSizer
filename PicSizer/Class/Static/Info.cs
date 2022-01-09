using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer
{
    public static class Info
    {
        /// <summary>
        /// 是否支持硬件加速
        /// </summary>
        public static bool isGPUSupport = false;

        /// <summary>
        /// 版本
        /// </summary>
        public static readonly Partial.Version ProjectVersion = new Partial.Version()
        {
            mainVersion = 3,
            secondVersion = 3,
            thirdVersion = 2,
            alpha = true
        };

        /// <summary>
        /// 项目名称
        /// </summary>
        public const string ProjectName = "PicSizer";

        /// <summary>
        /// 设置文件的扩展名
        /// </summary>
        public const string SettingFileExtension = "pics";

        /// <summary>
        /// 配置文件的版本，防止跨版本导入
        /// </summary>
        public static readonly Partial.Version SettingVersion = new Partial.Version()
        {
            mainVersion = 1,
            secondVersion = 0,
            thirdVersion = 0,
            alpha = false
        };
    }
}
