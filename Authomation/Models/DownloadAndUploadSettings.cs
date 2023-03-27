using Disk.Interfaces;

namespace Authomation.Models
{
    class DownloadAndUploadSettings : IDownloadAndUploadSettings
    {
        public string CloudStoragePath { get; set; }
        public string ExportPath { get; set; }
        public bool OverwriteCloudFiles { get; set; }
        public bool DeleteFileAfterUpload { get; set; }
    }
}
