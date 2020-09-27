using SPTarkov.Launcher.Generics;
using SPTarkov.Launcher.Helpers;
using SPTarkov.Launcher.ViewModel;
using System.Windows;

namespace SPTarkov.Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SPTarkovLauncherMainWindow : Window
    {
        public GenericICommand ShowSettingsCommand { get; set; }

        public NavigationViewModel FullSpanNavigationViewModel { get; set; }
        public NavigationViewModel navigationViewModel { get; set; }
        public SPTarkovLauncherMainWindow()
        {
            InitializeComponent();

            if(LauncherSettingsProvider.Instance.FirstRun)
            {
                LauncherSettingsProvider.Instance.FirstRun = false;
                LauncherSettingsProvider.Instance.SaveSettings();
                LocalizationProvider.TryAutoSetLocale();
            }

            var viewmodel = new NavigationViewModel();
            viewmodel.SelectedViewModel = new ConnectServerViewModel(viewmodel);
            navigationViewModel = viewmodel;


            var fullSpanViewModel = new NavigationViewModel();
            FullSpanNavigationViewModel = fullSpanViewModel;

            ShowSettingsCommand = new GenericICommand(OnShowSettingsCommand);
        }

        public void OnShowSettingsCommand(object parameter)
        {
            if (LauncherSettingsProvider.Instance.IsEditingSettings)
            {
                return;
            }

            navigationViewModel.SelectedViewModel = null;

            LauncherSettingsProvider.Instance.IsEditingSettings = true;
            FullSpanNavigationViewModel.SelectedViewModel = new SettingsViewModel(FullSpanNavigationViewModel, navigationViewModel);
        }
    }
}
