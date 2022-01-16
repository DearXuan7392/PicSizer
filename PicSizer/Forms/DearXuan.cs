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
            this.Icon = Resource.yoimiya_ico;
            InitializeComponent();
        }

        private void DearXuan_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
            textBox1.SelectionStart = 0;
            textBox1.SelectionLength = 0;
            textBox2.SelectionStart = 0;
            textBox2.SelectionLength = 0;
            textBox3.SelectionStart = 0;
            textBox3.SelectionLength = 0;
            textBox4.SelectionStart = 0;
            textBox4.SelectionLength = 0;
        }

        private void OnCopyClick(object sender, EventArgs e)
        {
            string copy = null;
            if(sender == button1)
            {
                copy = textBox2.Text;
            }
            else if(sender == button3)
            {
                copy = textBox3.Text;
            }
            else if(sender == button5)
            {
                copy = textBox4.Text;
            }
            if(copy != null) Clipboard.SetText(copy);
        }

        private void OnOpenClick(object sender, EventArgs e)
        {
            try
            {
                string link = null;
                if(sender == button2)
                {
                    link = textBox2.Text;
                }
                else if(sender == button4)
                {
                    link = textBox3.Text;
                }
                else if(sender == button6)
                {
                    link = textBox4.Text;
                }
                if(link != null) System.Diagnostics.Process.Start(link);
            }
            catch(Exception ex)
            {
                MessageBox.Show("打开浏览器失败，请将链接复制到浏览器打开.", "PicSizer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnSendEmailClick(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:mail@dearxuan.top");
            }
            catch (Exception ex)
            {
                Clipboard.SetText("mail@dearxuan.top");
                MessageBox.Show("打开邮箱失败.邮箱地址已经复制到剪贴板.", "PicSizer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnPicClick(object sender, EventArgs e)
        {
            MessageBox.Show("id=90743869", "宵宫");
        }
    }
}
