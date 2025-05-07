using MalyFarmar.Clients; 

namespace MalyFarmar.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly ApiClient _client;
        private UserListViewDto _selectedUser;

        public LoginPage(ApiClient client)
        {
            InitializeComponent();
            _client = client;
            _ = LoadAndBindUsersAsync();
        }

        private async Task LoadAndBindUsersAsync()
        {
            try
            {
                var usersList = await _client.GetAllUsersAsync();

                if (usersList == null || usersList.Users == null)
                {
                    UsersCollectionView.ItemsSource = null;
                    return;
                }

                UsersCollectionView.ItemsSource = usersList.Users;
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", "Error loading users: " + e.Message, "OK");
            }
        }

        private void OnUserSelected(object sender, SelectionChangedEventArgs e) // Can be made async void if any await is added, but not strictly needed for Preferences.Set
        {
            var firstSelectedItem = e.CurrentSelection.FirstOrDefault();

            if (firstSelectedItem == null)
            {
                _selectedUser = null;
            }
            else
            {
                _selectedUser = firstSelectedItem as UserListViewDto;
            }

            if (_selectedUser == null)
            {
                Preferences.Default.Remove("CurrentUserId");
                return;
            }

            Preferences.Default.Set("CurrentUserId", _selectedUser.Id.ToString() ?? string.Empty);
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            if (_selectedUser == null)
            {
                await DisplayAlert("Select user", "Please tap one of the profiles above first.", "OK");
                return;
            }

            Preferences.Default.Set("CurrentUserId", _selectedUser.Id.ToString());
            Console.WriteLine("Signing in as user: " + _selectedUser.Id);

            Application.Current.MainPage = new AppShell();
        }

        private async void OnCreateUserClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Create User", "TODO Navigate to CreateUserPage.", "OK");
        }
    }
}