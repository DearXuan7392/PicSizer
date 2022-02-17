using System;

namespace PicSizer.Class.Unit
{
    [Serializable]
    public class Version
    {
        /// <summary>
        /// 主版本
        /// </summary>
        public byte mainVersion = 0;

        /// <summary>
        /// 次要版本
        /// </summary>
        public byte secondVersion = 0;

        /// <summary>
        /// 再次版本
        /// </summary>
        public byte thirdVersion = 0;

        /// <summary>
        /// 是否开发板
        /// </summary>
        public bool alpha = false;

        /// <summary>
        /// 版本比较
        /// </summary>
        public int CompareTo(Version other)
        {
            if (mainVersion != other.mainVersion) return mainVersion > other.mainVersion ? 1 : -1;
            if (secondVersion != other.secondVersion) return secondVersion > other.secondVersion ? 1 : -1;
            if (thirdVersion != other.thirdVersion) return thirdVersion > other.thirdVersion ? 1 : -1;
            if (alpha == other.alpha) return 0;
            return alpha ? -1 : 1;
        }

        override public string ToString()
        {
            string s = mainVersion + "." + secondVersion + "." + thirdVersion;
            if (alpha)
            {
                s += "-alpha";
            }
            return s;
        }
    }
}
