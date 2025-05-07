using MalyFarmar.Clients;

namespace MalyFarmar.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiClient _client;

    public HomePage(ApiClient client)
    {
        InitializeComponent();
        _client = client;
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        var response = await _client.GetOrdersAsync(2);

        var user = SecureStorage.Default.GetAsync("CurrentUserId").Result;

        CounterBtn.Text = $"hello {user}, User 2 has {response.Orders.Count} orders";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
}