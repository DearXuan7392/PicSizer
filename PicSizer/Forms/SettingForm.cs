using PicSizer.Class.Partial;
using PicSizer.Class.Static;
using System;
using System.Windows.Forms;

namespace PicSizer.Forms
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
            this.Icon = Info.icon;
            CheckForIllegalCrossThreadCalls = false;
            Forms.FormsSupport.BindNumericAndTrack(numericUpDown_Threads, trackBar_Threads);
            Forms.FormsSupport.BindNumericAndTrack(numericUpDown_Brightness, trackBar_Brightness);
            Forms.FormsSupport.BindNumericAndTrack(numericUpDown_WatermarkTransparency, trackBar_WatermarkTransparency);
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            LoadSetting(Value.setting);
        }

        /// <summary>
        /// 加载设置
        /// </summary>
        private void LoadSetting(Setting setting)
        {
            //压缩
            comboBox_CompressMode_JPEG.SelectedIndex = setting.CompressionMode_Jpeg.ToInt();
            comboBox_CompressMode_ICON.SelectedIndex = setting.CompressionMode_Icon.ToInt();
            comboBox_CompressMode_Other.SelectedIndex = setting.CompressionMode_Other.ToInt();
            numericUpDown_Quality.Value = setting.Quality;//指定画质

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
            numericUpDown_IconWidth.Value = setting.IconLimitWidth;
            numericUpDown_IconHeight.Value = setting.IconLimitHeight;

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
            label_BackgroundColor.BackColor = FormsSupport.BytesToColor(setting.backgroundColor);//背景色
            
            //水印
            comboBox_WatermarkMode.SelectedIndex = setting.watermarkMode.ToInt();
            label_WatermarkColor.BackColor = label_WatermarkFont.ForeColor 
                = FormsSupport.BytesToColor(setting.watermarkColor);//水印颜色
            label_WatermarkFont.Font = setting.watermarkFont;//水印字体
            numericUpDown_WatermarkTransparency.Value = setting.watermarkAlpha;//水印不透明度
            textBox_WatermarkText.Text = setting.watermarkText;//水印文本
        }

        /// <summary>
        /// 限制只能输入数字
        /// </summary>
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            //仅限输入数字
            if (e.KeyChar < '0' || e.KeyChar > '9')
            {
                if (e.KeyChar != 8 && e.KeyChar != 127)
                {
                    e.KeyChar = (char)0;
                }
            }
        }

        /// <summary>
        /// KB或MB选项
        /// </summary>
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

        /// <summary>
        /// 点击"保存"按钮
        /// </summary>
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

        /// <summary>
        /// 从当前配置中获取Setting对象
        /// </summary>
        private Setting SaveSetting()
        {
            Setting setting = new Setting()
            {
                //压缩
                CompressionMode_Jpeg = (CompressionMode_JPEG)comboBox_CompressMode_JPEG.SelectedIndex,//JPEG压缩模式
                CompressionMode_Icon = (CompressionMode_ICON)comboBox_CompressMode_ICON.SelectedIndex,//ICON压缩模式
                CompressionMode_Other = (CompressionMode_Other)comboBox_CompressMode_Other.SelectedIndex,//其它图片压缩模式
                Quality = (long)numericUpDown_Quality.Value,//指定画质
                LimitSize = comboBox_KB_or_MB.SelectedIndex == 0 ? (long)numericUpDown_Size.Value : (long)numericUpDown_Size.Value * 1024,//指定大小

                //尺寸
                resizeMode = (ResizeMode)comboBox_ResizeMode.SelectedIndex,//尺寸修正
                LimitWidth = (int)numericUpDown_LimitWidth.Value,
                LimitHeight = (int)numericUpDown_LimitHeight.Value,
                IconLimitWidth = (byte)numericUpDown_IconWidth.Value,
                IconLimitHeight = (byte)numericUpDown_IconHeight.Value,

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
                backgroundColor = FormsSupport.ColorToBytes(label_BackgroundColor.BackColor),//背景色
                
                //水印
                watermarkMode = (WatermarkMode)comboBox_WatermarkMode.SelectedIndex,
                watermarkColor = FormsSupport.ColorToBytes(label_WatermarkColor.BackColor),//水印颜色
                watermarkFont = label_WatermarkFont.Font,//水印字体
                watermarkAlpha = (byte)numericUpDown_WatermarkTransparency.Value,//水印不透明度
                watermarkText = textBox_WatermarkText.Text//水印文本
            };
            return setting;
        }

        /// <summary>
        /// JPEG压缩模式被修改
        /// </summary>
        private void CompressionMode_JPEG_SelectedIndexChanged(object sender, EventArgs e)
        {
            //指定大小
            if (comboBox_CompressMode_JPEG.SelectedIndex == 0)
            {
                numericUpDown_Size.Enabled = comboBox_KB_or_MB.Enabled = true;//文件大小控件可用
                numericUpDown_Quality.Enabled = false;//画质控件不可用
                checkBox_AcceptExceedPicture.Enabled = true;//"接受超出大小的文件"控件可
            }
            //指定画质
            else
            {
                if (comboBox_ExtensionMode.SelectedIndex == ExtensionMode.JPEG.ToInt())
                {
                    //如果指定JPEG格式,且指定画质压缩,则文件大小控件不可用
                    numericUpDown_Size.Enabled = comboBox_KB_or_MB.Enabled = false;
                }
                numericUpDown_Quality.Enabled = true;//画质控件可用
                checkBox_AcceptExceedPicture.Enabled = false;//"接受超出大小的文件"控件不可用
            }
        }

        /// <summary>
        /// 尺寸修正模式被修改
        /// </summary>
        private void comboBox_ResizeMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            numericUpDown_LimitWidth.Enabled = numericUpDown_LimitHeight.Enabled = comboBox_ResizeMode.SelectedIndex != 0;
        }

        /// <summary>
        /// 命名格式被修改
        /// </summary>
        private void comboBox_RenameMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox_CustomRenameStr.Enabled = comboBox_RenameMode.SelectedIndex == 2;
            numericUpDown_StartIndex.Enabled = comboBox_RenameMode.SelectedIndex != 1;
        }

        /// <summary>
        /// 水印模式被修改
        /// </summary>
        private void comboBox_WatermarkMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            numericUpDown_WatermarkTransparency.Enabled
                = trackBar_WatermarkTransparency.Enabled
                = label_WatermarkColor.Enabled
                = label_WatermarkFont.Enabled
                = textBox_WatermarkText.Enabled
                = comboBox_WatermarkMode.SelectedIndex != WatermarkMode.Non.ToInt();
        }

        /// <summary>
        /// 单击"导出"按钮
        /// </summary>
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

        /// <summary>
        /// 单击"读取"按钮
        /// </summary>
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

        /// <summary>
        /// 单击"置顶"按钮
        /// </summary>
        private void checkBox_TopMost_CheckedChanged(object sender, EventArgs e)
        {
            Forms.FormsSupport.SetTopMost(checkBox_TopMost.Checked);
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Forms.FormsSupport.SetTopMost(Value.setting.topMost);
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

        /// <summary>
        /// 单击"硬件加速"按钮
        /// </summary>
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

        /// <summary>
        /// 单击"颜色"按钮
        /// </summary>
        private void OnColorChoose(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Dialog.Show_ColorChooseDialog(label_BackgroundColor.BackColor);
        }

        /// <summary>
        /// 修改后缀格式
        /// </summary>
        private void comboBox_ExtensionMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExtensionMode extension = (ExtensionMode)comboBox_ExtensionMode.SelectedIndex;
            switch (extension)
            {
                case ExtensionMode.Original:
                    //全部设为可用
                    comboBox_CompressMode_JPEG.Enabled = comboBox_CompressMode_ICON.Enabled = comboBox_CompressMode_Other.Enabled = true;
                    //模拟按下JPEG压缩模式选项
                    CompressionMode_JPEG_SelectedIndexChanged(null, null);
                    break;
                case ExtensionMode.JPEG:
                    //部分可用
                    comboBox_CompressMode_JPEG.Enabled = true;
                    comboBox_CompressMode_ICON.Enabled = comboBox_CompressMode_Other.Enabled = false;
                    numericUpDown_IconWidth.Enabled = numericUpDown_IconHeight.Enabled = false;
                    //指定大小窗格可用
                    numericUpDown_Size.Enabled = comboBox_KB_or_MB.Enabled = true;
                    //模拟按下JPEG压缩模式选项
                    CompressionMode_JPEG_SelectedIndexChanged(null, null);
                    break;
                case ExtensionMode.ICON:
                    //部分可用
                    comboBox_CompressMode_ICON.Enabled = true;
                    comboBox_CompressMode_JPEG.Enabled = comboBox_CompressMode_Other.Enabled = false;
                    numericUpDown_IconWidth.Enabled = numericUpDown_IconHeight.Enabled = true;
                    //指定大小窗格可用
                    numericUpDown_Size.Enabled = comboBox_KB_or_MB.Enabled = true;
                    break;
                default:
                    //部分可用
                    comboBox_CompressMode_Other.Enabled = true;
                    comboBox_CompressMode_JPEG.Enabled = comboBox_CompressMode_ICON.Enabled = false;
                    numericUpDown_IconWidth.Enabled = numericUpDown_IconHeight.Enabled = false;
                    //指定大小窗格可用
                    numericUpDown_Size.Enabled = comboBox_KB_or_MB.Enabled = true;
                    break;
            }
        }

        private void OnFontChoose(object sender, EventArgs e)
        {
            ((Label)sender).Font = Dialog.Show_FontChooseDialog(((Label)sender).Font);
        }

        private void OnWatermarkColorChoose(object sender, EventArgs e)
        {
            Label label = sender as Label;
            label.BackColor 
                = label_WatermarkFont.ForeColor
                = Dialog.Show_ColorChooseDialog(label.BackColor);
        }
    }
}
