using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSizer.Partial
{
    public static class Dialog
    {
        private const string _Title = "PicSizer";
        private const string _Error = "错误";

        /// <summary>
        /// 显示弹窗
        /// </summary>
        /// [MethodImpl(MethodImplOptions.Synchronized)]
        public static void ShowDialog(string msg)
        {
            MessageBox.Show(Value.mainForm, msg, _Title);
            SetFocus();
        }

        public static void ShowDialog_ResizeFinish(int total, int success)
        {
            string s = "总共: " + total + " 张\n压缩完成: " + success + "张\n未完成: " + (total - success) + "张";
            MessageBox.Show(Value.mainForm, s, "压缩已结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SetFocus();
        }

        /// <summary>
        /// 弹出错误提示框
        /// </summary>
        public static void ShowDialog_Error(string msg)
        {
            MessageBox.Show(Value.mainForm, msg, _Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 弹出警告框
        /// </summary>
        public static void ShowDialog_Warning(string msg)
        {
            MessageBox.Show(Value.mainForm, msg, _Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 弹出错误框
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void ShowDialog_Exception(Exception e)
        {
            ShowDialog_Error(e.ToString());
        }

        /// <summary>
        /// 弹出确认框
        /// </summary>
        public static bool ShowDialog_OKDialog(string msg)
        {
            return MessageBox.Show(Value.mainForm, msg, _Title, MessageBoxButtons.OKCancel) == DialogResult.OK;
        }

        /// <summary>
        /// 弹出版本错误框
        /// </summary>
        public static bool ShowDialog_VersionError(SettingIO.SettingFilePrefix prefix)
        {
            string s = "不匹配的文件版本!\n\n该配置文件对应的版本是: "+ prefix.PicSizerVersion.ToString() + "\n" +
                "而您的版本是: " + Info.ProjectVersion.ToString() + "\n\n" +
                "继续加载可能会引发错误，仍然要加载吗?";
            return ShowDialog_OKDialog(s);
        }

        /// <summary>
        /// 显示保存失败对话框
        /// </summary>
        public static void ShowDialog_SavingFailed()
        {
            ShowDialog_Error("保存失败.");
        }

        /// <summary>
        /// 弹出打开配置文件失败对话框
        /// </summary>
        public static void ShowDialog_OpenSettingFailed()
        {
            ShowDialog_Error("无法打开这个文件,请确保文件有效且版本正确.");
        }

        /// <summary>
        /// 显示打开文件对话框
        /// </summary>
        public static OpenFileDialog GetOpenFileDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "添加图片";
            if (Value.setting.AllowAnyExtension)
            {
                dialog.Filter = "图片(JPG,PNG,BMP,TIFF)|*.jpg;*.png;*.bmp;*.tiff|所有|*.*";
            }
            else
            {
                dialog.Filter = "图片(JPG,PNG,BMP,TIFF)|*.jpg;*.png;*.bmp;*.tiff";
            }
            dialog.Multiselect = true;
            return dialog;
        }

        public static FolderBrowserDialog GetFolderBrowserDialog()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选择文件夹";
            return dialog;
        }

        /// <summary>
        /// 获取焦点
        /// </summary>
        private static void SetFocus()
        {
            Value.mainForm.Focus();
        }

        public static void OpenLink(string link)
        {
            try
            {
                System.Diagnostics.Process.Start(link);
            }
            catch (Exception)
            {
                MessageBox.Show(Value.mainForm, "打开浏览器失败，请将链接\"" + link + "\"复制到浏览器打开.", "PicSizer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
