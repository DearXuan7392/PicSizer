using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using PicSizer.Class.Partial;
using PicSizer.Class.Static;

namespace PicSizer.Forms
{
    public partial class UpdateSaveForm : Form
    {
        private string url;
        private string path;
        private Thread _thread;
        private int percentage = -1; // 下载进度(百分制)
        private long alreadyDownloadBytes = 0; // 已下载大小(B)
        private long totalDownloadBytes = 0; // 总大小(B)
        private long downloadBytesInSecond = 0; // 一秒内下载大小(B)

        private bool ContinueDownload = true;

        private System.Timers.Timer timer = new System.Timers.Timer()
        {
            Interval = 1000
        };

        public UpdateSaveForm(string url, string path)
        {
            InitializeComponent();
            FormsSupport.ForbidAltF4(this);
            CheckForIllegalCrossThreadCalls = false;
            TopMost = Value.setting.topMost;
            this.url = url;
            this.path = path;
            timer.Elapsed += OnTimer;
        }

        private void UpdateSaveForm_Load(object sender, EventArgs e)
        {
            try
            {
                _thread = new Thread(DownloadFileAsync);
                _thread.Start();
            }
            catch (Exception exception)
            {
                //删除已下载文件
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                Dialog.ShowDialog_Exception(exception);
            }
        }

        private void DownloadFileAsync()
        {
            HttpWebResponse response = null;
            Stream stream = null;
            FileStream fileStream = null;
            const int OnceReadLength = 512; // 每次读取的字节长度
            try
            {
                HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(url);
                response = (HttpWebResponse) request.GetResponse();
                stream = response.GetResponseStream();
                fileStream = new FileStream(path, FileMode.Create);
                totalDownloadBytes = response.ContentLength;
                byte[] bytes = new byte[OnceReadLength];
                int osize = stream.Read(bytes, 0, OnceReadLength);
                timer.Start();
                UpdateProgress(); // 建立连接后立即刷新
                while (osize > 0 && ContinueDownload)
                {
                    alreadyDownloadBytes += osize;
                    downloadBytesInSecond += osize;
                    fileStream.Write(bytes, 0, osize);
                    UpdateProgress();
                    osize = stream.Read(bytes, 0, OnceReadLength);
                }
            }
            finally
            {
                fileStream?.Dispose();
                stream?.Dispose();
                response?.Dispose();
            }
            OnDownloadFinish();
        }

        /// <summary>
        /// 点击取消按钮
        /// </summary>
        private void OnStopClick(object sender, EventArgs e)
        {
            ContinueDownload = false;
        }

        /// <summary>
        /// 下载完成
        /// </summary>
        private void OnDownloadFinish()
        {
            this.timer.Dispose();
            this.Close();
            this.Dispose();
            //下载完成
            if (alreadyDownloadBytes == totalDownloadBytes)
            {
                if (checkBox_RunWhenFinish.Checked)
                {
                    Process.Start(path);
                    System.Environment.Exit(0);
                }
                else
                {
                    Dialog.ShowDialog("下载完成");
                }
            }
            //下载未完成
            else
            {
                //删除已下载文件
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        /// <summary>
        /// 将B转换为合适的单位
        /// </summary>
        private string SwitchUnit(long bytes)
        {
            if (bytes < 1024)
            {
                return $"{bytes} B"; // B
            }
            else if (bytes < 1048576)
            {
                return $"{bytes / 1024.0 : 0.00} KB"; // KB
            }
            else
            {
                return $"{bytes / 1048576.0 : 0.00} MB"; // MB
            }
        }

        /// <summary>
        /// 更新进度
        /// </summary>
        private void UpdateProgress()
        {
            int per = (int) (alreadyDownloadBytes * 100 / totalDownloadBytes);
            if (per != percentage)
            {
                percentage = per;
                progressBar1.Value = per;
                this.Text = $"正在下载更新...({per}%)";
            }
        }

        private void OnTimer(object sender, ElapsedEventArgs args)
        {
            label_Speed.Text = $"{SwitchUnit(downloadBytesInSecond)}ps / {SwitchUnit(totalDownloadBytes)}";
            downloadBytesInSecond = 0;
        }
    }
}