using SPTarkov.Launcher.Generics;
using SPTarkov.Launcher.Helpers;
using SPTarkov.Launcher.Models.Launcher;

namespace SPTarkov.Launcher.ViewModel
{
    public class WipeProfileViewModel
    {
        public GenericICommand WipeCommand { get; set; }
        public GenericICommand BackCommand { get; set; }
        public WipeProfileModel ProfileWipe { get; set; }
        private NavigationViewModel navigationViewModel { get; set; }

        public WipeProfileViewModel(NavigationViewModel viewModel)
        {
            navigationViewModel = viewModel;
            WipeCommand = new GenericICommand(OnWipeCommand);
            BackCommand = new GenericICommand(OnBackCommand);

            WipeProfileModel tmpWipeProfile = new WipeProfileModel();

            ProfileWipe = tmpWipeProfile;
        }

        public void OnBackCommand(object parameter)
        {
            navigationViewModel.SelectedViewModel = new EditProfileViewModel(navigationViewModel);
        }

        public void OnWipeCommand(object parameter)
        {
            int status = AccountManager.Wipe(ProfileWipe.EditionsCollection.SelectedEdition);

            switch (status)
            {
                case 1:
                    navigationViewModel.SelectedViewModel = new ProfileViewModel(navigationViewModel);
                    break;

                case -1:
                    navigationViewModel.NotificationQueue.Enqueue(LocalizationProvider.Instance.login_failed);
                    return;

                case -2:
                    navigationViewModel.SelectedViewModel = new ConnectServerViewModel(navigationViewModel);
                    return;
            }
        }
    }
}
