using System;
using System.Windows.Forms;

namespace PicSizer.Forms
{
    public partial class DearXuan : Form
    {
        public DearXuan()
        {
            this.Icon = Class.Static.Info.icon;
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
            this.TopMost = PicSizer.Class.Static.Value.setting.topMost;
        }

        private void OnCopyClick(object sender, EventArgs e)
        {
            string copy = null;
            if (sender == button1)
            {
                copy = textBox2.Text;
            }
            else if (sender == button3)
            {
                copy = textBox3.Text;
            }
            else if (sender == button5)
            {
                copy = textBox4.Text;
            }
            if (copy != null) Clipboard.SetText(copy);
        }

        private void OnOpenClick(object sender, EventArgs e)
        {
            string link = null;
            if (sender == button2)
            {
                link = textBox2.Text;
            }
            else if (sender == button4)
            {
                link = textBox3.Text;
            }
            else if (sender == button6)
            {
                link = textBox4.Text;
            }
            if (link != null) PicSizer.Class.Partial.Dialog.OpenLink(link);
        }

        private void OnSendEmailClick(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:mail@dearxuan.com");
            }
            catch (Exception)
            {
                Clipboard.SetText("mail@dearxuan.com");
                MessageBox.Show("打开邮箱失败.邮箱地址已经复制到剪贴板.", "PicSizer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnPicClick(object sender, EventArgs e)
        {
            MessageBox.Show("id=90743869", "宵宫");
        }
    }
}
