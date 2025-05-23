using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Clients;
using MalyFarmar.Pages;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels.Shared;
using System.Collections.ObjectModel;
using MalyFarmar.Resources.Strings;

namespace MalyFarmar.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _preferencesService;

        public ObservableCollection<UserListViewDto> Users { get; } // Already an ObservableCollection

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
        // Notify SignInCommand when SelectedUser changes
        private UserListViewDto? _selectedUser;

        // This method is auto-called by CommunityToolkit.Mvvm when SelectedUser changes
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
            // SignInCommand.NotifyCanExecuteChanged(); // Handled by [NotifyCanExecuteChangedFor]
        }

        public LoginViewModel(ApiClient apiClient, IPreferencesService preferencesService)
        {
            _apiClient = apiClient;
            _preferencesService = preferencesService;
            Users = new ObservableCollection<UserListViewDto>();
            IsBusy = false;
        }

        protected override async Task LoadDataAsync()
        {
            await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            IsBusy = true;
            try
            {
                var usersList = await _apiClient.GetAllUsersAsync();
                Users.Clear();
                if (usersList?.Users != null)
                {
                    foreach (var user in usersList.Users)
                    {
                        Users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Error loading users: " + ex.Message,
                    "OK"
                );
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanSignIn() => SelectedUser != null && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanSignIn))]
        private async Task SignIn()
        {
            if (SelectedUser == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Select User",
                    "Please tap one of the profiles above first.",
                    "Error"
                );
                return;
            }

            System.Diagnostics.Debug.WriteLine("Signing in as user: " + SelectedUser.Id);

            Application.Current.MainPage = new AppShell();
        }

        [RelayCommand(CanExecute = nameof(CanNavigate))]
        private async Task CreateUser()
        {
            if (Application.Current?.MainPage?.Navigation != null)
            {
                var serviceProvider = Application.Current.Handler?.MauiContext?.Services ??
                                      (Application.Current as IPlatformApplication)?.Services;

                if (serviceProvider != null)
                {
                    var createUserPage = serviceProvider.GetService<CreateUserPage>();
                    if (createUserPage != null)
                    {
                        await Application.Current.MainPage.Navigation.PushAsync(createUserPage);
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            "Error",
                            "Unable to open the create user page. Please try again later.",
                            CommonStrings.Ok);
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "Application services not available to open create user page.",
                        CommonStrings.Ok);
                }
            }
            else
            {
                // This would happen if MainPage is not a NavigationPage or not set.
                await Application.Current.MainPage.DisplayAlert(
                    "Navigation Error",
                    "Cannot navigate at this time. Please restart the application.",
                    CommonStrings.Ok);
            }
        }


        private bool CanNavigate() => !IsBusy;


        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SignInCommand))]
        [NotifyCanExecuteChangedFor(nameof(CreateUserCommand))]
        private bool _isBusy;
    }
}