using System.ComponentModel;

namespace Authomation.Models
{
    class SendFilesButtonIsEnabled : INotifyPropertyChanged
    {
        private bool buttonIsEnabled ;

        public bool ButtonIsEnabled
        {
            get
            {
                return buttonIsEnabled;
            }
            set
            {
                buttonIsEnabled = value;
                OnPropertyChanged("ButtonIsEnabled");
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
