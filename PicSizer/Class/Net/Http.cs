using System;
using System.Net;
using System.Text;
using PicSizer.Forms;

namespace PicSizer.Class.Partial
{
    public static class Http
    {
        /// <summary>
        /// 从指定url读取text
        /// </summary>
        /// <param name="url">指定url</param>
        /// <param name="delay">最大等待时间</param>
        public static string ReadTextFromUrl(string url, int delay = 1000)
        {
            PicWebClient client = null;
            try
            {
                client = new PicWebClient();
                client.TimeOut = delay;
                client.Encoding = Encoding.UTF8;
                return client.DownloadString(url);
            }
            finally
            {
                client?.Dispose();
            }
        }

        /// <summary>
        /// 将远程文件下载到本地硬盘
        /// </summary>
        public static void SaveFileToDisk(string url, string path)
        {
            new UpdateSaveForm(url, path).ShowDialog();
        }

        /// <summary>
        /// 发送反馈
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns></returns>
        public static bool SendStrToServer(string feedback)
        {
            return true;
        }
        
        private class PicWebClient : WebClient
        {
            public int TimeOut { get; set; }

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);
                request.Timeout = this.TimeOut;
                return request;
            }
        }
    }
}