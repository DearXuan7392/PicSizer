using PicSizer.Partial;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicSizer.Unit
{
    public static class Extension
    {
        /// <summary>
        /// Bitmap支持的格式的集合(小写,包括点号)
        /// </summary>
        public static HashSet<string> BitmapSupportExtension = new HashSet<string>();

        /// <summary>
        /// 后缀名到ImageCodecInfo类的集合
        /// </summary>
        public static Dictionary<string, ImageCodecInfo> BitmapExportExtension = new Dictionary<string, ImageCodecInfo>();

        public class ExtensionTypeList
        {
            public string name;
            public string[] list;
        }
    }
}
