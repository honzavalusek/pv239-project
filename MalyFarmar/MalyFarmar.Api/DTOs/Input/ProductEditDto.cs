namespace MalyFarmar.Api.DTOs.Input;

public class ProductEditDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PricePerUnit { get; set; }
}