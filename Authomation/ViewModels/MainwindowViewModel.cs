using Authomation.Models;
using Disk.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WinLogger;
using DryIoc;
using System.Threading.Tasks;

namespace Authomation
{
    class MainwindowViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private Paths selectedPaths;
        private GetFolderPathCommand onSelectLocalFolderClick;
        private SendFilesCommand onSendFilesClick;
        public static IContainer _container = new Container();

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

        public MainwindowViewModel()
        {
            selectedPaths = new Paths
                { 
                LocalPath = "-",
                RemotePath = "-"
                };

            onSelectLocalFolderClick = new GetFolderPathCommand(GetFolderPath);
            onSendFilesClick = new SendFilesCommand(SendFilesAsync);
        }

        public void GetFolderPath(object obj)
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

        public async void SendFilesAsync(object obj)
        {
            LogUploader.LogInformation("Старт сервиса отправки файлов...");
            await Task.Run(() => SendFiles());
        }

        public void SendFiles()
        {
            var config = Startup.BuildConfiguration();   
            
            _container.RegisterInstance(config);
            _container.AddModules();

            IDiskUploader diskUploader = _container.Resolve<IDiskUploader>();
            diskUploader.UploadExportFiles();
            MessageBox.Show(diskUploader.Info,"Информация о службе отправки");

        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string pop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(pop));
            }
        }

    }
}
