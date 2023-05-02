using System;
using System.Windows.Forms;
using PicSizer.Static;
using PicSizer.Window.Partial;

namespace PicSizer.Window.Forms
{
    public partial class ProgressForm : PicBaseForm
    {
        public ProgressForm(int total)
        {
            InitializeComponent();
            label7.Text = total.ToString();
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 更新进度条
        /// </summary>
        public void UpdateProgress(int success, int error, int total)
        {
            int sum = success + error;
            int percent = sum * 100 / total;
            label5.Text = success.ToString();
            label6.Text = error.ToString();
            label8.Text = percent + "%";
            progressBar1.Value = percent > 100 ? 100 : percent;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PicValue.ExitNow = true;
            button1.Enabled = false;
        }

        private void ProgressForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormsControl.ProgressForm = null;
            this.Dispose();
        }
    }
}