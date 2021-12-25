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
        const char EMPTY = (char)0;//空字符
        private bool isBrightnessChange = true;

        public static SettingForm form;

        public SettingForm()
        {
            InitializeComponent();
            comboBox3.SelectedIndex = 0;
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            //初始化各个控件的初始值
            comboBox1.SelectedIndex = Setting.resizeMode.ToInt();
            comboBox2.SelectedIndex = Setting.compressionMode.ToInt();
            comboBox4.SelectedIndex = Setting.renameMode.ToInt();
            comboBox5.SelectedIndex = Setting.extensionMode.ToInt();
            comboBox6.SelectedIndex = Setting.doWhenException.ToInt();
            numericUpDown1.Value = Setting.LimitWidth;
            numericUpDown2.Value = Setting.LimitHeight;
            numericUpDown3.Value = Setting.CompressionValue;
            numericUpDown4.Value = Setting.LimitSize;
            numericUpDown5.Value = Setting.StartIndex;
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            //仅限输入数字
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
            //限制最大大小为 1GB
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
                    Dialog.ShowDialog_Error("自定义命名中必须出现\"{0}\"以替换成数字");
                    return;
                }
            }

            Setting.resizeMode = (ResizeMode)comboBox1.SelectedIndex;
            Setting.compressionMode = (CompressionMode)comboBox2.SelectedIndex;
            Setting.renameMode = (RenameMode)comboBox4.SelectedIndex;
            Setting.extensionMode = (ExtensionMode)comboBox5.SelectedIndex;
            Setting.doWhenException = (DoWhenException)comboBox6.SelectedIndex;

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

            Setting.brightness = (byte)trackBar1.Value;

            form.Hide();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (isBrightnessChange)
            {
                isBrightnessChange = false;
                numericUpDown6.Value = trackBar1.Value;
                isBrightnessChange = true;
            }
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if (isBrightnessChange)
            {
                isBrightnessChange = false;
                trackBar1.Value = (int)numericUpDown6.Value;
                isBrightnessChange = true;
            }
        }
    }
}
