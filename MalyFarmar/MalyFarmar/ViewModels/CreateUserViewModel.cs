using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Clients;
using MalyFarmar.Resources.Strings;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels.Shared;
using System.Globalization;
using CommunityToolkit.Maui.Alerts;


namespace MalyFarmar.ViewModels
{
    public partial class CreateUserViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _preferencesService;
        private readonly ILocationService _locationService;

        [ObservableProperty]
        private string? _firstName;
        [ObservableProperty]
        private string? _lastName;
        [ObservableProperty]
        private string? _email;
        [ObservableProperty]
        private string? _phoneNumber;

        [ObservableProperty]
        private string? _userLongitude;
        [ObservableProperty]
        private string? _userLatitude;

        [ObservableProperty]
        private string? _firstNameError;
        [ObservableProperty]
        private string? _lastNameError;
        [ObservableProperty]
        private string? _emailError;
        [ObservableProperty]
        private string? _phoneNumberError;

        [ObservableProperty]
        private string? _generalError;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GetLocationCommand))]
        [NotifyCanExecuteChangedFor(nameof(CreateAccountCommand))]
        private bool _isProcessing = false;

        [ObservableProperty]
        private double? _fetchedLatitude;
        [ObservableProperty]
        private double? _fetchedLongitude;

        [ObservableProperty]
        private string? _locationStatus;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GetLocationCommand))]
        private bool _isCheckingLocation = false;

        private CancellationTokenSource? _cancelTokenSource;

        public CreateUserViewModel(ApiClient apiClient, IPreferencesService preferencesService, ILocationService locationService)
        {
            _apiClient = apiClient;
            _preferencesService = preferencesService;
            _locationService = locationService;
        }

        private bool CanGetLocation() => !IsProcessing && !IsCheckingLocation;

        [RelayCommand(CanExecute = nameof(CanGetLocation))]
        private async Task GetLocationAsync()
        {
            IsCheckingLocation = true;
            IsProcessing = true;
            LocationStatus = "Fetching location...";
            FetchedLatitude = null;
            FetchedLongitude = null;
            UserLatitude = string.Empty;
            UserLongitude = string.Empty;

            try
            {
                var locationResult = await _locationService.GetCurrentLocationAsync();

                if (locationResult.Location != null)
                {
                    FetchedLatitude = locationResult.Location.Latitude;
                    FetchedLongitude = locationResult.Location.Longitude;
                    UserLatitude = locationResult.Location.Latitude.ToString("F6", CultureInfo.InvariantCulture);
                    UserLongitude = locationResult.Location.Longitude.ToString("F6", CultureInfo.InvariantCulture);
                    LocationStatus = $"{LocationStrings.LocationAcquiredMessage}: Lat {UserLatitude}, Lon {UserLongitude}";
                }
                else if (!string.IsNullOrEmpty(locationResult.ErrorMessage))
                {
                    LocationStatus = locationResult.ErrorMessage;
                }
                else
                {
                    LocationStatus = LocationStrings.UnableToRetrieveLocationMessage;
                }
            }
            catch (Exception ex)
            {
                LocationStatus = $"Error fetching location: {ex.Message}";
            }
            finally
            {
                IsCheckingLocation = false;
                IsProcessing = false;
            }
        }

        private bool CanCreateAccount() => !IsProcessing;

        [RelayCommand(CanExecute = nameof(CanCreateAccount))]
        private async Task CreateAccountAsync()
        {
            if (!await ValidateInputAsync())
            {
                return;
            }

            IsProcessing = true;
            GeneralError = null;

            try
            {
                var userCreateDto = new UserCreateDto
                {
                    FirstName = FirstName?.Trim(),
                    LastName = LastName?.Trim(),
                    Email = Email?.Trim(),
                    PhoneNumber = PhoneNumber?.Trim(),
                    UserLongitude = FetchedLongitude,
                    UserLatitude = FetchedLatitude
                };

                var createdUser = await _apiClient.CreateUserAsync(userCreateDto);

                if (createdUser == null || createdUser.Id <= 0)
                {
                    GeneralError = CreateUserPageStrings.FailedToCreateAccountMessage;
                    return;
                }

                var toast = Toast.Make(CreateUserPageStrings.AccountCreatedAlertDescription);
                await toast.Show();

                _preferencesService.SetCurrentUserId(createdUser.Id);

                var app = Application.Current as App;
                app?.SwitchToShell();
            }
            catch (ApiException apiEx)
            {
                GeneralError = CleanApiErrorMessage(apiEx.Message, apiEx.Response);
            }
            catch (Exception ex)
            {
                GeneralError = CommonStrings.UnexpectedErrorMessage;
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private string CleanApiErrorMessage(string primaryMessage, string content)
        {
            if (!string.IsNullOrWhiteSpace(content) && content.Contains("errors"))
            {
                return CommonStrings.ValidationErrorsMessage;
            }

            if (primaryMessage.Contains("400"))
            {
                return CommonStrings.InvalidDataMessage;
            }

            if (primaryMessage.Contains("500"))
            {
                return CommonStrings.ServerErrorMessage;
            }

            return string.IsNullOrEmpty(content) ? primaryMessage : $"{primaryMessage} Details: {content}";
        }

        private async Task<bool> ValidateInputAsync()
        {
            FirstNameError = LastNameError = EmailError = PhoneNumberError = GeneralError = null;
            var isValid = true;

            if (string.IsNullOrWhiteSpace(FirstName))
            {
                FirstNameError = CreateUserPageStrings.FirstNameRequiredMessage;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                LastNameError = CreateUserPageStrings.LastNameRequiredMessage;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                EmailError = CreateUserPageStrings.EmailRequiredMessage;
                isValid = false;
            }
            else if (!IsValidEmail(Email))
            {
                EmailError = CreateUserPageStrings.InvalidEmailMessage;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                PhoneNumberError = CreateUserPageStrings.PhoneNumberRequiredMessage;
                isValid = false;
            }

            if (!isValid && string.IsNullOrEmpty(GeneralError))
            {
                GeneralError = CreateUserPageStrings.CorrectTheHighlightedFieldsMessage;
            }

            return isValid;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email.Trim());
                return addr.Address == email.Trim();
            }
            catch
            {
                return false;
            }
        }
    }
}