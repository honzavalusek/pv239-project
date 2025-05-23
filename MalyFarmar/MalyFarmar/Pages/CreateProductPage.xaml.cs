using MalyFarmar.ViewModels;
using MalyFarmar.Pages.Shared;

namespace MalyFarmar.Pages
{
    public partial class CreateProductPage : BaseContentPage 
    {
        public CreateProductPage(CreateProductViewModel viewModel) : base(viewModel) 
        {
            InitializeComponent();
        }
    }
}