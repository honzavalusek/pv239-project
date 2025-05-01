namespace MalyFarmar.Api.DTOs.Output;

public class ProductDetailViewDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string Unit { get; set; }
    public decimal PricePerUnit { get; set; }
    public UserViewDto Seller { get; set; }
}