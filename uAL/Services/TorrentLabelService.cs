using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uAL.Services
{
    public class TorrentLabelService
    {
        public static string CreateTorrentLabel(string downloadDir, string newFilePath)
        {
            FileInfo fi = new FileInfo(newFilePath);
            string newFileDirectory = fi.Directory.ToString();
            string computedLabel = PathUtils.MakeRelativePath(downloadDir, newFileDirectory);
            return computedLabel;

        }
    }
}
