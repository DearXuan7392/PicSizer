using PicSizer.Static;
using System;
using System.Windows.Forms;

namespace PicSizer.Window.Forms
{
    public partial class AboutForm : PicBaseForm
    {
        public AboutForm()
        {
            InitializeComponent();
            richTextBox1.Text = richTextBox1.Text
                .Replace("{{project_name}}", PicInfo.ProjectName)
                .Replace("{{project_version}}", PicInfo.ProjectVersion);
        }

        private void About_Load(object sender, EventArgs e)
        {
            
        }
    }
}
