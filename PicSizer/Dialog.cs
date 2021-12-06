using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSizer
{
    public static class Dialog
    {
        private const string _Title = "PicSizer";
        private const string _Error = "错误";

        /// <summary>
        /// 显示弹窗
        /// </summary>
        public static void ShowDialog(string msg)
        {
            MessageBox.Show(msg, _Title);
        }

        /// <summary>
        /// 弹出错误提示框
        /// </summary>
        public static void ShowDialog_Error(string msg)
        {
            MessageBox.Show(msg, _Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 弹出警告框
        /// </summary>
        public static void ShowDialog_Warning(string msg)
        {
            MessageBox.Show(msg, _Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 弹出错误框
        /// </summary>
        public static void ShowDialog_Exception(Exception e)
        {
            ShowDialog_Error(e.ToString());
        }

        /// <summary>
        /// 弹出确认框
        /// </summary>
        public static bool ShowDialog_OKDialog(string msg)
        {
            return MessageBox.Show(msg, _Title, MessageBoxButtons.OKCancel) == DialogResult.OK;
        }
    }
}
