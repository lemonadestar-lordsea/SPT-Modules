using SPTarkov.Launcher.Generics;
using SPTarkov.Launcher.Helpers;
using SPTarkov.Launcher.Models.Launcher;
using System.Windows;

namespace SPTarkov.Launcher.ViewModel
{
    public class LoginViewModel
    {
        public LoginModel login { get; set; }
        public ServerSetting DefaultServer { get; set; }
        public GenericICommand ShowRegisterCommand { get; set; }
        public GenericICommand LoginCommand { get; set; }
        private NavigationViewModel navigationViewModel { get; set; }

        public LoginViewModel(NavigationViewModel viewModel)
        {
            navigationViewModel = viewModel;

            ShowRegisterCommand = new GenericICommand(OnShowRegisterCommand);
            LoginCommand = new GenericICommand(OnLoginCommand);

            DefaultServer = LauncherSettingsProvider.GetDefaultServer();

            LoginModel tmpLogin = new LoginModel();

            if (DefaultServer.AutoLoginCreds != null)
            {
                tmpLogin.Email = DefaultServer.AutoLoginCreds.Email ?? "";
                tmpLogin.Password = DefaultServer.AutoLoginCreds.Password ?? "";
            }

            login = tmpLogin;
        }

        public void OnShowRegisterCommand(object parameter)
        {
            navigationViewModel.SelectedViewModel = new RegisterViewModel(navigationViewModel);
        }

        public void OnLoginCommand(object parameter)
        {
            int status = AccountManager.Login(login);

            switch (status)
            {
                case 1:
                    if (LauncherSettingsProvider.Instance.UseAutoLogin && DefaultServer.AutoLoginCreds != login)
                    {
                        DefaultServer.AutoLoginCreds = login;
                    }

                    LauncherSettingsProvider.Instance.SaveSettings();

                    navigationViewModel.SelectedViewModel = new ProfileViewModel(navigationViewModel);
                    break;

                case -1:
                    MessageBox.Show(LocalizationProvider.Instance.incorrect_login);
                    return;

                case -2:
                    navigationViewModel.SelectedViewModel = new ConnectServerViewModel(navigationViewModel);
                    return;
            }
        }
    }
}
