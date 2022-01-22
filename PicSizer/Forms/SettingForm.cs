using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PicSizer.Partial;

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
            this.Icon = Info.icon;
            CheckForIllegalCrossThreadCalls = false;
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            LoadSetting(SharedVariable.setting);
        }

        private void LoadSetting(Setting setting)
        {
            //压缩
            comboBox_CompressionMode.SelectedIndex = setting.compressionMode.ToInt();//压缩模式
            numericUpDown_Value.Value = setting.CompressionValue;//指定画质

            long size = setting.LimitSize;
            if (size > 1024)
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
            comboBox_ResizeMode.SelectedIndex = setting.resizeMode.ToInt();//尺寸修正
            numericUpDown_LimitWidth.Value = setting.LimitWidth;
            numericUpDown_LimitHeight.Value = setting.LimitHeight;

            //命名
            comboBox_RenameMode.SelectedIndex = setting.renameMode.ToInt();//命名方式
            comboBox_ExtensionMode.SelectedIndex = setting.extensionMode.ToInt();//指定后缀
            numericUpDown_StartIndex.Value = setting.StartIndex;//起始下表
            textBox_CustomRenameStr.Text = setting.CustomRenameStr;//自定名称

            //其它
            comboBox_DoWhenException.SelectedIndex = setting.doWhenException.ToInt();//异常处理
            checkBox_AllowAnyExtension.Checked = setting.AllowAnyExtension;//允许任意后缀
            checkBox_TopMost.Checked = setting.topMost;//置顶
            numericUpDown_Threads.Value = setting.maxThreads;//最大线程数

            //图像处理
            trackBar_Brightness.Value = setting.brightness;//亮度
            checkBox_UseGPU.Checked = setting.useGPU;//硬件加速
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

        private void button_Save_Click(object sender, EventArgs e)
        {
            string fileNameError = CheckCustomName();
            if(fileNameError != null)
            {
                Dialog.ShowDialog_Error(fileNameError);
                return;
            }
            SharedVariable.setting = SaveSetting();
            FileCheck.SetImageCodeInfo(SharedVariable.setting.extensionMode);

            this.Hide();
        }

        private Setting SaveSetting()
        {
            Setting setting = new Setting()
            {
                //压缩
                compressionMode = (CompressionMode)comboBox_CompressionMode.SelectedIndex,//压缩模式
                CompressionValue = (long)numericUpDown_Value.Value,//指定画质
                LimitSize = comboBox_KB_or_MB.SelectedIndex == 0 ? (long)numericUpDown_Size.Value : (long)numericUpDown_Size.Value * 1024,//指定大小

                //尺寸
                resizeMode = (ResizeMode)comboBox_ResizeMode.SelectedIndex,//尺寸修正
                LimitWidth = (int)numericUpDown_LimitWidth.Value,
                LimitHeight = (int)numericUpDown_LimitHeight.Value,

                //命名
                renameMode = (RenameMode)comboBox_RenameMode.SelectedIndex,//命名方式
                extensionMode = (ExtensionMode)comboBox_ExtensionMode.SelectedIndex,//指定后缀
                StartIndex = (int)numericUpDown_StartIndex.Value,//起始下表
                CustomRenameStr = textBox_CustomRenameStr.Text,//自定名称

                //其它
                doWhenException = (DoWhenException)comboBox_DoWhenException.SelectedIndex,//异常处理
                AllowAnyExtension = checkBox_AllowAnyExtension.Checked,//允许任意后缀
                topMost = checkBox_TopMost.Checked,//置顶
                maxThreads = (int)numericUpDown_Threads.Value,

                //图像处理
                brightness = (byte)trackBar_Brightness.Value,//亮度
                useGPU = checkBox_UseGPU.Enabled && checkBox_UseGPU.Checked//硬件加速
            };
            return setting;
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

        private void button_Export_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Title = "导出配置",
                Filter = "配置文件(PICS)|*.pics|所有|*.*",
                FileName = "set.pics",
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SettingIO.WriteSettingToFile(SaveSetting(), dialog.FileName);
            }
        }

        private void button_ReadSetting_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Title = "读取配置",
                Filter = "配置文件(PICS)|*.pics|所有|*.*",
            };
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                Setting setting = SettingIO.ReadSettingFromFile(dialog.FileName);
                if (setting != null)
                {
                    LoadSetting(setting);
                    Dialog.ShowDialog("导入成功.");
                }
            }
        }

        private void checkBox_TopMost_CheckedChanged(object sender, EventArgs e)
        {
            SetTopMost(checkBox_TopMost.Checked);
        }

        private void SetTopMost(bool flag)
        {
            if(SharedVariable.settingForm.TopMost != flag)
            {
                SharedVariable.settingForm.TopMost
                    = SharedVariable.dearXuan.TopMost
                    = SharedVariable.progressForm.TopMost
                    = SharedVariable.mainForm.TopMost
                    = flag;
            }
        }

        private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetTopMost(SharedVariable.setting.topMost);
        }

        private void SettingForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void SettingForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] paths = e.Data.GetData(DataFormats.FileDrop, false) as string[];
            if(paths?.Length != 1)
            {
                Dialog.ShowDialog_Error("请拖入配置文件,它的后缀名通常为\"" + Info.SettingFileExtension + "\"");
                return;
            }
            Setting setting = SettingIO.ReadSettingFromFile(paths[0]);
            if (setting != null)
            {
                LoadSetting(setting);
                Dialog.ShowDialog("导入成功.");
            }
        }

        private void checkBox_UseGPU_CheckedChanged(object sender, EventArgs e)
        {
            //如果GPU不支持
            if(!Info.isGPUSupport && checkBox_UseGPU.Checked)
            {
                checkBox_UseGPU.Checked = false;
            }
        }

        /// <summary>
        /// 检车自定义的文件名是否合法
        /// </summary>
        private string CheckCustomName()
        {
            if(comboBox_RenameMode.SelectedIndex == 2 && !textBox_CustomRenameStr.Text.Contains("{ori}") && !textBox_CustomRenameStr.Text.Contains("{num}"))//自定义文件名
            {
                return "自定义命名中必须出现\"{ori}\"或\"{num}\"";
            }
            return null;
        }

        private void comboBox_ExtensionMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox_ExtensionMode.SelectedIndex != ExtensionMode.JPEG.ToInt())
            {
                label_Warn.Visible = true;
            }
            else
            {
                label_Warn.Visible = false;
            }
        }
    }
}
