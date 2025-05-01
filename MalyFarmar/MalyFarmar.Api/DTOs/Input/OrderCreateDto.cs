using System.ComponentModel.DataAnnotations;

namespace MalyFarmar.Api.DTOs.Input;

public class OrderCreateDto
{
    [Required]
    public int BuyerId { get; set; }

    public DateTime? PickUpAt { get; set; }

    [Required]
    public List<OrderItemCreateDto> Items { get; set; }
}