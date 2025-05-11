namespace MalyFarmar.Api.BusinessLayer.DTOs.Output;

public class UserSummaryDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public int NumberOfOrders { get; set; }
    public int NumberOfActiveReservations { get; set; }
}