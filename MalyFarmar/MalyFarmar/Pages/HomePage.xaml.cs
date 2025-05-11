using MalyFarmar.Clients;
using MalyFarmar.Services.Interfaces;

namespace MalyFarmar.Pages;

public partial class HomePage : ContentPage
{
    private readonly ApiClient _client;
    private readonly IPreferencesService _preferencesService;

    public HomePage(ApiClient client, IPreferencesService preferencesService)
    {
        InitializeComponent();
        _client = client;
        _preferencesService = preferencesService;
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        var currentUserId = _preferencesService.GetCurrentUserId() ?? throw new Exception("User ID not found");

        var user = (await _client.GetUserAsync(currentUserId)).FullName;

        var response = await _client.GetOrdersAsync(currentUserId);

        var orders = response.Orders.Count;

        CounterBtn.Text = $"hello {user}, you have {orders} orders";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
}