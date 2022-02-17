using System;
using System.Windows.Forms;
using PicSizer.Class.Static;

namespace PicSizer
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            this.Icon = Info.icon;
            richTextBox1.Text = Info.ProjectName + "\n"
                + "程序版本: " + Info.ProjectVersion + "\n\n"
                + Info.Description;
        }

        private void About_Load(object sender, EventArgs e)
        {
            this.TopMost = Value.setting.topMost;
        }

        private void button_author_Click(object sender, EventArgs e)
        {
            (new DearXuan()).ShowDialog();
        }
    }
}
