using System.ComponentModel.DataAnnotations;

namespace MalyFarmar.Api.BusinessLayer.DTOs.Input;

public class OrderSetPickUpDateDto
{
    [Required]
    public DateTime PickUpAt { get; set; }
}