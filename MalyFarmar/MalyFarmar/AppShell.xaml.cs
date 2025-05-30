using MalyFarmar.Pages;

namespace MalyFarmar;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(CreateUserPage), typeof(CreateUserPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(SellPage), typeof(SellPage));
        Routing.RegisterRoute(nameof(ProductDetailPage), typeof(ProductDetailPage));
        Routing.RegisterRoute(nameof(EditProductPage), typeof(EditProductPage));
        Routing.RegisterRoute(nameof(CreateProductPage), typeof(CreateProductPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        Routing.RegisterRoute(nameof(MyReservationsPage), typeof(MyReservationsPage));
        Routing.RegisterRoute(nameof(OrderDetailPage), typeof(OrderDetailPage));

    }
}