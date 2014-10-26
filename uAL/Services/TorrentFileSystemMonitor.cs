using System;
using System.IO;
using System.Threading;
using uAL.Infrastructure;
using uAL.Services;

namespace uAL
{
    public class TorrentFileSystemMonitor
    {
        private const int MAX_RETRIES_LOCK = 60;

        public TorrentFileSystemMonitor(Action<string,string> OnNewTorrent)
        {
            string downloadDir = uAL.Program.settings.Dir;
            string downloadFolder = downloadDir.Substring(downloadDir.LastIndexOf('\\') + 1);
            FileSystemWatcher w = new FileSystemWatcher(downloadDir);
            w.Filter = "*.torrent";
            w.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            w.EnableRaisingEvents = true;
            w.IncludeSubdirectories = true;

            w.Created += (s, e) =>
            {
                var isCreated = false;
                var retries = 0;
                while (!isCreated && retries++ < MAX_RETRIES_LOCK)
                {
                    try
                    {

                        Thread.Sleep(500); // FSW workaround, file has not finished, let's wait a bit since they're just torrent files.
                        using (var torrentStream = File.OpenRead(e.FullPath))
                        {
                            // Just testing
                        }
                        isCreated = true;

                    }
                    catch (FileNotFoundException ex)
                    {
                        LoggingAdapter.Error("Couldn't open the torrent file: " + e.FullPath, ex);
                    }

                }
                if (retries==MAX_RETRIES_LOCK){
                    LoggingAdapter.Error("Giving up tring to load torrent " + e.FullPath);
                    return;
                }


                var computedLabel = TorrentLabelService.CreateTorrentLabel(downloadDir, e.FullPath);
                LoggingAdapter.Info(string.Format("Generated label {0} for file {1}",computedLabel,e.FullPath));

                if (!string.IsNullOrEmpty(computedLabel))
                    OnNewTorrent(e.FullPath, computedLabel);
            };
        }

        

        
    }
}
