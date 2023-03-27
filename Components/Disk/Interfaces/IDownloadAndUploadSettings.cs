namespace Disk.Interfaces
{
    public interface IDownloadAndUploadSettings
    {
        string CloudStoragePath { get; set; }
        string ExportPath { get; set; }
        bool OverwriteCloudFiles { get; set; }
        bool DeleteFileAfterUpload { get; set; }
    }
}
