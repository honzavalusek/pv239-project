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
        var user = _client.GetUserAsync(Preferences.Default.Get("CurrentUserId", 0)).Result.FullName;

        var response = await _client.GetOrdersAsync(Preferences.Default.Get("CurrentUserId", 0));

        var orders = response.Orders.Count;

        CounterBtn.Text = $"hello {user}, you have {orders} orders";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
}