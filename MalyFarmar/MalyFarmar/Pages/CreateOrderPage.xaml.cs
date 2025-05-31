using MalyFarmar.ViewModels;
using MalyFarmar.Pages.Shared;

namespace MalyFarmar.Pages
{
    public partial class CreateOrderPage : BaseContentPage
    {
        public CreateOrderPage(CreateOrderViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

    }
}