using MalyFarmar.ViewModels;

namespace MalyFarmar.Pages
{
    public partial class ProductDetailPage : ContentPage
    {
        public ProductDetailPage(ProductDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is ProductDetailViewModel viewModel)
            {
                if (viewModel.ProductId > 0)
                {
                    await viewModel.LoadProductDetailsAsync();
                }
            }
        }
    }
}