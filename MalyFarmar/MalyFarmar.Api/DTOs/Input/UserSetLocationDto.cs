using System.ComponentModel.DataAnnotations;

namespace MalyFarmar.Api.DTOs.Input;

public class UserSetLocationDto
{
    [Required]
    public double UserLongitude { get; set; }

    [Required]
    public double UserLatitude { get; set; }
}