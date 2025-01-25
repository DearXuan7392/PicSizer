#region

using System.Drawing;
using System.Windows.Forms;

#endregion

namespace PicSizer.Program.Window
{
    /// <summary>
    /// PicSizer窗体支持类
    /// </summary>
    public static class Support
    {
        /// <summary>
        /// 屏蔽Alt+F4,防止退出
        /// </summary>
        public static void ForbidAltF4(Form form)
        {
            form.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.F4 && e.Alt)
                {
                    e.Handled = true;
                }
            };
        }

        /// <summary>
        /// 绑定数字输入控件和滑动控件,让两者的Value属性保持相等
        /// </summary>
        public static void BindNumericAndTrack(NumericUpDown numeric, TrackBar track)
        {
            bool valueChange = true;
            numeric.ValueChanged += (sender, args) =>
            {
                if (valueChange)
                {
                    valueChange = false;
                    track.Value = (int)numeric.Value;
                    valueChange = true;
                }
            };
            track.ValueChanged += (sender, args) =>
            {
                if (valueChange)
                {
                    valueChange = false;
                    numeric.Value = track.Value;
                    valueChange = true;
                }
            };
        }

        /// <summary>
        /// 把颜色转成byte数组
        /// </summary>
        public static byte[] ColorToBytes(Color color)
        {
            return new[]
            {
                color.R,
                color.G,
                color.B
            };
        }

        /// <summary>
        /// 把byte数组转成颜色
        /// </summary>
        public static Color BytesToColor(byte[] bytes, byte alpha = 255)
        {
            return Color.FromArgb(
                alpha,
                bytes[0],
                bytes[1],
                bytes[2]);
        }
    }
}