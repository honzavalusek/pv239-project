using MalyFarmar.ViewModels;
using MalyFarmar.Pages.Shared;

namespace MalyFarmar.Pages
{
    [QueryProperty(nameof(ProductId), "productId")]
    public partial class EditProductPage : BaseContentPage
    {
        private int _productIdValue;
        public int ProductId
        {
            get => _productIdValue;
            set
            {
                _productIdValue = value;
                if (BindingContext is EditProductViewModel viewModel)
                {
                    viewModel.ProductId = value;
                }
            }
        }

        public EditProductPage(EditProductViewModel viewModel) : base(viewModel) // ViewModel injected
        {
            InitializeComponent();
            if (_productIdValue > 0)
            {
                viewModel.ProductId = _productIdValue;
            }
        }

    }
}