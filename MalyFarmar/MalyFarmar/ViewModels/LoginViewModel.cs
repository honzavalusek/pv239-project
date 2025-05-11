using MalyFarmar.Clients;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using MalyFarmar.Pages;
using MalyFarmar.Resources.Strings;
using MalyFarmar.Services.Interfaces;

namespace MalyFarmar.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _preferencesService;
        private readonly ILocationService _locationService;
        private UserListViewDto? _selectedUser;

        public ObservableCollection<UserListViewDto> Users { get; private set; }

        public UserListViewDto? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser == value) return;
                _selectedUser = value;
                OnPropertyChanged();

                if (_selectedUser != null)
                {
                    _preferencesService.SetCurrentUserId(_selectedUser.Id);
                }
                else
                {
                    _preferencesService.UnsetCurrentUserId();
                }

                // Refresh SignIn command's can execute status
                (SignInCommand as Command)?.ChangeCanExecute();
            }
        }

        public ICommand SignInCommand { get; } // todo smazat -> relay command
        public ICommand CreateUserCommand { get; }

        public LoginViewModel(ApiClient apiClient, IPreferencesService preferencesService, ILocationService locationService)
        {
            _apiClient = apiClient;
            _preferencesService = preferencesService;
            _locationService = locationService;

            Users = new ObservableCollection<UserListViewDto>();

            SignInCommand = new Command(
                execute: SignInAsync,
                canExecute: () => SelectedUser != null);

            CreateUserCommand = new Command(CreateUserAsync);

            _ = LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var usersList = await _apiClient.GetAllUsersAsync();

                if (usersList?.Users != null)
                {
                    Users.Clear();
                    foreach (var user in usersList.Users)
                    {
                        Users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    LoginPageStrings.ErrorLoadingUsersAlertTitle,
                    LoginPageStrings.ErrorLoadingUsersAlertDescription + ": " + ex.Message,
                    CommonStrings.Ok
                    );
            }
        }

        private async void SignInAsync()
        {
            if (SelectedUser == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    LoginPageStrings.SelectUserAlertTitle,
                    LoginPageStrings.SelectUserAlertDescription,
                    CommonStrings.Error
                    );
                return;
            }

            _preferencesService.SetCurrentUserId(SelectedUser.Id);
            Console.WriteLine("Signing in as user: " + SelectedUser.Id);

            Application.Current.MainPage = new AppShell();
        }

        private void CreateUserAsync()
        {
            Application.Current.MainPage = new CreateUserPage(_apiClient, _preferencesService, _locationService); // TODO: I think this can be done better, using DI somehow
        }
    }
}
