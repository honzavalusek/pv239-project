namespace MalyFarmar.Api.DTOs.Output;

public class UserViewDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public double? UserLongitude { get; set; }
    public double? UserLatitude { get; set; }
    public string FullName { get; set; }
}