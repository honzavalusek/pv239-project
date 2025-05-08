using MalyFarmar.Clients;
using MalyFarmar.ViewModels;

namespace MalyFarmar.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(ApiClient client)
        {
            InitializeComponent();
            BindingContext = new LoginViewModel(client);
        }
    }
}
