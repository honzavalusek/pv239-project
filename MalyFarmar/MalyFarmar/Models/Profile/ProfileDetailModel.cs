using CommunityToolkit.Mvvm.ComponentModel;
using MalyFarmar.Models.Shared;

namespace MalyFarmar.Models.Profile;

public partial class ProfileDetailModel : BaseModel
{
    public required int Id { get; set; }

    [ObservableProperty]
    public partial string FirstName { get; set; }

    [ObservableProperty]
    public partial string LastName { get; set; }

    [ObservableProperty]
    public partial string Email { get; set; }

    [ObservableProperty]
    public partial string PhoneNumber { get; set; }

    [ObservableProperty]
    public partial double? UserLongitude { get; set; }

    [ObservableProperty]
    public partial double? UserLatitude { get; set; }
}