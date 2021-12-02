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
    public partial class SettingForm : Form
    {
        const char EMPTY = (char)0;

        public static SettingForm form;

        public SettingForm()
        {
            InitializeComponent();
            comboBox3.SelectedIndex = 0;
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = Setting.resizeMode.ToInt();
            comboBox2.SelectedIndex = Setting.compressionMode.ToInt();
            comboBox4.SelectedIndex = Setting.renameMode.ToInt();
            comboBox5.SelectedIndex = Setting.extensionMode.ToInt();
            numericUpDown1.Value = Setting.LimitWidth;
            numericUpDown2.Value = Setting.LimitHeight;
            numericUpDown3.Value = Setting.CompressionValue;
            numericUpDown4.Value = Setting.LimitSize;
            numericUpDown5.Value = Setting.StartIndex;
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar < '0' || e.KeyChar > '9')
            {
                if(e.KeyChar != 8 && e.KeyChar != 127)
                {
                    e.KeyChar = EMPTY;
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox3.SelectedIndex == 0)//KB
            {
                numericUpDown4.Maximum = 1048576;
            }
            else//MB
            {
                numericUpDown4.Maximum = 1024;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox4.SelectedIndex == 2)
            {
                if (!textBox1.Text.Contains("{0}"))
                {
                    MessageBox.Show("自定义命名中必须出现\"{0}\"以替换成数字", "PicSizer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            Setting.resizeMode = (ResizeMode)comboBox1.SelectedIndex;
            Setting.compressionMode = (CompressionMode)comboBox2.SelectedIndex;
            Setting.renameMode = (RenameMode)comboBox4.SelectedIndex;
            Setting.extensionMode = (ExtensionMode)comboBox5.SelectedIndex;

            Setting.LimitWidth = (int)numericUpDown1.Value;
            Setting.LimitHeight = (int)numericUpDown2.Value;
            Setting.CompressionValue = (long)numericUpDown3.Value;

            long size = (long)numericUpDown4.Value;
            if(comboBox3.SelectedIndex == 1)
            {
                size *= 1024;
            }
            Setting.LimitSize = size;

            Setting.StartIndex = (int)numericUpDown5.Value;

            Setting.CustomRenameStr = textBox1.Text;

            form.Hide();
        }
    }
}
