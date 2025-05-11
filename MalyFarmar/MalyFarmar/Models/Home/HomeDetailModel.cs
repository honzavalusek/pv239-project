using CommunityToolkit.Mvvm.ComponentModel;
using MalyFarmar.Models.Shared;

namespace MalyFarmar.Models.Home;

public partial class HomeDetailModel : BaseModel
{
    public required int Id { get; set; }

    [ObservableProperty]
    public partial string FirstName { get; set; }

    [ObservableProperty]
    public partial int NumberOfOrders { get; set; }

    [ObservableProperty]
    public partial int NumberOfActiveReservations { get; set; }
}