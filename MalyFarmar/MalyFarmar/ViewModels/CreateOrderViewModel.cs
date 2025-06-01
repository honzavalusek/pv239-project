using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Clients;
using MalyFarmar.Resources.Strings;
using MalyFarmar.Services.Interfaces;
using MalyFarmar.ViewModels.Shared;
using System.Globalization;

namespace MalyFarmar.ViewModels
{
    [QueryProperty(nameof(ProductId), "ProductId")]
    public partial class CreateOrderViewModel : BaseViewModel
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _prefs;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(PlaceOrderCommand))]
        ProductDetailViewDto? _product;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(PlaceOrderCommand))]
        string? _quantityStr;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(PlaceOrderCommand))]
        bool _isSubmitting;

        [ObservableProperty]
        DateTime _pickUpDate = DateTime.Now;

        [ObservableProperty]
        string? _errorMessage;

        public int ProductId { get; set; }

        public bool CanPlaceOrder =>
            !IsSubmitting
            && Product != null
            && double.TryParse(QuantityStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var q)
            && q > 0;

        public CreateOrderViewModel(ApiClient apiClient, IPreferencesService prefs)
        {
            _apiClient = apiClient;
            _prefs = prefs;
        }

        public override async Task OnAppearingAsync()
        {
            await base.OnAppearingAsync();
            if (ProductId > 0)
                await LoadProductAsync();
        }

        private async Task LoadProductAsync()
        {
            try
            {
                Product = await _apiClient.GetProductAsync(ProductId);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{CreateOrderPageStrings.ErrorLoadProductPrefix}: {ex.Message}";
            }
        }

        [RelayCommand(CanExecute = nameof(CanPlaceOrder))]
        async Task PlaceOrderAsync()
        {
            if (Product == null) return;

            if (!double.TryParse(QuantityStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var qty))
            {
                ErrorMessage = CreateOrderPageStrings.ErrorInvalidQuantity;
                return;
            }

            IsSubmitting = true;
            ErrorMessage = null;

            try
            {
                var buyerId = _prefs.GetCurrentUserId().Value;
                var dto = new OrderCreateDto
                {
                    BuyerId = buyerId,
                    PickUpAt = PickUpDate,
                    Items = new List<OrderItemCreateDto>
                    {
                        new OrderItemCreateDto
                        {
                            ProductId    = Product.Id,
                            Amount       = qty,
                            PricePerUnit = Product.PricePerUnit
                        }
                    }
                };

                var result = await _apiClient.CreateOrderAsync(dto);

                await Shell.Current.GoToAsync("..");
            }
            catch (ApiException a)
            {
                ErrorMessage = $"{CreateOrderPageStrings.ErrorPlaceOrderPrefix}: {a.Message}";
            }
            finally
            {
                IsSubmitting = false;
            }
        }
    }
}
