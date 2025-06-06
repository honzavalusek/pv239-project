using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Clients;
using MalyFarmar.Mappers;
using MalyFarmar.Models.Profile;
using MalyFarmar.Resources.Strings;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels.Shared;

namespace MalyFarmar.ViewModels.Profile;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly ApiClient _apiClient;
    private readonly IPreferencesService _preferencesService;
    private readonly ILocationService _locationService;

    [ObservableProperty]
    public partial ProfileDetailModel? Model { get; set; }

    public ProfileViewModel(ApiClient apiClient, IPreferencesService preferencesService, ILocationService locationService)
    {
        _apiClient = apiClient;
        _preferencesService = preferencesService;
        _locationService = locationService;
    }

    public override async Task OnAppearingAsync()
    {
        Console.WriteLine($"current user: {_preferencesService.GetCurrentUserId()}");
        ForceDataRefresh = true;
        await base.OnAppearingAsync();
    }
    
    protected override async Task LoadDataAsync()
    {
        var currentUserId = _preferencesService.GetCurrentUserId() ?? throw new Exception("User ID not found");

        var fetched = await _apiClient.GetUserAsync(currentUserId);

        if (fetched == null)
        {
            throw new Exception("User not found");
        }

        Model = fetched.ToProfileDetailModel();
    }

    [RelayCommand]
    private async Task Logout()
    {
        _preferencesService.UnsetCurrentUserId();
        
        if (Application.Current is App app)
        {
            app.SwitchToLogin();
        }
    }

    [RelayCommand]
    private async Task SetLocationAsync()
    {
        var locationResult = await _locationService.GetCurrentLocationAsync();

        string message = String.Empty;

        if (locationResult.Location != null)
        {
            var setLocationDto = new UserSetLocationDto()
            {
                UserLatitude = locationResult.Location.Latitude,
                UserLongitude = locationResult.Location.Longitude
            };

            var currentUserId = _preferencesService.GetCurrentUserId() ?? throw new Exception("User ID not found");

            await _apiClient.SetUserLocationAsync(currentUserId, setLocationDto);
            message = ProfilePageStrings.LocationUpdatedSuccessfullyMessage;

            await LoadDataAsync();
        }
        else if (locationResult.ErrorMessage != null)
        {
            message = locationResult.ErrorMessage;
        }

        var toast = Toast.Make(message);
        await toast.Show();
    }
}