namespace MalyFarmar.Api.DTOs.Input;

public class ProductCreateDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal TotalAmount { get; set; }
    public string Unit { get; set; }
    public decimal PricePerUnit { get; set; }
    public int SellerId { get; set; }
}