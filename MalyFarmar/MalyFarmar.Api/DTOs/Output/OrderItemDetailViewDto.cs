namespace MalyFarmar.Api.DTOs.Output;

public class OrderItemDetailViewDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public decimal PricePerUnit { get; set; }
    public ProductDetailViewDto Product { get; set; }
}