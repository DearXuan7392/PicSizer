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
        int finish = 0;
        int total = 0;

        public static ProgressForm form;

        public delegate void UpdateProgressDelegate(int now);
        public delegate void PrepareToHideDelegate();

        UpdateProgressDelegate __UpdateProgress = new UpdateProgressDelegate(_SetNow);
        PrepareToHideDelegate __PrepareToHide = new PrepareToHideDelegate(_PrepareToHide);

        public ProgressForm()
        {
            InitializeComponent();
        }

        public void init(int total)
        {
            this.total = total;
            label4.Text = "0";
            label5.Text = total.ToString();
            label6.Text = "0%";
            progressBar1.Value = 0;
            Setting.ThreadExitNow = false;
        }

        public void SetNow(int now)
        {
            this.Invoke(__UpdateProgress,now);
        }

        private static void _SetNow(int now)
        {
            form.finish = now;
            int percent = now * 100 / form.total;
            form.label4.Text = now.ToString();
            form.label6.Text = percent + "%";
            form.progressBar1.Value = percent;
            if (now == form.total) form.PrepareToHide();
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
            Setting.ThreadExitNow = true;
        }

        public static void _PrepareToHide()
        {
            form.Hide();
            string s = "总共: " + form.total + " 张\n压缩完成: " + form.finish + "张\n未完成: " + (form.total - form.finish) + "张";
            MessageBox.Show(s, "压缩已结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void PrepareToHide()
        {
            this.Invoke(__PrepareToHide);
        }
    }
}
