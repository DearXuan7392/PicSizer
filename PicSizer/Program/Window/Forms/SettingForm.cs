#region

using System;
using System.Windows.Forms;
using PicSizer.Program.Static;
using PicSizer.Program.Window.Partial;
using static PicSizer.Program.Static.PicUnit;

#endregion

namespace PicSizer.Program.Window.Forms
{
    public partial class SettingForm : PicBaseForm
    {
        public SettingForm()
        {
            InitializeComponent();
            //绑定滑动条和数字框
            Support.BindNumericAndTrack(numericUpDown_Threads, trackBar_Threads);
            Support.BindNumericAndTrack(numericUpDown_Brightness, trackBar_Brightness);
            Support.BindNumericAndTrack(numericUpDown_WatermarkTransparency, trackBar_WatermarkTransparency);
            //设置布局尺寸
            splitContainer1.SplitterDistance = splitContainer1.Width / 2 - 4;
            splitContainer2.SplitterDistance = splitContainer2.Width / 2 - 4;
            trackBar_Threads.Width = groupBox4.Width - 16;
            //设置滑动条和数字狂最大值
            numericUpDown_Threads.Maximum = PicValue.CpuCoresNum; //最大线程数
            trackBar_Threads.Maximum = PicValue.CpuCoresNum; //最大线程数
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            LoadSetting();
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
            if (comboBox_KB_or_MB.SelectedIndex == 0) //KB
            {
                numericUpDown_Size.Maximum = 1048576;
            }
            else //MB
            {
                numericUpDown_Size.Maximum = 1024;
            }
        }

        /// <summary>
        /// JPEG压缩模式被修改
        /// </summary>
        private void CompressionMode_JPEG_SelectedIndexChanged(object sender, EventArgs e)
        {
            //指定画质
            if (comboBox_CompressType.SelectedIndex == 0)
            {
                numericUpDown_Size.Enabled = comboBox_KB_or_MB.Enabled = false; //则文件大小控件不可用
                numericUpDown_Quality.Enabled = true; //画质控件可用
                checkBox_AcceptExceedPicture.Enabled = false; //"接受超出大小的文件"控件不可用
            }
            //指定大小
            else
            {
                numericUpDown_Size.Enabled = comboBox_KB_or_MB.Enabled = true; //文件大小控件可用
                numericUpDown_Quality.Enabled = false; //画质控件不可用
                checkBox_AcceptExceedPicture.Enabled = true; //"接受超出大小的文件"控件可
            }
        }

        /// <summary>
        /// 尺寸修正模式被修改
        /// </summary>
        private void comboBox_ResizeMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            numericUpDown_LimitWidth.Enabled =
                numericUpDown_LimitHeight.Enabled = comboBox_ResizeMode.SelectedIndex != 0;
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
                                = comboBox_WatermarkMode.SelectedIndex != WatermarkType.Non.ToInt();
        }

        /// <summary>
        /// 单击"置顶"按钮
        /// </summary>
        private void checkBox_TopMost_CheckedChanged(object sender, EventArgs e)
        {
            FormsControl.SetTopMost(checkBox_TopMost.Checked);
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormsControl.SetTopMost(PicSetting.TopMost);
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

        /// <summary>
        /// 单击"硬件加速"按钮
        /// </summary>
        private void checkBox_UseGPU_CheckedChanged(object sender, EventArgs e)
        {
            //如果GPU不支持
            if (!PicValue.IsGpuSupport && checkBox_UseGPU.Checked)
            {
                checkBox_UseGPU.Checked = false;
            }
        }

        /// <summary>
        /// 单击"颜色"按钮
        /// </summary>
        private void OnColorChoose(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Dialog.Show_ColorChooseDialog(label_BackgroundColor.BackColor);
        }

        /// <summary>
        /// 选择字体
        /// </summary>
        private void OnFontChoose(object sender, EventArgs e)
        {
            ((Label)sender).Font = Dialog.Show_FontChooseDialog(((Label)sender).Font);
        }

        /// <summary>
        /// 选择水印颜色
        /// </summary>
        private void OnWatermarkColorChoose(object sender, EventArgs e)
        {
            Label label = sender as Label;
            label.BackColor
                = label_WatermarkFont.ForeColor
                    = Dialog.Show_ColorChooseDialog(label.BackColor);
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        private void button_Save_Click(object sender, EventArgs e)
        {
            SaveSetting();
            Close();
        }

        /// <summary>
        /// 后缀格式被修改
        /// </summary>
        private void comboBox_ExtensionMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string extension = null;
            switch ((ExtensionType)comboBox_ExtensionMode.SelectedIndex)
            {
                case ExtensionType.Jpeg:
                    extension = ".jpg";
                    break;
                case ExtensionType.Png:
                    extension = ".png";
                    break;
                case ExtensionType.Webp:
                    extension = ".webp";
                    break;
                case ExtensionType.Origin:
                    extension = ".{ext}";
                    break;
            }

            string ori = textBox_OutputFilename.Text;
            int index = ori.IndexOf('.');
            if (index != -1)
            {
                textBox_OutputFilename.Text = ori.Substring(0, index) + extension;
            }
            else
            {
                textBox_OutputFilename.Text += extension;
            }
        }
    }
}