#region

using System.Collections.Generic;
using System.Windows.Forms;

#endregion

namespace PicSizer.Program.Window.Assemble
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
        private readonly PicListViewMenu picListViewMenu = new PicListViewMenu();
    }
}