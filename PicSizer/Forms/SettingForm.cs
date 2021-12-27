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
            CheckForIllegalCrossThreadCalls = false;
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            //压缩
            comboBox_CompressionMode.SelectedIndex = Setting.compressionMode.ToInt();//压缩模式
            numericUpDown_Value.Value = Setting.CompressionValue;//指定画质

            long size = Setting.LimitSize;
            if(size > 1024)
            {
                size /= 1024;
                comboBox_KB_or_MB.SelectedIndex = 0;//KB
            }
            else
            {
                comboBox_KB_or_MB.SelectedIndex = 1;//MB
            }
            numericUpDown_Size.Value = size;//指定大小

            //尺寸
            comboBox_ResizeMode.SelectedIndex = Setting.resizeMode.ToInt();//尺寸修正
            numericUpDown_LimitWidth.Value = Setting.LimitWidth;
            numericUpDown_LimitHeight.Value = Setting.LimitHeight;

            //命名
            comboBox_RenameMode.SelectedIndex = Setting.renameMode.ToInt();//命名方式
            comboBox_ExtensionMode.SelectedIndex = Setting.extensionMode.ToInt();//指定后缀
            numericUpDown_StartIndex.Value = Setting.StartIndex;//起始下表
            textBox_CustomRenameStr.Text = Setting.CustomRenameStr;//自定名称

            //操作
            comboBox_DoWhenException.SelectedIndex = Setting.doWhenException.ToInt();//异常处理
            checkBox_AllowAnyExtension.Checked = Setting.AllowAnyExtension;//允许任意后缀

            //图像处理
            trackBar_Brightness.Value = Setting.brightness;//亮度
            checkBox_UseGPU.Checked = Setting.useGPU;//硬件加速
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
            if(comboBox_KB_or_MB.SelectedIndex == 0)//KB
            {
                numericUpDown_Size.Maximum = 1048576;
            }
            else//MB
            {
                numericUpDown_Size.Maximum = 1024;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox_RenameMode.SelectedIndex == 2 && !textBox_CustomRenameStr.Text.Contains("{0}"))
            {
                Dialog.ShowDialog_Error("自定义命名中必须出现\"{0}\"以替换成数字");
                return;
            }

            //压缩
            Setting.compressionMode = (CompressionMode)comboBox_CompressionMode.SelectedIndex;//压缩模式
            Setting.CompressionValue = (long)numericUpDown_Value.Value;//指定画质
            long size = (long)numericUpDown_Size.Value;
            if(comboBox_KB_or_MB.SelectedIndex == 1)
            {
                size *= 1024;
            }
            Setting.LimitSize = size;//指定大小

            //尺寸
            Setting.resizeMode = (ResizeMode)comboBox_ResizeMode.SelectedIndex;//尺寸修正
            Setting.LimitWidth = (int)numericUpDown_LimitWidth.Value;
            Setting.LimitHeight = (int)numericUpDown_LimitHeight.Value;

            //命名
            Setting.renameMode = (RenameMode)comboBox_RenameMode.SelectedIndex;//命名方式
            Setting.extensionMode = (ExtensionMode)comboBox_ExtensionMode.SelectedIndex;//指定后缀
            Setting.StartIndex = (int)numericUpDown_StartIndex.Value;//起始下表
            Setting.CustomRenameStr = textBox_CustomRenameStr.Text;//自定名称

            //操作
            Setting.doWhenException = (DoWhenException)comboBox_DoWhenException.SelectedIndex;//异常处理
            Setting.AllowAnyExtension = checkBox_AllowAnyExtension.Checked;//允许任意后缀

            //图像处理
            Setting.brightness = (byte)trackBar_Brightness.Value;//亮度
            Setting.useGPU = checkBox_UseGPU.Enabled && checkBox_UseGPU.Checked;//硬件加速

            form.Hide();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (isBrightnessChange)
            {
                isBrightnessChange = false;
                numericUpDown_Brightness.Value = trackBar_Brightness.Value;
                isBrightnessChange = true;
            }
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if (isBrightnessChange)
            {
                isBrightnessChange = false;
                trackBar_Brightness.Value = (int)numericUpDown_Brightness.Value;
                isBrightnessChange = true;
            }
        }

        private void comboBox_CompressionMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            numericUpDown_Size.Enabled = comboBox_KB_or_MB.Enabled = comboBox_CompressionMode.SelectedIndex == 0;
            numericUpDown_Value.Enabled = comboBox_CompressionMode.SelectedIndex == 1;
        }

        private void comboBox_ResizeMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            numericUpDown_LimitWidth.Enabled = numericUpDown_LimitHeight.Enabled = comboBox_ResizeMode.SelectedIndex != 0;
        }

        private void comboBox_RenameMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox_CustomRenameStr.Enabled = comboBox_RenameMode.SelectedIndex == 2;
            numericUpDown_StartIndex.Enabled = comboBox_RenameMode.SelectedIndex != 1;
        }
    }
}
