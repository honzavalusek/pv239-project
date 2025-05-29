using MalyFarmar.Pages.Shared;
using MalyFarmar.ViewModels;

namespace MalyFarmar.Pages
{
    public partial class OrderDetailPage : BaseContentPage
    {
        public OrderDetailPage(OrderDetailViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}