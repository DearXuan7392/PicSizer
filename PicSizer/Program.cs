using PicSizer.Window;
using System;
using System.Diagnostics;
using System.Runtime;
using System.Windows.Forms;

namespace PicSizer
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //载入dll
            FileIO.DLL.LoadDll();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Window.Forms.MainForm());

        }
    }
}
