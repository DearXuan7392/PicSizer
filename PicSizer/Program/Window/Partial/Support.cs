using System;

namespace PicSizer.Window.Partial
{
    /// <summary>
    /// 支持
    /// </summary>
    public static class Support
    {
        /// <summary>
        /// Enum转int
        /// </summary>
        public static int ToInt(this Enum e)
        {
            return e.GetHashCode();
        }
    }

}
