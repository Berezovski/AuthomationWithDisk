using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disk.Interfaces
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
