#region

using System;
using System.Windows.Forms;
using PicSizer.Program.FileIO;
using PicSizer.Program.Window.Forms;

#endregion

namespace PicSizer
{
    static class EntryPoint
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //载入dll
            Dll.LoadDll();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}