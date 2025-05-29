using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Clients; 
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels.Shared;
using MalyFarmar.Resources.Strings; 
using System.Collections.ObjectModel;

namespace MalyFarmar.ViewModels
{
    public partial class MyReservationsViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _preferencesService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RefreshCommand))] // Only RefreshCommand in this VM for now
        private bool _isBusy;

        [ObservableProperty]
        private string? _statusMessage;

        public ObservableCollection<OrderListViewDto> Reservations { get; }

        public MyReservationsViewModel(ApiClient apiClient, IPreferencesService preferencesService)
        {
            _apiClient = apiClient;
            _preferencesService = preferencesService;
            Reservations = new ObservableCollection<OrderListViewDto>();
        }

        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
            if (!Reservations.Any() && !IsBusy) // Load only if empty and not already busy
            {
                await ExecuteLoadReservationsAsync(isRefresh: false, CancellationToken.None);
            }
        }

        private bool CanExecuteRefresh() => !IsBusy;

        [RelayCommand]
        private async Task NavigateToHomeAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
        
        [RelayCommand(CanExecute = nameof(CanExecuteRefresh))]
        private async Task Refresh(CancellationToken cancellationToken)
        {
            await ExecuteLoadReservationsAsync(isRefresh: true, cancellationToken: cancellationToken);
        }

        private async Task ExecuteLoadReservationsAsync(bool isRefresh = false, CancellationToken cancellationToken = default)
        {
            if (IsBusy) return;

            if (cancellationToken.IsCancellationRequested)
            {
                System.Diagnostics.Debug.WriteLine("[MyReservationsVM] Load cancelled before starting.");
                return;
            }

            IsBusy = true;
            if (!isRefresh) StatusMessage = null;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                // Clear reservations only if we are about to fetch new ones or it's a refresh.
                if (isRefresh || !Reservations.Any() || StatusMessage == null)
                {
                    Reservations.Clear();
                }

                var sellerId = _preferencesService.GetCurrentUserId();
                if (sellerId == null)
                {
                    StatusMessage = MyReservationsPageStrings.StatusCurrentUserError; // Use new stringsmmmm
                }
                else
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var ordersListDto = await _apiClient.GetReservationsAsync(sellerId.Value, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    if (ordersListDto?.Orders != null)
                    {
                        foreach (var order in ordersListDto.Orders)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            
                            // Filter out finished orders 
                            if (order.StatusId != OrderStatusEnum._3)
                            {
                                Reservations.Add(order);
                            }
                        }

                        if (!Reservations.Any()) StatusMessage = MyReservationsPageStrings.StatusNoReservations;
                        else StatusMessage = null;
                    }
                    else
                    {
                        StatusMessage = MyReservationsPageStrings.StatusNoReservationsFoundOrError;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[MyReservationsVM] Reservation loading cancelled.");
                StatusMessage = MyReservationsPageStrings.StatusLoadingCancelled;
            }
            catch (ApiException apiEx)
            {
                StatusMessage = $"{MyReservationsPageStrings.StatusFailedToLoadPrefix}: {apiEx.Message}";
                System.Diagnostics.Debug.WriteLine($"[MyReservationsVM] API Error: {apiEx.StatusCode} - {apiEx.Message}");
            }
            catch (Exception ex)
            {
                StatusMessage = $"{MyReservationsPageStrings.StatusUnexpectedErrorPrefix}: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[MyReservationsVM] Generic Error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // [RelayCommand(CanExecute = nameof(CanExecuteRefresh))] // Example, use appropriate CanExecute
        // private async Task GoToReservationDetailAsync(OrderListViewDto order)
        // {
        //     if (order == null) return;
        //     await Shell.Current.GoToAsync($"{nameof(ReservationDetailPage)}?OrderId={order.Id}");
        // }
    }
}