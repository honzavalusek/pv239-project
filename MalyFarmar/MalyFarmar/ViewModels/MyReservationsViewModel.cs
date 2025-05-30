using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Clients;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels.Shared;
using MalyFarmar.Resources.Strings;
using System.Collections.ObjectModel;
using MalyFarmar.Pages;

namespace MalyFarmar.ViewModels
{
    public partial class MyReservationsViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _preferencesService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
        private bool _isBusy = false;

        [ObservableProperty]
        private bool _isRefreshing = false;

        [ObservableProperty]
        private string? _statusMessage;

        private bool CanExecuteRefresh() => !IsBusy;

        public ObservableCollection<OrderListViewDto> Reservations { get; }

        public MyReservationsViewModel(ApiClient apiClient, IPreferencesService preferencesService)
        {
            _apiClient = apiClient;
            _preferencesService = preferencesService;
            Reservations = new ObservableCollection<OrderListViewDto>();
        }

        public override async Task OnAppearingAsync()
        {
            ForceDataRefresh = true;
            await base.OnAppearingAsync();
        }

        protected override async Task LoadDataAsync()
        {
            await ExecuteLoadReservationsAsync();
        }

        [RelayCommand(CanExecute = nameof(CanExecuteRefresh), IncludeCancelCommand = true)]
        private async Task Refresh(CancellationToken cancellationToken)
        {
            IsRefreshing = true;
            try
            {
                await ExecuteLoadReservationsAsync(cancellationToken: cancellationToken);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task ExecuteLoadReservationsAsync(bool isRefresh = false, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested || IsBusy)
            {
                System.Diagnostics.Debug.WriteLine("[MyReservationsVM] Load cancelled before starting.");
                return;
            }

            IsBusy = true;
            StatusMessage = null;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Reservations.Clear();

                var sellerId = _preferencesService.GetCurrentUserId();

                if (sellerId == null)
                {
                    StatusMessage = MyReservationsPageStrings.StatusCurrentUserError;
                    return;
                }

                cancellationToken.ThrowIfCancellationRequested();
                var ordersListDto = await _apiClient.GetReservationsAsync(sellerId.Value, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (ordersListDto?.Orders != null)
                {
                    foreach (var order in ordersListDto.Orders)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (order.StatusId != OrderStatusEnum.Completed)
                        {
                            Reservations.Add(order);
                        }
                    }

                    if (!Reservations.Any())
                    {
                        StatusMessage = MyReservationsPageStrings.StatusNoReservations;
                        return;
                    }

                    StatusMessage = null;
                }
                else
                {
                    StatusMessage = MyReservationsPageStrings.StatusNoReservationsFoundOrError;
                }
                StatusMessage = Reservations.Any() ? null : MyReservationsPageStrings.StatusNoReservations;
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

        [RelayCommand]
        private async Task NavigateToHomeAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand(CanExecute = nameof(CanExecuteRefresh))]
        private async Task GoToReservationDetailAsync(OrderListViewDto? order)
        {
            if (order == null)
            {
                return;
            }

            await Shell.Current.GoToAsync(nameof(OrderDetailPage), new Dictionary<string, object>
            {
                [nameof(OrderDetailViewModel.OrderId)] = order.Id
            });
        }
    }
}