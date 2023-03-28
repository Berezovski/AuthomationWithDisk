using Authomation.Disk.Interfaces;

namespace Authomation.GUI.Models
{
    class DownloadAndUploadSettings : IDownloadAndUploadSettings
    {
        public string CloudStoragePath { get; set; }
        public string ExportPath { get; set; }
        public bool OverwriteCloudFiles { get; set; }
        public bool DeleteFileAfterUpload { get; set; }
    }
}
