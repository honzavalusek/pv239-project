using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Clients;
using MalyFarmar.Resources.Strings;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels.Shared;

namespace MalyFarmar.ViewModels
{
    [QueryProperty(nameof(OrderId), nameof(OrderId))]
    public partial class OrderDetailViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _preferencesService;

        public int OrderId
        {
            get;
            set
            {
                if (SetProperty(ref field, value) && field > 0)
                {
                    LoadDataAsync();
                }
            }
        }

        [ObservableProperty]
        private OrderDetailViewDto? _orderDetail;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SetPickUpDateTimeCommand))]
        [NotifyCanExecuteChangedFor(nameof(CompleteOrderCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelOrderCommand))]
        private bool _canSetPickUpDate;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SetPickUpDateTimeCommand))]
        [NotifyCanExecuteChangedFor(nameof(CompleteOrderCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelOrderCommand))]
        private bool _canCompleteOrder;

        [ObservableProperty]
        private bool _isSellerTheCurrentUser = false;

        [ObservableProperty]
        private bool _isSellerNotTheCurrentUser = false;

        [ObservableProperty]
        private DateTime _selectedPickUpDate = DateTime.Today.AddDays(1);

        [ObservableProperty]
        private TimeSpan _selectedPickUpTime = new TimeSpan(10, 0, 0);

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SetPickUpDateTimeCommand))]
        [NotifyCanExecuteChangedFor(nameof(CompleteOrderCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelOrderCommand))]
        private bool _isSubmittingAction;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SetPickUpDateTimeCommand))]
        [NotifyCanExecuteChangedFor(nameof(CompleteOrderCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelOrderCommand))]
        private bool _isLoadingData;

        [ObservableProperty]
        private string? _errorMessage;

        private bool CanExecuteAction() => !IsSubmittingAction && !IsLoadingData && OrderDetail != null;

        private bool CanExecuteSellerOnlyAction() => CanExecuteAction() && IsSellerTheCurrentUser;

        public OrderDetailViewModel(ApiClient apiClient, IPreferencesService preferencesService)
        {
            _apiClient = apiClient;
            _preferencesService = preferencesService;
        }

        protected override async Task LoadDataAsync()
        {
            if (OrderId > 0)
            {
                await ExecuteLoadOrderDetailsAsync(CancellationToken.None);
            }
        }

        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();

            if (OrderDetail == null)
            {
                return;
            }

            UpdateActionVisibilities();
            SetPickUpDateTimeCommand.NotifyCanExecuteChanged();
            CompleteOrderCommand.NotifyCanExecuteChanged();
            CancelOrderCommand.NotifyCanExecuteChanged();
        }


        private async Task ExecuteLoadOrderDetailsAsync(CancellationToken cancellationToken)
        {
            if (IsLoadingData || OrderId == 0)
            {
                return;
            }

            IsLoadingData = true;
            ErrorMessage = null;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                OrderDetail = await _apiClient.GetOrderAsync(OrderId, cancellationToken);

                if (OrderDetail == null)
                {
                    ErrorMessage = OrderDetailPageStrings.ErrorOrderNotFound;
                    return;
                }

                if (OrderDetail.PickUpAt.HasValue)
                {
                    SelectedPickUpDate = OrderDetail.PickUpAt.Value.Date;
                    SelectedPickUpTime = OrderDetail.PickUpAt.Value.TimeOfDay;
                }

                IsSellerTheCurrentUser = _preferencesService.GetCurrentUserId() == OrderDetail.Seller.Id;
                IsSellerNotTheCurrentUser = !IsSellerTheCurrentUser;

                UpdateActionVisibilities();
            }
            catch (OperationCanceledException)
            {
                ErrorMessage = OrderDetailPageStrings.ErrorFailedToLoadDetailsPrefix;
                System.Diagnostics.Debug.WriteLine($"[OrderDetailVM] Loading cancelled for OrderId: {OrderId}");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{OrderDetailPageStrings.ErrorFailedToLoadDetailsPrefix}: {ex.Message}";
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        private void UpdateActionVisibilities()
        {
            if (OrderDetail == null)
            {
                CanSetPickUpDate = false;
                CanCompleteOrder = false;
                return;
            }

            var currentUserId = _preferencesService.GetCurrentUserId();

            CanSetPickUpDate = OrderDetail.StatusId == OrderStatusEnum.Created && OrderDetail.Seller.Id == currentUserId;
            CanCompleteOrder = OrderDetail.StatusId == OrderStatusEnum.PickUpSet && OrderDetail.Seller.Id == currentUserId;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteSellerOnlyAction))]
        private async Task SetPickUpDateTime(CancellationToken cancellationToken) // Method name changed for generated command
        {
            if (!CanSetPickUpDate) // Extra check, though CanExecute should handle
            {
                ErrorMessage = "Cannot set pick-up time for this order's current status.";
                return;
            }

            IsSubmittingAction = true;
            ErrorMessage = null;
            try
            {
                DateTime combinedPickUpAt = SelectedPickUpDate.Date + SelectedPickUpTime;
                if (combinedPickUpAt <= DateTime.Now)
                {
                    ErrorMessage = OrderDetailPageStrings.ErrorPickUpTimeInPast;
                    IsSubmittingAction = false;
                    return;
                }

                var dto = new OrderSetPickUpDateDto { PickUpAt = combinedPickUpAt };
                await _apiClient.SetPickUpDateTimeAsync(OrderId, dto, cancellationToken);

                var toast = Toast.Make(OrderDetailPageStrings.AlertPickUpSetSuccess);
                await toast.Show();

                await ExecuteLoadOrderDetailsAsync(cancellationToken);
            }
            catch (Exception ex) { ErrorMessage = $"{OrderDetailPageStrings.ErrorSettingPickUpTimePrefix}: {ex.Message}"; }
            finally { IsSubmittingAction = false; }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteSellerOnlyAction))]
        private async Task CompleteOrder(CancellationToken cancellationToken)
        {
            if (!CanCompleteOrder)
            {
                ErrorMessage = "Cannot complete this order at its current status.";
                return;
            }

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                OrderDetailPageStrings.AlertFinishConfirmTitle,
                OrderDetailPageStrings.AlertFinishConfirmMessage,
                OrderDetailPageStrings.AlertFinishConfirmYes,
                OrderDetailPageStrings.AlertFinishConfirmCancel);

            if (!confirm)
            {
                return;
            }

            IsSubmittingAction = true;
            ErrorMessage = null;
            try
            {
                await _apiClient.SetOrderCompletedAsync(OrderId, cancellationToken);

                var toast = Toast.Make(OrderDetailPageStrings.AlertOrderCompletedSuccess);
                await toast.Show();

                await ExecuteLoadOrderDetailsAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{OrderDetailPageStrings.ErrorCompletingOrderPrefix}: {ex.Message}";
            }
            finally
            {
                IsSubmittingAction = false;
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteAction))]
        private async Task CancelOrder(CancellationToken cancellationToken)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                OrderDetailPageStrings.AlertCancelConfirmTitle,
                OrderDetailPageStrings.AlertCancelConfirmMessage,
                OrderDetailPageStrings.AlertCancelConfirmYes,
                OrderDetailPageStrings.AlertCancelConfirmCancel);

            if (!confirm)
            {
                return;
            }

            IsSubmittingAction = true;
            ErrorMessage = null;
            try
            {
                await _apiClient.SetOrderCompletedAsync(OrderId, cancellationToken);

                var toast = Toast.Make(OrderDetailPageStrings.AlertCancelCompletedSuccess);
                await toast.Show();

                await ExecuteLoadOrderDetailsAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{OrderDetailPageStrings.ErrorCompletingOrderPrefix}: {ex.Message}";
            }
            finally
            {
                IsSubmittingAction = false;
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}