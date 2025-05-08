// ViewModels/CreateUserViewModel.cs
using MalyFarmar.Clients; // For ApiClient, UserCreateDto, UserViewDto
using Microsoft.Maui.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MalyFarmar.ViewModels
{
    public class CreateUserViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phoneNumber;
        private string _userLongitude;
        private string _userLatitude;

        private string _firstNameError;
        private string _lastNameError;
        private string _emailError;
        private string _phoneNumberError;
        private string _longitudeError;
        private string _latitudeError;
        private string _generalError;
        private bool _isBusy;

        public event PropertyChangedEventHandler PropertyChanged;

        // Bindable properties for Entry fields
        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string PhoneNumber { get => _phoneNumber; set => SetProperty(ref _phoneNumber, value); }
        public string UserLongitude { get => _userLongitude; set => SetProperty(ref _userLongitude, value); }
        public string UserLatitude { get => _userLatitude; set => SetProperty(ref _userLatitude, value); }

        // Bindable properties for error labels
        public string FirstNameError { get => _firstNameError; set => SetProperty(ref _firstNameError, value); }
        public string LastNameError { get => _lastNameError; set => SetProperty(ref _lastNameError, value); }
        public string EmailError { get => _emailError; set => SetProperty(ref _emailError, value); }
        public string PhoneNumberError { get => _phoneNumberError; set => SetProperty(ref _phoneNumberError, value); }
        public string LongitudeError { get => _longitudeError; set => SetProperty(ref _longitudeError, value); }
        public string LatitudeError { get => _latitudeError; set => SetProperty(ref _latitudeError, value); }
        public string GeneralError { get => _generalError; set => SetProperty(ref _generalError, value); }
        
        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

        public ICommand CreateAccountCommand { get; }

        public CreateUserViewModel(ApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            CreateAccountCommand = new Command(async () => await OnCreateAccountAsync(), () => !IsBusy);
        }

        private async Task OnCreateAccountAsync()
        {
            if (!await ValidateInputAsync())
            {
                return;
            }

            IsBusy = true;
            ((Command)CreateAccountCommand).ChangeCanExecute(); // Disable button

            double? longitude = null;
            if (!string.IsNullOrWhiteSpace(UserLongitude) && double.TryParse(UserLongitude, out double lon))
            {
                longitude = lon;
            }

            double? latitude = null;
            if (!string.IsNullOrWhiteSpace(UserLatitude) && double.TryParse(UserLatitude, out double lat))
            {
                latitude = lat;
            }

            var userCreateDto = new UserCreateDto
            {
                FirstName = FirstName.Trim(),
                LastName = LastName.Trim(),
                Email = Email.Trim(),
                PhoneNumber = PhoneNumber.Trim(),
                UserLongitude = longitude,
                UserLatitude = latitude
            };

            try
            {
                GeneralError = null;
                // Ensure ApiClient.CreateUserAsync targets "api/User/create" and expects UserViewDto
                UserViewDto createdUser = await _apiClient.CreateUserAsync(userCreateDto);

                if (createdUser != null && createdUser.Id > 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Account created successfully!", "OK");
                    Preferences.Default.Set("CurrentUserId", createdUser.Id.ToString());
                    Application.Current.MainPage = new AppShell(); // Or use a navigation service
                }
                else
                {
                    GeneralError = "Failed to create account. Unexpected response from server.";
                }
            }
            catch (ApiException apiEx)
            {
                GeneralError = CleanApiErrorMessage(apiEx.Message, apiEx.Message);
            }
            catch (Exception ex)
            {
                GeneralError = "An unexpected error occurred. Please check your connection and try again.";
            }
            finally
            {
                IsBusy = false;
                ((Command)CreateAccountCommand).ChangeCanExecute(); // Re-enable button
            }
        }

        private string CleanApiErrorMessage(string primaryMessage, string content)
        {
            // Basic attempt to make error messages more user-friendly
            // You might want to parse 'content' if it's JSON with validation errors
            if (!string.IsNullOrWhiteSpace(content) && content.Contains("errors")) // Very basic check for ASP.NET Core validation problem details
            {
                // In a real app, parse the JSON from 'content' to extract specific field errors.
                return "Please check your input. The server reported validation errors.";
            }
            if (primaryMessage.Contains("400")) return "Invalid data submitted. Please check your entries.";
            if (primaryMessage.Contains("500")) return "Server error. Please try again later.";
            return primaryMessage;
        }

        private async Task<bool> ValidateInputAsync()
        {
            // Clear previous errors
            FirstNameError = LastNameError = EmailError = PhoneNumberError = LongitudeError = LatitudeError = GeneralError = null;
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(FirstName)) { FirstNameError = "First name is required."; isValid = false; }
            if (string.IsNullOrWhiteSpace(LastName)) { LastNameError = "Last name is required."; isValid = false; }
            if (string.IsNullOrWhiteSpace(Email)) { EmailError = "Email is required."; isValid = false; }
            else if (!IsValidEmail(Email)) { EmailError = "Please enter a valid email address."; isValid = false; }
            if (string.IsNullOrWhiteSpace(PhoneNumber)) { PhoneNumberError = "Phone number is required."; isValid = false; }
            
            if (!string.IsNullOrWhiteSpace(UserLongitude) && !double.TryParse(UserLongitude, out _)) { LongitudeError = "Invalid longitude value."; isValid = false; }
            if (!string.IsNullOrWhiteSpace(UserLatitude) && !double.TryParse(UserLatitude, out _)) { LatitudeError = "Invalid latitude value."; isValid = false; }

            if (!isValid && string.IsNullOrEmpty(GeneralError))
            {
                GeneralError = "Please correct the highlighted fields.";
            }
            return isValid;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try { var addr = new System.Net.Mail.MailAddress(email.Trim()); return addr.Address == email.Trim(); }
            catch { return false; }
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value)) return false;
            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}