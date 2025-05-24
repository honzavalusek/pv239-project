using MalyFarmar.ViewModels;
using MalyFarmar.Pages.Shared;

namespace MalyFarmar.Pages
{
    public partial class LoginPage : BaseContentPage
    {
        public LoginPage(LoginViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}