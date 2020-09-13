using SPTarkov.Launcher.Generics;
using SPTarkov.Launcher.Helpers;
using SPTarkov.Launcher.Models.Launcher;
using System.Windows;

namespace SPTarkov.Launcher.ViewModel
{
    public class EditProfileViewModel
    {
        public LoginModel login { get; set; }

        public GenericICommand UpdateCommand { get; set; }
        public GenericICommand BackCommand { get; set; }
        public GenericICommand WipeProfileCommand { get; set; }

        private NavigationViewModel navigationViewModel { get; set; }
        public EditProfileViewModel(NavigationViewModel viewModel)
        {
            navigationViewModel = viewModel;

            UpdateCommand = new GenericICommand(OnUpdateCommand);
            BackCommand = new GenericICommand(OnBackCommand);
            WipeProfileCommand = new GenericICommand(OnWipeProfileCommand);

            ServerSetting DefaultServer = LauncherSettingsProvider.GetDefaultServer();

            LoginModel tmpLogin = new LoginModel();
            tmpLogin.Email = DefaultServer.AutoLoginCreds.Email;
            tmpLogin.Password = DefaultServer.AutoLoginCreds.Password;

            login = tmpLogin;
        }

        private string GetStatus(int status)
        {
            switch(status)
            {
                case 1:
                    return "OK";

                case -1:
                    
                    return "Login failed";

                case -2:
                    return "CONNECTION_ERROR";
            }

            return "Undefined Response";   
        }

        public void OnUpdateCommand(object parameter)
        {
            string emailStatus = GetStatus(AccountManager.ChangeEmail(login.Email));
            string passStatus = GetStatus(AccountManager.ChangePassword(login.Password));

            if(emailStatus == "CONNECTION_ERROR" || passStatus == "CONNECTION_ERROR")
            {
                navigationViewModel.SelectedViewModel = new ConnectServerViewModel(navigationViewModel);
                return;
            }

            if(emailStatus == "OK" && passStatus == "OK")
            {
                navigationViewModel.SelectedViewModel = new ProfileViewModel(navigationViewModel);
            }
            else
            {
                MessageBox.Show(LocalizationProvider.Instance.edit_profile_update_error); 
                //$"Some Issues occurred while updating your profile.\n\nEmail Update: {emailStatus}\nPassword Update: {passStatus}", "Update Errors Occured", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OnBackCommand(object parameter)
        {
            navigationViewModel.SelectedViewModel = new ProfileViewModel(navigationViewModel);
        }

        public void OnWipeProfileCommand(object parameter)
        {
            navigationViewModel.SelectedViewModel = new WipeProfileViewModel(navigationViewModel);
        }
    }
}
