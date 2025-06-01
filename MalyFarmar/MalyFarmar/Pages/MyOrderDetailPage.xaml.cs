using MalyFarmar.ViewModels;
using MalyFarmar.Pages.Shared;

namespace MalyFarmar.Pages
{
    public partial class MyOrderDetailPage : BaseContentPage
    {
        public MyOrderDetailPage(MyOrderDetailViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

    }
}