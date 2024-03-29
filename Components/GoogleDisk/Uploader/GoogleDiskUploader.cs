﻿using Disk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static Google.Apis.Drive.v3.FilesResource;
using File = Google.Apis.Drive.v3.Data.File;
using GoogleDisk.Settings;
using WinLogger;
using System.Text;
using System.Reflection;

namespace GoogleDisk.Uploader
{
    public class GoogleDiskUploader : IDiskUploader
    {
        #region Fields

        private GoogleDiskSettings _diskSettings;
        private readonly string[] Scopes = { DriveService.Scope.Drive };

        private const string formatPath = "yyyy'.'MM'.'dd";

        private UserCredential _userCredential;
        private DriveService _driveService;

        private StringBuilder info;
        #endregion

        #region Properties
        IDownloadAndUploadSettings IDiskUploader.DownloadAndUploadSettings
        {
            get
            {
                return _diskSettings;
            }
            set
            {
                PropertyInfo[] tmp = value.GetType().GetProperties();
                for (int i = 0; i < tmp.Length; i++)
                {
                    var fieldInfo = _diskSettings.GetType().GetProperty(tmp[i].Name);
                    fieldInfo.SetValue(_diskSettings, tmp[i].GetValue(value));
                }

            }
        }
        string IDiskUploader.Info
        {
            get
            {
                string tmp = info.ToString();
                info.Clear();
                return tmp;
            }
        }

        #endregion

        #region Constructor

        public GoogleDiskUploader(GoogleDiskSettings settings)
        {
            _diskSettings = settings;
            info = new StringBuilder();
        }

        #endregion

        #region Methods


