using System.ComponentModel.DataAnnotations;

namespace MalyFarmar.Api.BusinessLayer.DTOs.Input;

public class ProductCreateDto
{
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }

    [Required]
    public decimal TotalAmount { get; set; }

    [Required]
    public string Unit { get; set; }

    [Required]
    public decimal PricePerUnit { get; set; }

    [Required]
    public int SellerId { get; set; }

    public byte[]? Image { get; set; }
}