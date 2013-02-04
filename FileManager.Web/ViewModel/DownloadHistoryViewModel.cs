using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileManager.Web.ViewModel
{
    public class DownloadHistoryViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public long ResourceId { get; set; }
        public string ResourceTitle { get; set; }
        public long UserWiseDownloadCount { get; set; }
        public long ResourceTotalDownloadCount { get; set; }
        public string  DownloaddateTime { get; set; }
    }
}