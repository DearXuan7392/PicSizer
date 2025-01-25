#region

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PicSizer.Program.Static;
using PicSizer.Program.Window.Forms;

#endregion

namespace PicSizer.Program.Window
{
    public static class FormsControl
    {
        public static MainForm MainForm { get; set; } = null;
        public static PicPreViewForm PicPreViewForm { get; set; }
        public static ProgressForm ProgressForm { get; set; }

        /// <summary>
        /// 当前打开的窗体列表
        /// </summary>
        public static readonly List<Form> FormList = new List<Form>();

        public static void SetTopMost(bool topMost)
        {
            FormList.ForEach(form => { form.TopMost = topMost; });
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ShowAboutForm()
        {
            (new AboutForm()).ShowDialog(MainForm);
        }

        public static void ShowPicPreViewForm(int index = -1)
        {
            if (index == -1)
            {
                index = PicValue.PicListView.SelectedItems[0].Index;
            }

            if (PicPreViewForm == null)
            {
                PicPreViewForm = new PicPreViewForm();
                PicPreViewForm.Show(MainForm);
            }

            PicPreViewForm.UpdatePreviewPicture(index);
        }

        public static void ShowTempBitmapForm(ref Bitmap bitmap)
        {
            PicPreViewForm?.Dispose();
            PicPreViewForm = new PicPreViewForm();
            PicPreViewForm.ShowTempBitmap(ref bitmap);
            PicPreViewForm.ShowDialog(MainForm);
        }

        public static void ShowProgressForm(int total)
        {
            ProgressForm = new ProgressForm(total);
            ProgressForm.ShowDialog(MainForm);
        }

        public static void ShowSettingForm()
        {
            (new SettingForm()).ShowDialog(MainForm);
        }
    }
}