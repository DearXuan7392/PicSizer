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
            richTextBox1.Text = richTextBox1.Text
                .Replace("{{project_name}}", Info.ProjectName)
                .Replace("{{project_version}}", Info.ProjectVersion.ToString());
            button_download_alpha.Visible = !Info.ProjectVersion.alpha;
        }

        private void About_Load(object sender, EventArgs e)
        {
            this.TopMost = Value.setting.topMost;
        }

        private void button_author_Click(object sender, EventArgs e)
        {
            (new DearXuan()).ShowDialog();
        }

        private void button_download_alpha_Click(object sender, EventArgs e)
        {
            Class.Partial.Update.CheckUpdate(true);
        }
    }
}
