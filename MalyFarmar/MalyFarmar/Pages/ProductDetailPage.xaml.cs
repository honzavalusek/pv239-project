using MalyFarmar.ViewModels;
using MalyFarmar.Pages.Shared; 

namespace MalyFarmar.Pages
{
    public partial class ProductDetailPage : BaseContentPage 
    {
        public ProductDetailPage(ProductDetailViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}