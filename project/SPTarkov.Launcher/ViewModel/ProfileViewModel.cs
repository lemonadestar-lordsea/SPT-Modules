using SPTarkov.Common.Utils.App;
using SPTarkov.Launcher.Generics;
using SPTarkov.Launcher.Helpers;
using System.Windows;

namespace SPTarkov.Launcher.ViewModel
{
    public class ProfileViewModel
    {
        public string CurrentEmail { get; set; }
        public string CurrentEdition { get; set; }
        public string CurrentID { get; set; }
        public GenericICommand LogoutCommand { get; set; }
        public GenericICommand EditProfileCommand { get; set; }
        public GenericICommand StartGameCommand { get; set; }
        private NavigationViewModel navigationViewModel { get; set; }
        private GameStarter gameStarter = new GameStarter();
        private System.Windows.Forms.NotifyIcon trayIcon = new System.Windows.Forms.NotifyIcon();

        private ProcessMonitor monitor { get; set; }
        public ProfileViewModel(NavigationViewModel viewModel)
        {
            navigationViewModel = viewModel;
            LogoutCommand = new GenericICommand(OnLogoutCommand);
            EditProfileCommand = new GenericICommand(OnEditProfileCommand);
            StartGameCommand = new GenericICommand(OnStartGameCommand);

            monitor = new ProcessMonitor("EscapeFromTarkov", 1000, aliveCallback: null, exitCallback: GameExitCallback);

            CurrentEmail = AccountManager.SelectedAccount.email;
            CurrentEdition = AccountManager.SelectedAccount.edition;
            CurrentID = AccountManager.SelectedAccount.id;

            trayIcon.Icon = Properties.Resources.icon;
            trayIcon.Text = "SPTarkov Launcher";
            trayIcon.Visible = false;
            trayIcon.MouseDoubleClick += TrayIcon_MouseDoubleClick;
            Application.Current.Exit += Current_Exit;
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();
        }

        public void OnLogoutCommand(object parameter)
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();

            navigationViewModel.SelectedViewModel = new LoginViewModel(navigationViewModel);
        }
        public void OnEditProfileCommand(object parameter)
        {
            navigationViewModel.SelectedViewModel = new EditProfileViewModel(navigationViewModel);
        }
        public void OnStartGameCommand(object parameter)
        {
            LauncherSettingsProvider.Instance.GameRunning = true;

            int status = gameStarter.LaunchGame(ServerManager.SelectedServer, AccountManager.SelectedAccount);

            switch (status)
            {
                case 1:
                    monitor.Start();

                    if(LauncherSettingsProvider.Instance.HideToTrayOnGameStart)
                    {
                        trayIcon.Visible = true;
                        Application.Current.MainWindow.Hide();
                    }

                    break;

                case -1:
                    MessageBox.Show("SPTarkov shouldn't be installed into live game. Please make a copy of the gamefiles and install SPTarkov there.");
                    break;

                case -2:
                    MessageBox.Show("Escape From Tarkov isn't installed on your computer. Please buy a copy of the game and support the developers!");
                    break;

                case -3:
                    MessageBox.Show("The launcher is not running from the game directory");
                    return;
            }
        }

        private void GameExitCallback(ProcessMonitor monitor)
        {
            monitor.Stop();

            LauncherSettingsProvider.Instance.GameRunning = false;

            //Make sure the call to MainWindow happens on the UI thread.
            Application.Current.Dispatcher.Invoke(() =>
            {
                Application.Current.MainWindow.Show();
            });
            
        }

        private void TrayIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Application.Current.MainWindow.Show();
        }
    }
}
