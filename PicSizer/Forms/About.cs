using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSizer
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            richTextBox1.Text = Info.ProjectName + "\n"
                + "程序版本: " + Info.ProjectVersion + "\n\n"
                + Info.Description;
        }

        private void About_Load(object sender, EventArgs e)
        {

        }
    }
}
