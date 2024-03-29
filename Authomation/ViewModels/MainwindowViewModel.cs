﻿using Authomation.Models;
using Disk.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WinLogger;
using DryIoc;
using System.Threading.Tasks;
using System.Threading;

namespace Authomation
{
    class MainwindowViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        #region Fields
        private Paths selectedPaths;
        private GetFolderPathCommand onSelectLocalFolderClick;

        private SendFilesCommand onSendFilesClick;
        private SendFilesButtonIsEnabled sendFilesButtonEnabled;
        private SendFilesHeaderText sendFilesHeader;

        private GetFilesCommand onGetFilesClick;
        private GetFilesButtonIsEnabled getFilesButtonEnabled;
        private GetFilesHeaderText getFilesHeader;

        private ExitFromProgramClick onExitFromProgramClick;

        private IDiskUploader _diskUploader;

        private CancellationTokenSource killSenderTask;

        public static IContainer _container = new Container();
        #endregion

        #region Constructors
        public MainwindowViewModel()
        {
            selectedPaths = new Paths
                { 
                LocalPath = "-",
                RemotePath = "-"
                };

            onSelectLocalFolderClick = new GetFolderPathCommand(GetFolderPath);
            onSendFilesClick = new SendFilesCommand(SendFilesOrKillSenderTask);
            sendFilesButtonEnabled = new SendFilesButtonIsEnabled { ButtonIsEnabled = true };
            sendFilesHeader = new SendFilesHeaderText { ButtonHeader = "Отправить" };

            onGetFilesClick = new GetFilesCommand(GetFilesOrKillSenderTask);
            getFilesButtonEnabled = new GetFilesButtonIsEnabled { ButtonIsEnabled = true };
            getFilesHeader = new GetFilesHeaderText { ButtonHeader = "Получить" };

            onExitFromProgramClick = new ExitFromProgramClick(ExitFromProgram);

            killSenderTask = new CancellationTokenSource();

            var config = Startup.BuildConfiguration();
            _container.RegisterInstance(config);
            _container.AddModules();
            _diskUploader = _container.Resolve<IDiskUploader>();

            SelectedPaths.LocalPath = _diskUploader.DownloadAndUploadSettings.ExportPath;
            SelectedPaths.RemotePath = _diskUploader.DownloadAndUploadSettings.CloudStoragePath;
        }
        #endregion

        #region Properties
        public ExitFromProgramClick OnExitFromProgramClick
        {
            get
            {
                return onExitFromProgramClick;
            }
            set
            {
                onExitFromProgramClick = value;
                OnPropertyChanged("OnExitFromProgramClick");
            }
        }
        public Paths SelectedPaths
        {
            get
            {
                return selectedPaths;
            }
            set
            {
                selectedPaths = value;
                OnPropertyChanged("SelectedPaths");
            }
        }

        public SendFilesButtonIsEnabled SendFilesButtonEnabled
        {
            get
            {
                return sendFilesButtonEnabled;
            }
            set
            {
                sendFilesButtonEnabled = value;
                OnPropertyChanged("SendFilesButtonEnabled");
            }
        }

        public GetFilesButtonIsEnabled GetFilesButtonEnabled
        {
            get
            {
                return getFilesButtonEnabled;
            }
            set
            {
                getFilesButtonEnabled = value;
                OnPropertyChanged("GetFilesButtonEnabled");
            }
        }

        public SendFilesHeaderText SendFilesHeader
        {
            get
            {
                return sendFilesHeader;
            }
            set
            {
                sendFilesHeader = value;
                OnPropertyChanged("SendFilesHeader");
            }
        }

        public GetFilesHeaderText GetFilesHeader
        {
            get
            {
                return getFilesHeader;
            }
            set
            {
                getFilesHeader = value;
                OnPropertyChanged("GetFilesHeader");
            }
        }

        public GetFolderPathCommand OnSelectLocalFolderClick
        {
            get
            {
                return onSelectLocalFolderClick;
            }
            set
            {
                onSelectLocalFolderClick = value;
                OnPropertyChanged("OnSelectLocalFolderClick");
            }
        }

