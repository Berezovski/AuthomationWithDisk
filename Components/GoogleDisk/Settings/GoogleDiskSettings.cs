using Disk.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GoogleDisk.Settings
{
    public class GoogleDiskSettings : IDownloadAndUploadSettings
    {
        public string OauthSecretFile { get; set; }
        public string FolderForSaveClientSettings { get; set; }
        public string UserName { get; set; }
        public string ApplicationName { get; set; }
        public string DeviceName { get; set; }
        public string CloudStoragePath { get; set; }
        public string ExportPath { get; set; }
        public bool OverwriteCloudFiles { get; set; }
        public bool DeleteFileAfterUpload { get; set; }

        public static GoogleDiskSettings FromConfiguration(IConfiguration configuration)
        {
            return configuration.GetSection(nameof(GoogleDiskSettings)).Get<GoogleDiskSettings>();
        }
    }
}
