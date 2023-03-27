using System.ComponentModel;

namespace Authomation.Models
{
    class GetFilesHeaderText : INotifyPropertyChanged
    {
        private string buttonHeader;

        public string ButtonHeader
        {
            get
            {
                return buttonHeader;
            }
            set
            {
                buttonHeader = value;
                OnPropertyChanged("ButtonHeader");
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
