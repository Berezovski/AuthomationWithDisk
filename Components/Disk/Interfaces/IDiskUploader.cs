namespace Authomation.Disk.Interfaces
{
    public interface IDiskUploader
    {
        IDownloadAndUploadSettings DownloadAndUploadSettings { get; set; }
        string Info { get; }
        bool RunService();
        bool CheckLocalPath();
        void StartUploadExportFiles();
        void StartDownloadFiles();
    }
}