        public SendFilesCommand OnSendFilesClick
        {
            get
            {
                return onSendFilesClick;
            }
            set
            {
                onSendFilesClick = value;
                OnPropertyChanged("OnSendFilesClick");
            }
        }

        public GetFilesCommand OnGetFilesClick
        {
            get
            {
                return onGetFilesClick;
            }
            set
            {
                onGetFilesClick = value;
                OnPropertyChanged("OnGetFilesClick");
            }
        }
        #endregion

        #region Methods

        private void ExitFromProgram(object obj)
        {
            Environment.Exit(1);
        }

        private void GetFilesOrKillSenderTask(object obj)
        {

            if (GetFilesHeader.ButtonHeader == "Получить")
            {
                _diskUploader.DownloadAndUploadSettings = new DownloadAndUploadSettings
                {
                    CloudStoragePath = SelectedPaths.RemotePath,
                    ExportPath = SelectedPaths.LocalPath,
                    DeleteFileAfterUpload = true,
                    OverwriteCloudFiles = true
                };

                LogUploader.LogInformation("Старт сервиса отправки файлов...");
                killSenderTask = new CancellationTokenSource();
                Task senderTask = new Task(() => StartGetFiles(1000), killSenderTask.Token);
                senderTask.Start();
            }
            else
            {
                GetFilesButtonEnabled.ButtonIsEnabled = false;
                GetFilesHeader.ButtonHeader = "Получить";
                killSenderTask.Cancel();

            }

        }

        private void GetFolderPath(object obj)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    selectedPaths.LocalPath = fbd.SelectedPath;
                }

            }
        }

        private void SendFilesOrKillSenderTask(object obj)
        {
            if (SendFilesHeader.ButtonHeader == "Отправить")
            {
                _diskUploader.DownloadAndUploadSettings = new DownloadAndUploadSettings
                {
                    CloudStoragePath = SelectedPaths.RemotePath,
                    ExportPath = SelectedPaths.LocalPath,
                    DeleteFileAfterUpload = true,
                    OverwriteCloudFiles = true
                };

                LogUploader.LogInformation("Старт сервиса отправки файлов...");
                killSenderTask = new CancellationTokenSource();
                Task senderTask = new Task(() => StartSendFiles(1000), killSenderTask.Token);
                senderTask.Start();
            }
            else
            {
                SendFilesButtonEnabled.ButtonIsEnabled = false;
                SendFilesHeader.ButtonHeader = "Отправить";
                killSenderTask.Cancel();

            }

        }

        private void StartGetFiles(int period)
        {
            
            try
            {
                GetFilesButtonEnabled.ButtonIsEnabled = false;
                SendFilesButtonEnabled.ButtonIsEnabled = false;

                _diskUploader.RunService();
                _diskUploader.CheckLocalPath();

                GetFilesHeader.ButtonHeader = "Остановить";
                GetFilesButtonEnabled.ButtonIsEnabled = true;


                while (!killSenderTask.Token.IsCancellationRequested)
                {

                    _diskUploader.StartDownloadFiles();

                    Thread.Sleep(period);
                }


            }
            catch
            {

            }
            finally
            {
                GetFilesHeader.ButtonHeader = "Получить";
                SendFilesButtonEnabled.ButtonIsEnabled = true;
                GetFilesButtonEnabled.ButtonIsEnabled = true;
            }


        }

        private void StartSendFiles(int period)
        {
            try
            {
                SendFilesButtonEnabled.ButtonIsEnabled = false;
                GetFilesButtonEnabled.ButtonIsEnabled = false;

                _diskUploader.RunService();
                _diskUploader.CheckLocalPath();

                SendFilesHeader.ButtonHeader = "Остановить";
                SendFilesButtonEnabled.ButtonIsEnabled = true;

                while (!killSenderTask.Token.IsCancellationRequested)
                {

                    _diskUploader.StartUploadExportFiles();
                    Thread.Sleep(period);
                }

            }
            catch
            {


            }
            finally
            {
                SendFilesHeader.ButtonHeader = "Отправить";
                SendFilesButtonEnabled.ButtonIsEnabled = true;
                GetFilesButtonEnabled.ButtonIsEnabled = true;
            }


        }

        public void OnPropertyChanged(string pop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(pop));
            }
        }
        #endregion

        #region Events
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        #endregion

    }
}
