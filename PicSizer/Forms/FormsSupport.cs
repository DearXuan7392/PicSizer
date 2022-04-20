using PicSizer.Class.Partial;
using PicSizer.Class.Static;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSizer.Forms
{
    public static class FormsSupport
    {
        /// <summary>
        /// 设置置顶
        /// </summary>
        public static void SetTopMost(bool flag)
        {
            if (Value.settingForm.TopMost != flag)
            {
                Value.settingForm.TopMost
                    = Value.progressForm.TopMost
                    = Value.mainForm.TopMost
                    = Value.mainForm.listView1.topMost
                    = flag;
            }
        }

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
        /// 绑定数字输入控件和滑动控件
        /// </summary>
        public static void BindNumericAndTrack(NumericUpDown numeric, TrackBar track)
        {
            bool ValueChange = true;
            numeric.ValueChanged += (sender, args) =>
            {
                if (ValueChange)
                {
                    ValueChange = false;
                    track.Value = (int)numeric.Value;
                    ValueChange = true;
                }
            };
            track.ValueChanged += (sender, args) =>
            {
                if (ValueChange)
                {
                    ValueChange = false;
                    numeric.Value = track.Value;
                    ValueChange = true;
                }
            };
        }

        /// <summary>
        /// 把颜色转成byte数组
        /// </summary>
        public static byte[] ColorToBytes(Color color)
        {
            return new byte[]
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
