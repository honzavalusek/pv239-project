using System.ComponentModel.DataAnnotations;

namespace MalyFarmar.Api.BusinessLayer.DTOs.Input;

public class ProductSearchDto
{
    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }

    public double? RadiusInMeters { get; set; }
}