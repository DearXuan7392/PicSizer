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
    public partial class ProgressForm : Form
    {
        static int success = 0; // 压缩成功
        static int error = 0; // 压缩失败
        static int total = 0; // 总数

        public ProgressForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        public void init(int total)
        {
            success = 0;
            error = 0;
            ProgressForm.total = total;
            label5.Text = "0";
            label6.Text = "0";
            label7.Text = total.ToString();
            label8.Text = "0%";
            button1.Enabled = true;
            progressBar1.Value = 0;
            SharedVariable.ThreadExitNow = false;
        }

        public void AddOne(bool flag)
        {
            if (flag)
            {
                success++;
            }
            else
            {
                error++;
            }
            int sum = success + error;
            int percent = sum * 100 / total;
            label5.Text = success.ToString();
            label6.Text = error.ToString();
            label8.Text = percent + "%";
            progressBar1.Value = percent;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            //屏蔽Alt+F4
            if(e.KeyCode == Keys.F4 && e.Alt == true)
            {
                e.Handled = true;
            }
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SharedVariable.ThreadExitNow = true;
            button1.Enabled = false;
        }

        public void PrepareToHide()
        {
            this.Hide();
            string s = "总共: " + total + " 张\n压缩完成: " + success + "张\n未完成: " + (total - success) + "张";
            MessageBox.Show(s, "压缩已结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
