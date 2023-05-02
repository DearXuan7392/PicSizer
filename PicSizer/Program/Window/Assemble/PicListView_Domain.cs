using PicSizer.Window.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSizer.Window.Assemble
{
    /// <summary>
    /// PicListView的属性类
    /// </summary>
    public partial class PicListView : ListView
    {
        public PicListView()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        /// <summary>
        /// PicListView里的所有图片的路径集合
        /// </summary>
        public HashSet<string> ItemPathHashSet = new HashSet<string>();

        /// <summary>
        /// 右键菜单
        /// </summary>
        private PicListViewMenu picListViewMenu = new PicListViewMenu();
    }
}
