// ViewModels/CreateProductViewModel.cs

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MalyFarmar.Clients; // For ApiClient, DTOs, and likely FileParameter
using Microsoft.Maui.Storage; // For FileResult
using Microsoft.Maui.Media; // For MediaPicker
using System;
using System.Globalization; // For CultureInfo
using System.IO;
using System.Threading.Tasks;
using MalyFarmar.Services.Interfaces;
using Microsoft.Maui.Controls; // For Shell, Application.Current.MainPage.DisplayAlert, Preferences

namespace MalyFarmar.ViewModels
{
    public partial class CreateProductViewModel : ObservableObject
    {
        private readonly ApiClient _apiClient;
        private readonly IPreferencesService _preferencesService;

        [ObservableProperty] private string _name;

        [ObservableProperty] private string _description;

        [ObservableProperty] private string _totalAmountStr;

        [ObservableProperty] private string _unit;

        [ObservableProperty] private string _pricePerUnitStr;

        [ObservableProperty] private FileResult? _selectedImageFile;

        [ObservableProperty] private ImageSource _productImageSource;

        [ObservableProperty] private bool _isBusy;

        [ObservableProperty] private string _errorMessage;

        public CreateProductViewModel(ApiClient apiClient, IPreferencesService preferencesService)
        {
            _apiClient = apiClient;
            _preferencesService = preferencesService;
        }

        [RelayCommand]
        private async Task PickImageAsync()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select Product Image"
                });

                if (result != null)
                {
                    // Accessing GENERATED property 'SelectedImageFile'
                    SelectedImageFile = result;
                    // Accessing GENERATED property 'ProductImageSource'
                    ProductImageSource = ImageSource.FromFile(result.FullPath);
                    // Accessing GENERATED property 'ErrorMessage'
                    ErrorMessage = null;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error picking image: {ex.Message}";
                await Application.Current.MainPage.DisplayAlert("Error", ErrorMessage, "OK");
            }
        }

        [RelayCommand]
        private async Task CreateProductAsync()
        {
            if (!ValidateInput(out double totalAmount, out double pricePerUnit))
            {
                return; // ValidateInput will set ErrorMessage
            }

            IsBusy = true;
            ErrorMessage = null;
            Stream imageStream = null; // Keep stream reference for disposal

            try
            {
                var sellerId = _preferencesService.GetCurrentUserId();
                if (sellerId == null)
                {
                    ErrorMessage = "Could not determine current user. Please log in.";
                    IsBusy = false;
                    return;
                }

                byte[] imageBytes = null;
                if (SelectedImageFile != null)
                {
                    try
                    {
                        imageStream = await SelectedImageFile.OpenReadAsync();
                        var memoryStream = new MemoryStream();
                        await imageStream.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = $"Error processing image file: {ex.Message}";
                        IsBusy = false;
                        imageStream?.Dispose();
                        return;
                    }
                }

                var productCreateDto = new ProductCreateDto
                {
                    Name = Name,
                    Description = Description,
                    TotalAmount = totalAmount,
                    Unit = Unit,
                    PricePerUnit = pricePerUnit,
                    SellerId = sellerId.Value,
                    Image = imageBytes
                };

                var result = await _apiClient.CreateProductAsync(productCreateDto);

                if (result != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Success",
                        $"Product '{productCreateDto.Name}' created!", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    ErrorMessage = "Failed to create product. Server returned an unexpected response.";
                }
            }
            catch (ApiException apiEx)
            {
                ErrorMessage = $"Creation failed: (Status {apiEx.StatusCode}) {apiEx.Message}";
                System.Diagnostics.Debug.WriteLine($"API Error {apiEx.StatusCode}: {apiEx.Message}"); // Log raw content
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An unexpected error occurred: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Generic Error: {ex}");
            }
            finally
            {
                imageStream?.Dispose();
                IsBusy = false;
            }
        }

        private bool ValidateInput(out double totalAmount, out double pricePerUnit)
        {
            totalAmount = 0;
            pricePerUnit = 0;
            ErrorMessage = null;

            if (string.IsNullOrWhiteSpace(Name))
            {
                ErrorMessage = "Product name is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Unit))
            {
                ErrorMessage = "Unit is required (e.g., kg, piece, liter).";
                return false;
            }

            // Use double.TryParse
            if (string.IsNullOrWhiteSpace(TotalAmountStr) ||
                !double.TryParse(TotalAmountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out totalAmount) ||
                totalAmount <= 0)
            {
                ErrorMessage = "Valid total amount is required (must be > 0).";
                return false;
            }

            // Use double.TryParse
            if (string.IsNullOrWhiteSpace(PricePerUnitStr) || !double.TryParse(PricePerUnitStr, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out pricePerUnit) || pricePerUnit <= 0)
            {
                ErrorMessage = "Valid price per unit is required (must be > 0).";
                return false;
            }

            return true;
        }
    }
}