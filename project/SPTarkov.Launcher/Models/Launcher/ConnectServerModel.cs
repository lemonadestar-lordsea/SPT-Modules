﻿using System.ComponentModel;

namespace SPTarkov.Launcher.Models.Launcher
{
    public class ConnectServerModel : INotifyPropertyChanged
    {
        private string _InfoText;
        public string InfoText
        {
            get => _InfoText;
            set
            {
                if(_InfoText != value)
                {
                    _InfoText = value;
                    RaisePropertyChanged(nameof(InfoText));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
