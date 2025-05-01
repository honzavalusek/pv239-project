using System.ComponentModel.DataAnnotations;

namespace MalyFarmar.Api.DTOs.Input;

public class UserCreateDto
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string PhoneNumber { get; set; }
    public double? UserLongitude { get; set; }
    public double? UserLatitude { get; set; }
}