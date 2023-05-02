using PicSizer.Static;
using PicSizer.Window.Forms;
using PicSizer.Window.Partial;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicSizer.Window
{
    public static class FormsControl
    {
        public static MainForm MainForm { get; set; } = null;
        public static PicPreViewForm PicPreViewForm { get; set; } = null;
        public static ProgressForm ProgressForm { get; set; } = null;

        /// <summary>
        /// 当前打开的窗体列表
        /// </summary>
        public static List<Form> FormList = new List<Form>();

        public static void SetTopMost(bool topMost)
        {
            FormList.ForEach((form) =>
            {
                form.TopMost = topMost;
            });
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
            if(index == -1)
            {
                index = PicValue.picListView.SelectedItems[0].Index;
            }
            if(PicPreViewForm == null)
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
