using System.ComponentModel.DataAnnotations;

namespace MalyFarmar.Api.BusinessLayer.DTOs.Input;

public class OrderItemCreateDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public decimal PricePerUnit { get; set; }
}