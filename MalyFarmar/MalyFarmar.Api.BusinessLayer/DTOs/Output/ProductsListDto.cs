namespace MalyFarmar.Api.BusinessLayer.DTOs.Output;

public class ProductsListDto
{
    public List<ProductListViewDto> Products { get; set; } = new List<ProductListViewDto>();
}