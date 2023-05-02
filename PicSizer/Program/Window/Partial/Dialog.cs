using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Drawing;
using PicSizer.Window.Unit;
using PicSizer.Static;

namespace PicSizer.Window.Partial
{
    /// <summary>
    /// PicSizer对话框类
    /// </summary>
    public static partial class Dialog
    {
        /// <summary>
        /// 默认对话框标题
        /// </summary>
        private const string _Title = "PicSizer";

        /// <summary>
        /// 报错对话框标题
        /// </summary>
        private const string _Error = "错误";

        private static string _Dialog_String_Filter_PictureOnly =
            "常规格式|*.jpg;*.png;*.bmp;" + Extension.ExtensionTypeListToFilter();

        private static string _Dialog_String_Filter_All = _Dialog_String_Filter_PictureOnly + "|所有|*.*";

        /// <summary>
        /// 显示一般弹窗
        /// </summary>
        /// [MethodImpl(MethodImplOptions.Synchronized)]
        public static void ShowDialog(string msg)
        {
            MessageBox.Show(FormsControl.MainForm, msg, _Title);
            SetFocus();
        }

        /// <summary>
        /// 显示压缩结束弹窗
        /// </summary>
        /// <param name="total">总压缩图片数量</param>
        /// <param name="success">压缩成功的数量</param>
        public static void ShowDialog_ResizeFinish(int total, int success)
        {
            string s = 
                $"总共: {total} 张\n" +
                $"压缩完成: {success} 张\n" +
                $"未完成: {(total - success)} 张";
            FormsControl.MainForm.Invoke(new Action(() =>
            {
                MessageBox.Show(FormsControl.MainForm, s, "压缩已结束", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        /// <summary>
        /// 弹出错误提示框
        /// </summary>
        public static void ShowDialog_Error(string msg)
        {
            MessageBox.Show(FormsControl.MainForm, msg, _Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 弹出警告框
        /// </summary>
        public static void ShowDialog_Warning(string msg)
        {
            MessageBox.Show(FormsControl.MainForm, msg, _Error, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            return MessageBox.Show(FormsControl.MainForm, msg, _Title, MessageBoxButtons.OKCancel) == DialogResult.OK;
        }

        /// <summary>
        /// 弹出重试取消对话框
        /// </summary>
        public static DialogResult ShowDialog_RetryCancel(string msg)
        {
            return MessageBox.Show(FormsControl.MainForm, msg, _Title, MessageBoxButtons.RetryCancel);
        }

        /// <summary>
        /// 显示打开文件对话框并返回打开的文件
        /// </summary>
        /// <returns>返回文件路径或null</returns>
        public static string[] Show_OpenFileDialog()
        {
            //初始化对话框
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "添加图片";
            dialog.Filter = _Dialog_String_Filter_PictureOnly;
            //允许多选
            dialog.Multiselect = true;
            //点击了确定
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileNames;
            }

            return null;
        }

        /// <summary>
        /// 打开文件夹并返回文件夹路径
        /// </summary>
        /// <returns>文件夹路径或null</returns>
        public static string Show_FolderBrowserDialog()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选择文件夹";
            //点击确定
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //路径为空
                if (string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    Dialog.ShowDialog_Error("路径不能为空!");
                    return null;
                }

                //返回文件夹路径
                return dialog.SelectedPath;
            }

            //返回空
            return null;
        }

        /// <summary>
        /// 将新版本下载到硬盘
        /// </summary>
        public static string Show_SaveOnDiskDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "选择保存路径";
            dialog.DefaultExt = "txt";
            dialog.Filter = "可执行文件|*.exe";
            dialog.FileName = "PicSizer.exe";
            //点击了确定
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileNames[0];
            }

            return null;
        }

        /// <summary>
        /// 弹出选择颜色对话框
        /// </summary>
        public static Color Show_ColorChooseDialog(Color color)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.Color = color;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.Color;
            }
            else
            {
                return color;
            }
        }

        /// <summary>
        /// 弹出选择字体对话框
        /// </summary>
        public static Font Show_FontChooseDialog(Font font)
        {
            FontDialog dialog = new FontDialog();
            dialog.Font = font;
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.Font;
            }
            else
            {
                return font;
            }
        }

        /// <summary>
        /// 为主窗体设置焦点
        /// </summary>
        private static void SetFocus()
        {
            if(FormsControl.MainForm != null)
            {
                FormsControl.MainForm.Focus();
            }
        }

        /// <summary>
        /// 打开url链接
        /// </summary>
        public static void OpenLink(string link)
        {
            try
            {
                System.Diagnostics.Process.Start(link);
            }
            catch (Exception)
            {
                MessageBox.Show(FormsControl.MainForm, $"打开浏览器失败，请将链接 {link} 复制到浏览器打开.", _Title,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}