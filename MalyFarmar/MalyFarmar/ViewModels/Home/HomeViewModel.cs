using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Pages;
using MalyFarmar.Clients;
using MalyFarmar.Mappers;
using MalyFarmar.Models.Home;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels.Shared;

namespace MalyFarmar.ViewModels.Home;

public partial class HomeViewModel : BaseViewModel
{
    private readonly ApiClient _apiClient;
    private readonly IPreferencesService _preferencesService;

    [ObservableProperty]
    public partial HomeDetailModel? Model { get; set; }

    public HomeViewModel(ApiClient apiClient, IPreferencesService preferencesService)
    {
        _apiClient = apiClient;
        _preferencesService = preferencesService;
    }

    public override async Task OnAppearingAsync()
    {
        ForceDataRefresh = true;
        await base.OnAppearingAsync();
    }

    protected override async Task LoadDataAsync()
    {
        var currentUserId = _preferencesService.GetCurrentUserId() ?? throw new Exception("User ID not found");

        var fetched = await _apiClient.GetUserSummaryAsync(currentUserId);

        if (fetched == null)
        {
            throw new Exception("User not found");
        }

        Model = fetched.ToHomeDetailModel();
    }

    [RelayCommand]
    private async Task NavigateToMyOrdersAsync()
    {
        // TODO: Implement navigation to My Orders page
        var toast = Toast.Make("TODO: Implement Me!");
        await toast.Show();
    }

    [RelayCommand]
    private async Task NavigateToMyActiveReservationsAsync()
    {
        await Shell.Current.GoToAsync(nameof(MyReservationsPage));
    }
}