using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Authomation.Models
{
    class Paths : INotifyPropertyChanged
    {
        private string remotePath;
        private string localPath;


        public string RemotePath
        {
            get
            {
                return remotePath;
            }
            set
            {
                remotePath = value;
                OnPropertyChanged("RemotePath");
            }
        }
        public string LocalPath
        {
            get
            {
                return localPath;
            }
            set
            {
                localPath = value;
                OnPropertyChanged("LocalPath");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string pop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pop));
            }
        }
    }
}
