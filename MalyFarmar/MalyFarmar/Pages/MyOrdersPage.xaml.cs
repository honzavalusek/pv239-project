using MalyFarmar.ViewModels;
using MalyFarmar.Pages.Shared;

namespace MalyFarmar.Pages
{
    public partial class MyOrdersPage : BaseContentPage
    {
        public MyOrdersPage(MyOrdersPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

    }
}