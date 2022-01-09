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
        private bool isMaxThreadsChange = true;

        public SettingForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            //压缩
            comboBox_CompressionMode.SelectedIndex = Info.setting.compressionMode.ToInt();//压缩模式
            numericUpDown_Value.Value = Info.setting.CompressionValue;//指定画质

            long size = Info.setting.LimitSize;
            if(size > 1024)
            {
                size /= 1024;
                comboBox_KB_or_MB.SelectedIndex = 1;//MB
            }
            else
            {
                comboBox_KB_or_MB.SelectedIndex = 0;//KB
            }
            numericUpDown_Size.Value = size;//指定大小

            //尺寸
            comboBox_ResizeMode.SelectedIndex = Info.setting.resizeMode.ToInt();//尺寸修正
            numericUpDown_LimitWidth.Value = Info.setting.LimitWidth;
            numericUpDown_LimitHeight.Value = Info.setting.LimitHeight;

            //命名
            comboBox_RenameMode.SelectedIndex = Info.setting.renameMode.ToInt();//命名方式
            comboBox_ExtensionMode.SelectedIndex = Info.setting.extensionMode.ToInt();//指定后缀
            numericUpDown_StartIndex.Value = Info.setting.StartIndex;//起始下表
            textBox_CustomRenameStr.Text = Info.setting.CustomRenameStr;//自定名称

            //其它
            comboBox_DoWhenException.SelectedIndex = Info.setting.doWhenException.ToInt();//异常处理
            checkBox_AllowAnyExtension.Checked = Info.setting.AllowAnyExtension;//允许任意后缀
            checkBox_TopMost.Checked = SharedVariable.mainForm.TopMost;//置顶
            numericUpDown_Threads.Value = Info.setting.maxThreads;//最大线程数

            //图像处理
            trackBar_Brightness.Value = Info.setting.brightness;//亮度
            checkBox_UseGPU.Checked = Info.setting.useGPU;//硬件加速
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
            Info.setting.compressionMode = (CompressionMode)comboBox_CompressionMode.SelectedIndex;//压缩模式
            Info.setting.CompressionValue = (long)numericUpDown_Value.Value;//指定画质
            long size = (long)numericUpDown_Size.Value;
            if(comboBox_KB_or_MB.SelectedIndex == 1)
            {
                size *= 1024;
            }
            Info.setting.LimitSize = size;//指定大小

            //尺寸
            Info.setting.resizeMode = (ResizeMode)comboBox_ResizeMode.SelectedIndex;//尺寸修正
            Info.setting.LimitWidth = (int)numericUpDown_LimitWidth.Value;
            Info.setting.LimitHeight = (int)numericUpDown_LimitHeight.Value;

            //命名
            Info.setting.renameMode = (RenameMode)comboBox_RenameMode.SelectedIndex;//命名方式
            Info.setting.extensionMode = (ExtensionMode)comboBox_ExtensionMode.SelectedIndex;//指定后缀
            Info.setting.StartIndex = (int)numericUpDown_StartIndex.Value;//起始下表
            Info.setting.CustomRenameStr = textBox_CustomRenameStr.Text;//自定名称

            //其它
            Info.setting.doWhenException = (DoWhenException)comboBox_DoWhenException.SelectedIndex;//异常处理
            Info.setting.AllowAnyExtension = checkBox_AllowAnyExtension.Checked;//允许任意后缀
            SharedVariable.mainForm.TopMost
                = SharedVariable.settingForm.TopMost
                = SharedVariable.progressForm.TopMost
                = SharedVariable.dearXuan.TopMost
                = checkBox_TopMost.Checked;//置顶
            Info.setting.maxThreads = (int)numericUpDown_Threads.Value;

            //图像处理
            Info.setting.brightness = (byte)trackBar_Brightness.Value;//亮度
            Info.setting.useGPU = checkBox_UseGPU.Enabled && checkBox_UseGPU.Checked;//硬件加速

            this.Hide();
        }

        private void trackBar_Brightness_Scroll(object sender, EventArgs e)
        {
            if (isBrightnessChange)
            {
                isBrightnessChange = false;
                numericUpDown_Brightness.Value = trackBar_Brightness.Value;
                isBrightnessChange = true;
            }
        }

        private void numericUpDown_Brightness_ValueChanged(object sender, EventArgs e)
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

        private void numericUpDown_Threads_ValueChanged(object sender, EventArgs e)
        {
            if (isMaxThreadsChange)
            {
                isMaxThreadsChange = false;
                trackBar_Threads.Value = (int)numericUpDown_Threads.Value;
                isMaxThreadsChange = true;
            }
        }

        private void trackBar_Threads_Scroll(object sender, EventArgs e)
        {
            if (isMaxThreadsChange)
            {
                isMaxThreadsChange = false;
                numericUpDown_Threads.Value = trackBar_Threads.Value;
                isMaxThreadsChange = true;
            }
        }
    }
}
