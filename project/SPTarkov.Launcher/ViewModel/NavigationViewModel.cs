using System.ComponentModel;

namespace SPTarkov.Launcher.ViewModel
{
    public class NavigationViewModel : INotifyPropertyChanged
    {
        private object _selectedViewModel;

        public object SelectedViewModel
        {
            get => _selectedViewModel;
            set
            {
                _selectedViewModel = value;
                RaisePropertyChanged(nameof(SelectedViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
