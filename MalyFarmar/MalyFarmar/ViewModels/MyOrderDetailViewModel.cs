using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Clients;
using MalyFarmar.Resources.Strings;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels.Shared;

namespace MalyFarmar.ViewModels
{
    // “OrderId” comes from the query parameter when navigating.
    [QueryProperty(nameof(OrderId), "OrderId")]
    public partial class MyOrderDetailViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _prefs;

        [ObservableProperty]
        private OrderDetailViewDto? _orderDetail;

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string? _errorMessage;

        public int OrderId { get; set; }

        public MyOrderDetailViewModel(ApiClient apiClient, IPreferencesService prefs)
        {
            _apiClient = apiClient;
            _prefs = prefs;
        }

        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
            if (OrderId > 0)
            {
                await LoadOrderAsync();
            }
        }

        private async Task LoadOrderAsync()
        {
            if (OrderId <= 0)
            {
                ErrorMessage = MyOrderDetailPageStrings.ErrorOrderIdInvalid;
                return;
            }

            IsLoading = true;
            ErrorMessage = null;

            try
            {
                var detail = await _apiClient.GetOrderAsync(OrderId);
                if (detail == null)
                {
                    ErrorMessage = MyOrderDetailPageStrings.ErrorOrderNotFound;
                    return;
                }

                // Verify the current user is indeed the buyer
                var me = _prefs.GetCurrentUserId();
                if (!me.HasValue || detail.Buyer.Id != me.Value)
                {
                    ErrorMessage = MyOrderDetailPageStrings.ErrorNotBuyer;
                    OrderDetail = null;
                    return;
                }

                OrderDetail = detail;
            }
            catch (ApiException aex)
            {
                ErrorMessage = $"{MyOrderDetailPageStrings.ErrorFailedToLoadOrderPrefix}: {aex.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{MyOrderDetailPageStrings.ErrorUnexpectedPrefix}: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanCancelOrder()
        {
            return OrderDetail != null
                   && !IsLoading
                   && OrderDetail.StatusId == OrderStatusEnum.PickUpSet; // Only if Pending
        }

        [RelayCommand(CanExecute = nameof(CanCancelOrder))]
        private async Task CancelOrderAsync()
        {
            if (OrderDetail == null) return;

            IsLoading = true;
            try
            {
                await _apiClient.SetOrderCompletedAsync(OrderDetail.Id);
                // After canceling, pop back to the orders list:
                await Shell.Current.GoToAsync("..");
            }
            catch (ApiException aex)
            {
                ErrorMessage = $"{MyOrderDetailPageStrings.ErrorFailedToCancelPrefix}: {aex.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{MyOrderDetailPageStrings.ErrorUnexpectedPrefix}: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
