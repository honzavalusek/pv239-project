namespace MalyFarmar.Api.DTOs.Output;

public class ProductListViewDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal RemainingAmount { get; set; }
    public string Unit { get; set; }
    public decimal PricePerUnit { get; set; }
    public double? DistanceInMeters { get; set; }
    public string? ImageUrl { get; set; }
}
