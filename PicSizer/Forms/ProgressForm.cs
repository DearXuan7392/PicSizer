using System;
using System.Windows.Forms;
using PicSizer.Class.Static;

namespace PicSizer.Forms
{
    public partial class ProgressForm : Form
    {
        static int success = 0; // 压缩成功
        static int error = 0; // 压缩失败
        static int total = 0; // 总数

        public ProgressForm()
        {
            InitializeComponent();
            Support.ForbidAltF4(this);
            this.Icon = Info.icon;
            CheckForIllegalCrossThreadCalls = false;
        }

        public void Init(int total)
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
            Value.ThreadExitNow = false;
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

        private void ProgressForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Value.ThreadExitNow = true;
            button1.Enabled = false;
        }

        public void PrepareToHide()
        {
            this.Hide();
            PicSizer.Class.Partial.Dialog.ShowDialog_ResizeFinish(total, success);
        }
    }
}
