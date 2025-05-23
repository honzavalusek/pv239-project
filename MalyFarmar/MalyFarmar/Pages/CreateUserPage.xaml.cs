using MalyFarmar.ViewModels;
using MalyFarmar.Pages.Shared;

namespace MalyFarmar.Pages
{
    public partial class CreateUserPage : BaseContentPage
    {
        public CreateUserPage(CreateUserViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}