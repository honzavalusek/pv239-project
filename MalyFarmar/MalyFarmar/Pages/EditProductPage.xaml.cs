// Pages/EditProductPage.xaml.cs
using MalyFarmar.ViewModels;
using Microsoft.Maui.Controls;

namespace MalyFarmar.Pages
{
    [QueryProperty(nameof(ProductId), "productId")]
    public partial class EditProductPage : ContentPage
    {
        // This property will be set by the Shell navigation system
        private int _productIdValue;
        public int ProductId
        {
            get => _productIdValue;
            set
            {
                _productIdValue = value;
                // When ProductId is set on the page, pass it to the (already injected) ViewModel
                if (BindingContext is EditProductViewModel viewModel)
                {
                    viewModel.ProductId = value;
                }
            }
        }

        public EditProductPage(EditProductViewModel viewModel) // ViewModel is injected by DI
        {
            InitializeComponent();
            BindingContext = viewModel;

            if (_productIdValue > 0) // If ProductId was set by QueryProperty very early
            {
                viewModel.ProductId = _productIdValue;
            }
        }

    }
}