using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Clients;
using MalyFarmar.Pages;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels.Shared;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;

namespace MalyFarmar.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _preferencesService;
        private readonly CreateUserPage _createUserPage;

        public ObservableCollection<UserListViewDto> Users { get; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
        private UserListViewDto? _selectedUser;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
        [NotifyCanExecuteChangedFor(nameof(CreateUserCommand))]
        private bool _isBusy = false;

        private bool CanSignIn() => SelectedUser != null && !IsBusy;
        private bool CanNavigate() => !IsBusy;

        partial void OnSelectedUserChanged(UserListViewDto? value)
        {
            if (value != null)
            {
                _preferencesService.SetCurrentUserId(value.Id);
            }
            else
            {
                _preferencesService.UnsetCurrentUserId();
            }
        }

        public LoginViewModel(ApiClient apiClient, IPreferencesService preferencesService, CreateUserPage createUserPage)
        {
            _apiClient = apiClient;
            _preferencesService = preferencesService;
            _createUserPage = createUserPage;

            Users = new ObservableCollection<UserListViewDto>();
            IsBusy = false;
        }

        protected override async Task LoadDataAsync()
        {
            IsBusy = true;
            try
            {
                var usersList = await _apiClient.GetAllUsersAsync();
                SelectedUser = null;
                Users.Clear();
        
                _preferencesService.UnsetCurrentUserId();
                Console.WriteLine($"DEEEBUUUUGGGG: preferences current user: {_preferencesService.GetCurrentUserId()}");
                if (usersList?.Users == null)
                {
                    return;
                }

                foreach (var user in usersList.Users)
                {
                    Users.Add(user);
                }
            }
            catch (Exception ex)
            {
                var toast = Toast.Make("Error loading users: " + ex.Message);
                await toast.Show();
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        public async Task ResetStateAsync()
        {
            SelectedUser = null;
            Users.Clear();
        
            _preferencesService.UnsetCurrentUserId();

            await LoadDataAsync();
        }

        [RelayCommand(CanExecute = nameof(CanSignIn))]
        private async Task SignIn()
        {
            if (SelectedUser == null)
            {
                var toast = Toast.Make("Please tap one of the profiles above first.");
                await toast.Show();

                return;
            }

            System.Diagnostics.Debug.WriteLine("Signing in as user: " + SelectedUser.Id);

            var app = Application.Current as App;
            app?.SwitchToShell();
        }

        [RelayCommand(CanExecute = nameof(CanNavigate))]
        private async Task CreateUser()
        {
            var navigation = Application.Current?.MainPage?.Navigation;

            if (navigation != null)
            {
                await navigation.PushAsync(_createUserPage);
                return;
            }

            var toast = Toast.Make("Cannot navigate at this time. Please restart the application.");
            await toast.Show();
        }
    }
}