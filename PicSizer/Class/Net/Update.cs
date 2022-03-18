using System;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using PicSizer.Class.Static;
using PicSizer.Class.Unit;

namespace PicSizer.Class.Partial
{
    public static class Update
    {
        private static readonly string[] _URL =
        {
            "https://picsizer.dearxuan.top/version",
            "https://picsizer.vercel.app/version"
        };

        private static UpdateMsg Version_Official = null;
        private static UpdateMsg Version_Alpha = null;

        /// <summary>
        /// 正在查询的标记
        /// </summary>
        private static bool CanConnect = true;

        private static string _Already_Latest = "当前版本(" + Info.ProjectVersion + ")已经是最新版!";

        /// <summary>
        /// 检查更新
        /// </summary>
        /// <param name="alpha">待检测的版本,如果是正式版则只会查找正式版;如果是测试版则同时查找正式版和测试版</param>
        public static void CheckUpdate(bool alpha)
        {
            if (Version_Official == null || Version_Alpha == null)
            {
                if (Dialog.ShowDialog_RetryCancel("获取数据失败!\n请检查您的网络连接.") == DialogResult.Retry)
                {
                    GetLatestVersions();
                    CheckUpdate(alpha);
                }
                return;
            }

            //查找更新的版本
            UpdateMsg latest;
            if (alpha)
            {
                latest = Version_Alpha.Version.CompareTo(Version_Official.Version) > 0
                    ? Version_Alpha
                    : Version_Official;
            }
            else
            {
                latest = Version_Official;
            }
            //当前版本小于最新版
            if (Info.ProjectVersion.CompareTo(latest.Version) < 0)
            {
                string s = "发现新版本(" + Info.ProjectVersion + " -> " + latest.Version + "):\n\n更新日志:\n" 
                           + latest.desc 
                           + (latest.Version.alpha
                           ? "\n开发版可能不稳定,是否仍要下载?"
                           : "\n立即下载?");
                if (Dialog.ShowDialog_OKDialog(s))
                {
                    //Dialog.OpenLink(latest.download);
                    string path = Dialog.Show_SaveOnDiskDialog();
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        HttpReader.SaveFileToDisk(latest.download, path);
                    }
                }
            }
            else
            {
                Dialog.ShowDialog(_Already_Latest);
            }
        }
        
        /// <summary>
        /// 获取最新版版本号
        /// </summary>
        public static void GetLatestVersions()
        {
            if (Version_Alpha != null && Version_Official != null)
            {
                return;
            }

            if (CanConnect)
            {
                CanConnect = false;
                const int delay = 500; // 单次连接最大时长(ms)
                foreach (string url in _URL)
                {
                    try
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(HttpReader.ReadTextFromUrl(url, delay));
                        XmlNode root = xmlDocument.SelectSingleNode("picsizer");
                        Version_Official = ReadVersionFromXmlNode(root.SelectSingleNode("official"));
                        Version_Alpha = ReadVersionFromXmlNode(root.SelectSingleNode("alpha"));
                        Version_Alpha.Version.alpha = true;
                        if (Info.ProjectVersion.CompareTo(Version_Official.Version) >= 0)
                        {
                            Value.mainForm.检查更新ToolStripMenuItem.Text = "已是最新版";
                        }
                        else
                        {
                            Value.mainForm.检查更新ToolStripMenuItem.Text = "更新可用!";
                        }
                        return;
                    }
                    catch(Exception)
                    { }
                }
                Version_Official = Version_Alpha = null;
                CanConnect = true;
            }
        }

        private static UpdateMsg ReadVersionFromXmlNode(XmlNode node)
        {
            string node_version = node.SelectSingleNode("version").InnerText.Trim();
            string node_desc = node.SelectSingleNode("description").InnerText.Trim();
            string[] version = node_version.Split('.');
            UpdateMsg msg = new UpdateMsg();
            msg.Version.mainVersion = byte.Parse(version[0]);
            msg.Version.secondVersion = byte.Parse(version[1]);
            msg.Version.thirdVersion = byte.Parse(version[2]);
            msg.desc = node_desc.Replace(@"\n", "\n");
            msg.download = node.SelectSingleNode("download").InnerText.Trim();
            return msg;
        }
    }
}