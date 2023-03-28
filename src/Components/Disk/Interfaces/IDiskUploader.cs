using Authomation.Cipher.Interfaces;

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
        void InitCipher(ICipher cipher, byte[] password);
        void UpdatePassword(byte[] password);
    }
}
