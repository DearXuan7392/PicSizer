#region

using System;
using PicSizer.Program.Static;

#endregion

namespace PicSizer.Program.Window.Forms
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