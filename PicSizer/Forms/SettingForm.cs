using PicSizer.Class.Partial;
using PicSizer.Class.Static;
using System;
using System.Windows.Forms;

namespace PicSizer
{
    public partial class SettingForm : Form
    {
        const char EMPTY = (char)0;//空字符

        public SettingForm()
        {
            InitializeComponent();
            this.Icon = Info.icon;
            CheckForIllegalCrossThreadCalls = false;
            Forms.Support.BindNumericAndTrack(numericUpDown_Threads, trackBar_Threads);
            Forms.Support.BindNumericAndTrack(numericUpDown_Brightness, trackBar_Brightness);
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            LoadSetting(Value.setting);
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
            comboBox_NonJPEGCompressMethod.SelectedIndex = setting.nonJEPGCompressMethod.ToInt();//非JPEG压缩方式

            //尺寸
            comboBox_ResizeMode.SelectedIndex = setting.resizeMode.ToInt();//尺寸修正
            numericUpDown_LimitWidth.Value = setting.LimitWidth;
            numericUpDown_LimitHeight.Value = setting.LimitHeight;
            numericUpDown_IconSize.Value = setting.IconLimitSize;

            //命名
            comboBox_RenameMode.SelectedIndex = setting.renameMode.ToInt();//命名方式
            comboBox_ExtensionMode.SelectedIndex = setting.extensionMode.ToInt();//指定后缀
            numericUpDown_StartIndex.Value = setting.StartIndex;//起始下表
            textBox_CustomRenameStr.Text = setting.CustomRenameStr;//自定名称

            //其它
            comboBox_DoWhenException.SelectedIndex = setting.doWhenException.ToInt();//异常处理
            checkBox_AcceptExceedPicture.Checked = setting.AcceptExceedPicture;//是否接受超出限制的文件
            checkBox_AllowAnyExtension.Checked = setting.AllowAnyExtension;//允许任意后缀
            checkBox_TopMost.Checked = setting.topMost;//置顶
            numericUpDown_Threads.Value = setting.maxThreads;//最大线程数

            //图像处理
            trackBar_Brightness.Value = setting.brightness;//亮度
            checkBox_UseGPU.Checked = setting.useGPU;//硬件加速
            label_BackgroundColor.BackColor = System.Drawing.Color.FromArgb(
                255,
                setting.backgroundColor[0],
                setting.backgroundColor[1],
                setting.backgroundColor[2]);//背景色
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            //仅限输入数字
            if (e.KeyChar < '0' || e.KeyChar > '9')
            {
                if (e.KeyChar != 8 && e.KeyChar != 127)
                {
                    e.KeyChar = EMPTY;
                }
            }
        }

        private void comboBox_KB_or_MB_SelectedIndexChanged(object sender, EventArgs e)
        {
            //限制最大大小为 1GB
            if (comboBox_KB_or_MB.SelectedIndex == 0)//KB
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
            if (fileNameError != null)
            {
                Dialog.ShowDialog_Error(fileNameError);
                return;
            }
            Value.setting = SaveSetting();
            Value.mainForm.listView1.AllowAnyExtension = Value.setting.AllowAnyExtension;
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
                nonJEPGCompressMethod = (NonJEPGCompressMethod)comboBox_NonJPEGCompressMethod.SelectedIndex,//非JPEG压缩方式

                //尺寸
                resizeMode = (ResizeMode)comboBox_ResizeMode.SelectedIndex,//尺寸修正
                LimitWidth = (int)numericUpDown_LimitWidth.Value,
                LimitHeight = (int)numericUpDown_LimitHeight.Value,
                IconLimitSize = (byte)numericUpDown_IconSize.Value,

                //命名
                renameMode = (RenameMode)comboBox_RenameMode.SelectedIndex,//命名方式
                extensionMode = (ExtensionMode)comboBox_ExtensionMode.SelectedIndex,//指定后缀
                StartIndex = (int)numericUpDown_StartIndex.Value,//起始下表
                CustomRenameStr = textBox_CustomRenameStr.Text,//自定名称

                //其它
                doWhenException = (DoWhenException)comboBox_DoWhenException.SelectedIndex,//异常处理
                AcceptExceedPicture = checkBox_AcceptExceedPicture.Checked,//是否接受超出限制的文件
                AllowAnyExtension = checkBox_AllowAnyExtension.Checked,//允许任意后缀
                topMost = checkBox_TopMost.Checked,//置顶
                maxThreads = (int)numericUpDown_Threads.Value,

                //图像处理
                brightness = (byte)trackBar_Brightness.Value,//亮度
                useGPU = checkBox_UseGPU.Enabled && checkBox_UseGPU.Checked,//硬件加速
                backgroundColor = new byte[]
                {
                    label_BackgroundColor.BackColor.R,
                    label_BackgroundColor.BackColor.G,
                    label_BackgroundColor.BackColor.B
                }
            };
            return setting;
        }

        private void comboBox_CompressionMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            //指定大小
            if (comboBox_CompressionMode.SelectedIndex == 0)
            {
                numericUpDown_Size.Enabled = comboBox_KB_or_MB.Enabled = true;//文件大小控件可用
                numericUpDown_Value.Enabled = false;//画质控件不可用
                checkBox_AcceptExceedPicture.Enabled = true;//"接受超出大小的文件"控件可用
                //指定大小时输出格式可以自定义
                comboBox_ExtensionMode.Enabled = true;//允许用户修改后缀
                comboBox_NonJPEGCompressMethod.Enabled = true;//非JPEG的压缩模式
                numericUpDown_IconSize.Enabled = true;//ICON的限定尺寸可用
            }
            //指定画质
            else
            {
                numericUpDown_Size.Enabled = comboBox_KB_or_MB.Enabled = false;//文件大小控件不可用
                numericUpDown_Value.Enabled = true;//画质控件可用
                checkBox_AcceptExceedPicture.Enabled = false;//"接受超出大小的文件"控件不可用
                //指定画质时输出格式必须是JPEG
                comboBox_ExtensionMode.SelectedIndex = 0;//选中JPEG
                comboBox_ExtensionMode.Enabled = false;//禁止用户修改后缀
                comboBox_NonJPEGCompressMethod.Enabled = false;//非JPEG的压缩模式
                numericUpDown_IconSize.Enabled = false;//ICON的限定尺寸不可用
            }


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
            if (dialog.ShowDialog() == DialogResult.OK)
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
            Forms.Support.SetTopMost(checkBox_TopMost.Checked);
        }

        private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Forms.Support.SetTopMost(Value.setting.topMost);
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
            if (paths?.Length != 1)
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
            if (!Info.isGPUSupport && checkBox_UseGPU.Checked)
            {
                checkBox_UseGPU.Checked = false;
            }
        }

        /// <summary>
        /// 检车自定义的文件名是否合法
        /// </summary>
        private string CheckCustomName()
        {
            if (comboBox_RenameMode.SelectedIndex == 2 && !textBox_CustomRenameStr.Text.Contains("{ori}") && !textBox_CustomRenameStr.Text.Contains("{num}"))//自定义文件名
            {
                return "自定义命名中必须出现\"{ori}\"或\"{num}\"";
            }
            return null;
        }

        private void OnColorChoose(object sender, EventArgs e)
        {
            label_BackgroundColor.BackColor = Dialog.Show_ColorChooseDialog(label_BackgroundColor.BackColor);
        }
    }
}
