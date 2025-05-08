using MalyFarmar.Clients;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MalyFarmar.Pages;
using MalyFarmar.Resources.Strings;

namespace MalyFarmar.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _client;
        private UserListViewDto _selectedUser;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<UserListViewDto> Users { get; private set; }

        public UserListViewDto SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    OnPropertyChanged();

                    if (_selectedUser != null)
                    {
                        Preferences.Default.Set("CurrentUserId", _selectedUser.Id.ToString() ?? string.Empty);
                    }
                    else
                    {
                        Preferences.Default.Remove("CurrentUserId");
                    }

                    // Refresh SignIn command's can execute status
                    (SignInCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        public ICommand SignInCommand { get; }
        public ICommand CreateUserCommand { get; }

        public LoginViewModel(ApiClient client)
        {
            _client = client;
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
                var usersList = await _client.GetAllUsersAsync();

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

            Preferences.Default.Set("CurrentUserId", SelectedUser.Id.ToString());
            Console.WriteLine("Signing in as user: " + SelectedUser.Id);

            Application.Current.MainPage = new AppShell();
        }

        private void CreateUserAsync()
        {
            Application.Current.MainPage = new CreateUserPage(_client);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
