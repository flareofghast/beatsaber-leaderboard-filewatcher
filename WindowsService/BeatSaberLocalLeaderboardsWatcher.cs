using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceProcess;

namespace WindowsService
{
    public partial class BeatSaberLocalLeaderboardsWatcher : ServiceBase
    {
        private FileSystemWatcher fileWatcher;

        public BeatSaberLocalLeaderboardsWatcher()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Console.WriteLine("File Watcher Started");
            InitializeFileSystemWatcher();
        }

        protected override void OnStop()
        {
            Console.WriteLine("File Watcher Stopped");
            this.DisposeFileSystemWatcher();
        }

        private void InitializeFileSystemWatcher()
        {
            fileWatcher = new FileSystemWatcher(ConfigurationManager.AppSettings["PathToWatch"].ToString());
            fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileWatcher.EnableRaisingEvents = true;
            fileWatcher.Filter = ConfigurationManager.AppSettings["PathFilter"].ToString();

            fileWatcher.Changed += (sender, e) =>
            {
                Console.WriteLine("File Changed");
                this.PostFile(ConfigurationManager.AppSettings["SyncUrl"].ToString(), e.FullPath);
            };
        }

        private void PostFile(string url, string filepath)
        {
            if (!File.Exists(filepath))
            {
                Console.WriteLine("Cannot post file, file does not exist");
                return;
            }

            try
            {
                var text = File.ReadAllText(filepath);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(text);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    Console.WriteLine(String.Format("Response Status: {0}", httpResponse.StatusCode));
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void DisposeFileSystemWatcher()
        {
            if (this.fileWatcher != null)
            {
                this.fileWatcher.Dispose();
                this.fileWatcher = null;
            }
        }
    }
}
