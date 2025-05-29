using MalyFarmar.ViewModels;
using MalyFarmar.Pages.Shared;

namespace MalyFarmar.Pages
{
    public partial class MyReservationsPage : BaseContentPage
    {
        public MyReservationsPage(MyReservationsViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
