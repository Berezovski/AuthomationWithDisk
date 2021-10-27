using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disk.Interfaces
{
    public interface IDiskUploader
    {
        string Info { get; }
        bool UploadExportFiles();

    }
}
