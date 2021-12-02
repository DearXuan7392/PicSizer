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
    public partial class DearXuan : Form
    {
        public DearXuan()
        {
            InitializeComponent();
        }

        private void OnCopyClick(object sender, EventArgs e)
        {
            if(sender == button1)
            {
                Clipboard.SetText(textBox2.Text);
            }
            else
            {
                Clipboard.SetText(textBox3.Text);
            }
        }

        private void OnOpenClick(object sender, EventArgs e)
        {
            try
            {
                if(sender == button2)
                {
                    System.Diagnostics.Process.Start(textBox2.Text);
                }
                else
                {
                    System.Diagnostics.Process.Start(textBox3.Text);
                }
            }catch(Exception ex)
            {
                MessageBox.Show("打开浏览器失败，请将链接复制到浏览器打开.", "PicSizer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