        bool IDiskUploader.RunService()
        {
            try
            {
                SetUserCredential();
                StartDriveService();

                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool CheckLocalPath()
        {
            try
            {
                Directory.GetFiles(_diskSettings.ExportPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void StartUploadExportFiles()
        {
            var files = Directory.GetFiles(_diskSettings.ExportPath);
            //LogUploader.LogInformation($"Getting files to upload to the Google.Disk from: {_diskSettings.ExportPath}");
            info.Append($"Getting files to upload to the Google.Disk from: {_diskSettings.ExportPath}.");


            if (files.Length > 0)
            {
                var cloudPath = $"{_diskSettings.CloudStoragePath}\\{DateTime.Now.ToString(formatPath)}\\{_diskSettings.DeviceName}";
                var id = CreateFoldersFromPathAndGetLastFolderId(cloudPath);

                info.AppendLine($"Found {files.Length} files to upload to the Google.Disk.");
                //LogUploader.LogInformation($"Found {files.Length} files to upload to the Google.Disk");

                UploadFiles(id, files);
            }

        }

        public void StartDownloadFiles()
        {

            var cloudPath = $"{_diskSettings.CloudStoragePath}\\{DateTime.Now.ToString(formatPath)}\\{_diskSettings.DeviceName}";
            var id = CreateFoldersFromPathAndGetLastFolderId(cloudPath);

            // google request 
            var lsRequest = _driveService.Files.List();

            lsRequest.Q = $" '{id}' in parents and trashed = false";

            var fileList = lsRequest.Execute();          

            for (int i = 0; i < fileList.Files.Count; i++)
            {
                var request = _driveService.Files.Get(fileList.Files[i].Id);

                try
                {
                    DownloadFile(request, fileList.Files[i].Name);
                    DeleteFileWithId(fileList.Files[i].Id);
                }
                catch (Exception ex)
                {

                    info.AppendLine($"Error in method UploadExportFiles(): {ex.Message}.");

                }
            }         

        }

        private void DownloadFile(GetRequest request, string fileName)
        {
            using var stream = new System.IO.MemoryStream();
            request.Download(stream);
            System.IO.FileStream file = new System.IO.FileStream(Path.Combine(_diskSettings.ExportPath, fileName)
                , System.IO.FileMode.Create, System.IO.FileAccess.Write);
            stream.WriteTo(file);
            file.Close();
        }

        private void DeleteFileWithId(string fileId)
        {
            var deleteRequest = _driveService.Files.Delete(fileId);
            deleteRequest.Execute();
        }

        /// <inheritdoc/>

        #endregion

        #region Private Methods

        private void SetUserCredential()
        {
            using var stream = new FileStream(
                Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.ToString(),
                "JsonSettings",
                _diskSettings.OauthSecretFile), FileMode.Open);

            string pathForSavedSettings = new FileInfo(Environment.CurrentDirectory +
                $"{_diskSettings.FolderForSaveClientSettings}\\").Directory.FullName;

            _userCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        Scopes,
                        _diskSettings.UserName,
                        CancellationToken.None,
                        new FileDataStore(pathForSavedSettings, true)
                        ).GetAwaiter().GetResult();

        }

        private void StartDriveService()
        {
            _driveService = new DriveService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = _userCredential,
                    ApplicationName = _diskSettings.ApplicationName
                }
                );
        }

        private string CreateFoldersFromPathAndGetLastFolderId(string path)
        {
            string[] folders = path.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            string parentDirectory = "root";

            for (int i = 0; i < folders.Length; i++)
            {
                // google request 
                var lsRequest = _driveService.Files.List();

                lsRequest.Q = $"mimeType  ='application/vnd.google-apps.folder' and" +
                    $" name = '{folders[i]}' and '{parentDirectory}' in parents and trashed = false";

                var folderList = lsRequest.Execute();

                // find folders
                if (folderList.Files.Count == 0)
                {
                    info.AppendLine($"Creating folder {folders[i]} on Yandex.Disk.");

                    var folder = new File
                    {
                        Name = folders[i],
                        MimeType = "application/vnd.google-apps.folder",
                        Parents = new string[] { parentDirectory }
                    };
                    CreateRequest fileReq = _driveService.Files.Create(folder);
                    parentDirectory = fileReq.Execute().Id;

                }
                else
                {
                    if (folderList.Files.Count > 1)
                    {
                        info.AppendLine($"Warning! Same folders, pls, delete folders in {parentDirectory}.");
                    }
                    parentDirectory = folderList.Files[0].Id;

                }

            }

            return parentDirectory;
        }

        private void UploadFiles(string folderId, IEnumerable<string> filePaths)
        {
            if (!filePaths.Any())
            {
                info.AppendLine("Could not find any files to upload.");
                //LogUploader.LogWarning("Could not find any files to upload");
                return;
            }

            info.AppendLine($"Uploading files to folder with id {folderId}.");

            //LogUploader.LogInformation($"Uploading files to folder with id {folderId}");

            var startTick = Environment.TickCount;

            foreach (string filePath in filePaths)
            {
                var fileName = Path.GetFileName(filePath);
                info.AppendLine($"Uploading {fileName}.");
                //LogUploader.LogInformation($"Uploading {fileName}");

                var body = new File
                {
                    Name = Path.GetFileName(filePath),
                    MimeType = MimeTypes.GetMimeType(fileName),
                    Parents = new string[] { folderId }
                };

                byte[] sendByteArray = System.IO.File.ReadAllBytes(filePath);

                var dontDeleteId = UploadRequestAndGetFileId(body, sendByteArray);

                if (_diskSettings.OverwriteCloudFiles)
                {
                    DeleteFilesInFolderFromGoogleDiskWithoutMainId(fileName, body.MimeType, folderId, dontDeleteId);
                }

                if (_diskSettings.DeleteFileAfterUpload)
                {
                    info.AppendLine($"Deleting {filePath}.");
                    System.IO.File.Delete(filePath);
                }
            }

            // results
            var endTick = Environment.TickCount;
            info.AppendLine($"Uploaded files: {filePaths.Count()} in {endTick - startTick} mls.");

        }

        private string UploadRequestAndGetFileId(File body, byte[] sendByteArray)
        {
            using var stream = new MemoryStream(sendByteArray);
            var request = _driveService.Files.Create(body, stream, body.MimeType);
            request.Upload();

            return request.ResponseBody.Id;
        }

        private void DeleteFilesInFolderFromGoogleDiskWithoutMainId(string fileName, string mimeType, string folderId, string dontDeleteId)
        {
            var lsRequest = _driveService.Files.List();
            lsRequest.Q = $"mimeType  ='{mimeType}' and" +
            $" name = '{fileName}' and '{folderId}' in parents and trashed = false";
            var fileList = lsRequest.Execute();

            for (int i = 0; i < fileList.Files.Count; i++)
            {
                if (fileList.Files[i].Id != dontDeleteId)
                {
                    info.AppendLine($"Deleting {fileList.Files[i].Name} in Google.disk for upload new file with this name.");

                    var deleteRequest = _driveService.Files.Delete(fileList.Files[i].Id);
                    deleteRequest.Execute();
                }

            }
        }

        #endregion
    }
}
