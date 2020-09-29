using System.ComponentModel;

namespace SPTarkov.Launcher.Models.Launcher
{
    public class LoginModel : INotifyPropertyChanged
    {
        private bool _AllowOtherCommands;
        public bool AllowOtherCommands
        {
            get => _AllowOtherCommands;
            set
            {
                if (_AllowOtherCommands != value)
                {
                    _AllowOtherCommands = value;
                    RaisePropertyChanged(nameof(AllowOtherCommands));
                }
            }
        }

        private string _Email;
        public string Email
        {
            get => _Email;
            set
            {
                if(_Email != value)
                {
                    _Email = value;
                    RaisePropertyChanged(nameof(Email));
                }
            }
        }

        private string _Password;
        public string Password
        {
            get => _Password;
            set
            {
                if(_Password != value)
                {
                    _Password = value;
                    RaisePropertyChanged(nameof(Password));
                }
            }
        }

        public LoginModel()
        {
            Email = "";
            Password = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
