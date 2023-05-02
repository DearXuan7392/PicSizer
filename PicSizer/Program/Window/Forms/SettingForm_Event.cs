using PicSizer.Static;
using PicSizer.Window.Partial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PicSizer.Static.PicUnit;

namespace PicSizer.Window.Forms
{
    public partial class SettingForm
    {
        /// <summary>
        /// 加载设置
        /// </summary>
        private void LoadSetting()
        {
            //压缩
            comboBox_CompressType.SelectedIndex = PicSetting.CompressType.ToInt();
            numericUpDown_Quality.Value = PicSetting.Strength;//指定画质

            long size = PicSetting.LimitSize;
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
            comboBox_ResizeMode.SelectedIndex = PicSetting.ResizeType.ToInt();//尺寸修正
            numericUpDown_LimitWidth.Value = PicSetting.LimitWidth;
            numericUpDown_LimitHeight.Value = PicSetting.LimitHeight;

            //命名
            comboBox_ExtensionMode.SelectedIndex = PicSetting.ExtensionType.ToInt();//后缀格式
            numericUpDown_StartIndex.Value = PicSetting.OutputIndex;//起始下表
            textBox_OutputFilename.Text = PicSetting.OutputFilename;//自定名称

            //其它
            checkBox_AcceptExceedPicture.Checked = PicSetting.AcceptExceedPicture;//是否接受超出限制的文件
            checkBox_TopMost.Checked = PicSetting.TopMost;//置顶
            numericUpDown_Threads.Value = PicSetting.MaxThreads;//压缩线程数

            //图像处理
            trackBar_Brightness.Value = PicSetting.Brightness;//亮度
            checkBox_UseGPU.Checked = PicSetting.UseGPU;//硬件加速
            label_BackgroundColor.BackColor = Support.BytesToColor(PicSetting.BackgroundColor);//背景色

            //水印
            comboBox_WatermarkMode.SelectedIndex = PicSetting.WatermarkType.ToInt();
            label_WatermarkColor.BackColor = label_WatermarkFont.ForeColor
                = Support.BytesToColor(PicSetting.WatermarkColor);//水印颜色
            label_WatermarkFont.Font = PicSetting.WatermarkFont;//水印字体
            numericUpDown_WatermarkTransparency.Value = PicSetting.WatermarkAlpha;//水印不透明度
            textBox_WatermarkText.Text = PicSetting.WatermarkText;//水印文本

            //硬件加速
            if(PicValue.IsGPUSupport)
            {
                checkBox_UseGPU.Enabled = true;
                checkBox_UseGPU.Checked = PicSetting.UseGPU;
                string ToolTipStr = $"new string(PicValue.picDllResult.GpuName).TrimEnd(Convert.ToChar(0))\n" +
                    $"ThreadsPerBlock:{PicValue.picDllResult.GpuMaxThreadsPerBlock}\n" +
                    $"GridNum:{PicValue.picDllResult.GpuGrid[0]},{PicValue.picDllResult.GpuGrid[1]},{PicValue.picDllResult.GpuGrid[2]}";
                toolTip_GPU.SetToolTip(checkBox_UseGPU, ToolTipStr);
            }
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        private void SaveSetting()
        {
            //压缩
            PicSetting.CompressType = (CompressType)comboBox_CompressType.SelectedIndex;//JPEG压缩模式
            PicSetting.Strength = (int)numericUpDown_Quality.Value;//指定画质
            PicSetting.LimitSize = comboBox_KB_or_MB.SelectedIndex == 0 ? (long)numericUpDown_Size.Value : (long)numericUpDown_Size.Value * 1024;//指定大小

            //尺寸
            PicSetting.ResizeType = (ResizeType)comboBox_ResizeMode.SelectedIndex;//尺寸修正
            PicSetting.LimitWidth = (int)numericUpDown_LimitWidth.Value;
            PicSetting.LimitHeight = (int)numericUpDown_LimitHeight.Value;

            //命名
            PicSetting.ExtensionType = (ExtensionType)comboBox_ExtensionMode.SelectedIndex;//后缀格式
            PicSetting.OutputIndex = (int)numericUpDown_StartIndex.Value;//起始下表
            PicSetting.OutputFilename = textBox_OutputFilename.Text;//自定名称

            //其它
            PicSetting.AcceptExceedPicture = checkBox_AcceptExceedPicture.Checked;//是否接受超出限制的文件
            PicSetting.TopMost = checkBox_TopMost.Checked;//置顶
            PicSetting.MaxThreads = (int)numericUpDown_Threads.Value;

            //图像处理
            PicSetting.Brightness = (byte)trackBar_Brightness.Value;//亮度
            PicSetting.BackgroundColor = Support.ColorToBytes(label_BackgroundColor.BackColor);//背景色

            //水印
            PicSetting.WatermarkType = (WatermarkType)comboBox_WatermarkMode.SelectedIndex;
            PicSetting.WatermarkColor = Support.ColorToBytes(label_WatermarkColor.BackColor);//水印颜色
            PicSetting.WatermarkFont = label_WatermarkFont.Font;//水印字体
            PicSetting.WatermarkAlpha = (byte)numericUpDown_WatermarkTransparency.Value;//水印不透明度
            PicSetting.WatermarkText = textBox_WatermarkText.Text;//水印文本

            //硬件加速
            PicSetting.UseGPU = PicValue.IsGPUSupport && checkBox_UseGPU.Checked;
        }
    }
}
