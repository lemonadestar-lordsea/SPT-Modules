using SPTarkov.Launcher.Generics;
using SPTarkov.Launcher.Helpers;
using SPTarkov.Launcher.Models.Launcher;
using System.Windows;

namespace SPTarkov.Launcher.ViewModel
{
    public class SettingsViewModel
    {
        public GenericICommand BackCommand { get; set; }
        public GenericICommand RemoveServerCommand { get; set; }
        public GenericICommand SetServerAsDefaultCommand { get; set; }
        public GenericICommand AddServerCommand { get; set; }
        public GenericICommand SaveNewServerCommand { get; set; }
        public GenericICommand ShowServerListCommand { get; set; }
        public ServerSetting NewServer { get; set; }
        public LocaleCollection Locales { get; set; } = new LocaleCollection();
        private NavigationViewModel fullSpanNavigationViewModel { get; set; }
        private NavigationViewModel mainNavigationViewModel { get; set; }
        public SettingsViewModel(NavigationViewModel fullSpanViewModel, NavigationViewModel mainViewModel)
        {
            fullSpanNavigationViewModel = fullSpanViewModel;
            mainNavigationViewModel = mainViewModel;

            BackCommand = new GenericICommand(OnBackCommand);
            RemoveServerCommand = new GenericICommand(OnRemoveServerCommand);
            AddServerCommand = new GenericICommand(OnAddServerCommand);
            SetServerAsDefaultCommand = new GenericICommand(OnSetServerAsDefaultCommand);
            SaveNewServerCommand = new GenericICommand(OnSaveNewServerCommand);
            ShowServerListCommand = new GenericICommand(OnShowServerListCommand);

            ServerSetting tmpSettings = new ServerSetting();

            NewServer = tmpSettings;
        }


        public void OnBackCommand(object parameter)
        {
            LauncherSettingsProvider.Instance.SaveSettings();

            LauncherSettingsProvider.Instance.IsEditingSettings = false;

            mainNavigationViewModel.SelectedViewModel = new ConnectServerViewModel(mainNavigationViewModel);
            fullSpanNavigationViewModel.SelectedViewModel = null;
        }

        public void OnAddServerCommand(object parameter)
        {
            NewServer.Name = "";
            NewServer.Url = "";
            NewServer.IsDefault = false;

            LauncherSettingsProvider.Instance.IsAddingServer = true;
        }

        public void OnRemoveServerCommand(object parameter)
        {
            if(parameter is ServerSetting setting)
            {
                if (setting.Url == "https://127.0.0.1")
                {
                    MessageBoxResult result = MessageBox.Show($"{LocalizationProvider.Instance.local_server_remove_warning}\n{LocalizationProvider.Instance.remove_server_question}", "", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                LauncherSettingsProvider.Instance.RemoveServerAndSave(setting);
            }
        }

        public void OnSetServerAsDefaultCommand(object parameter)
        {
            if (parameter is ServerSetting setting)
            {
                LauncherSettingsProvider.Instance.SetDefaultServerAndSave(setting);
            }
        }

        public void OnSaveNewServerCommand(object parameter)
        {
            ServerAddStatus addStatus = LauncherSettingsProvider.Instance.AddServerAndSave(NewServer);

            if(addStatus.AddSucceeded)
            {
                LauncherSettingsProvider.Instance.IsAddingServer = false;
            }
            else
            {
                MessageBox.Show(addStatus.Message);
            }
        }

        public void OnShowServerListCommand(object parameter)
        {
            LauncherSettingsProvider.Instance.IsAddingServer = false;
        }
    }
}
