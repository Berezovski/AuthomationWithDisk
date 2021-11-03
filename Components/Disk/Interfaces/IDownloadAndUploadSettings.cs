using System;
using System.Collections.Generic;
using System.Text;

namespace Disk.Interfaces
{
    public interface IDownloadAndUploadSettings
    {
        public string CloudStoragePath { get; set; }
        public string ExportPath { get; set; }
        public bool OverwriteCloudFiles { get; set; }
        public bool DeleteFileAfterUpload { get; set; }
    }
}
