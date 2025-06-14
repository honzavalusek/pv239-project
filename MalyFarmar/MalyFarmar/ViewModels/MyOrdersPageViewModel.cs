using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Clients;
using MalyFarmar.Resources.Strings;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels.Shared;
using System.Collections.ObjectModel;
using MalyFarmar.Pages;

namespace MalyFarmar.ViewModels
{
    public partial class MyOrdersPageViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _prefs;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoadOrdersCommand))]
        [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
        private bool _isBusy = false;

        [ObservableProperty]
        private bool _isRefreshing = false;

        [ObservableProperty]
        private string? _statusMessage;

        // The buyer’s orders come from OrderListViewDto
        public ObservableCollection<OrderListViewDto> Orders { get; }
            = new ObservableCollection<OrderListViewDto>();

        public MyOrdersPageViewModel(ApiClient apiClient, IPreferencesService prefs)
        {
            _apiClient = apiClient;
            _prefs = prefs;
        }

        /// <summary>
        /// Called when the page appears. If we have no orders yet (or an error),
        /// re‐load them from the server.
        /// </summary>
        protected override async Task LoadDataAsync()
        {
            await base.LoadDataAsync();
            if (Orders.Count == 0 || !string.IsNullOrEmpty(StatusMessage))
            {
                await ExecuteLoadOrdersAsync();
            }
        }

        private bool CanExecuteLoadOrRefresh() => !IsBusy;

        [RelayCommand(CanExecute = nameof(CanExecuteLoadOrRefresh), IncludeCancelCommand = true)]
        private async Task LoadOrders(CancellationToken ct)
            => await ExecuteLoadOrdersAsync(cancellationToken: ct);

        [RelayCommand(CanExecute = nameof(CanExecuteLoadOrRefresh), IncludeCancelCommand = true)]
        private async Task Refresh(CancellationToken ct)
        {
            IsRefreshing = true;
            try
            {
                await ExecuteLoadOrdersAsync(cancellationToken: ct);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        /// <summary>
        /// Actually calls GET api/Order/get-orders/{buyerId} to fetch all orders.
        /// </summary>
        private async Task ExecuteLoadOrdersAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested || IsBusy)
            {
                return;
            }

            IsBusy = true;
            StatusMessage = null;

            try
            {
                Orders.Clear();

                var me = _prefs.GetCurrentUserId();
                if (!me.HasValue)
                {
                    StatusMessage = MyOrdersPageStrings.ErrorCurrentUserUnavailable;
                    return;
                }

                // Call the API: GET /api/Order/get-orders/{buyerId}
                var result = await _apiClient.GetOrdersAsync(me.Value, cancellationToken);
                if (result?.Orders == null || result.Orders.Count == 0)
                {
                    StatusMessage = MyOrdersPageStrings.StatusNoOrdersFound;
                    return;
                }

                foreach (var o in result.Orders)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Orders.Add(o);
                }

                StatusMessage = Orders.Count > 0
                    ? null
                    : MyOrdersPageStrings.StatusNoOrdersFound;
            }
            catch (OperationCanceledException)
            {
                StatusMessage = MyOrdersPageStrings.StatusLoadingCancelled;
            }
            catch (ApiException ex)
            {
                StatusMessage = $"{MyOrdersPageStrings.StatusFailedToLoadOrdersPrefix}: {ex.Message}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"{MyOrdersPageStrings.StatusUnexpectedErrorPrefix}: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Tapping an order row navigates to the detail page.
        /// </summary>
        [RelayCommand]
        private async Task GoToOrderDetailsAsync(int orderId)
        {
            await Shell.Current.GoToAsync(nameof(OrderDetailPage), new Dictionary<string, object>
            {
                [nameof(OrderDetailViewModel.OrderId)] = orderId
            });
        }

        /// <summary>
        /// Clicking “Cancel” calls POST /api/Order/{orderId}/set-order-completed, then reloads.
        /// </summary>
        [RelayCommand]
        private async Task CancelOrderAsync(int orderId)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            try
            {
                await _apiClient.SetOrderCompletedAsync(orderId);
                await ExecuteLoadOrdersAsync();
            }
            catch (ApiException aex)
            {
                StatusMessage = $"{MyOrdersPageStrings.StatusFailedToCancelOrderPrefix}: {aex.Message}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"{MyOrdersPageStrings.StatusUnexpectedErrorPrefix}: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
