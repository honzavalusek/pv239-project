namespace MalyFarmar.Api.DTOs.Output;

public class OrderListDto
{
    public List<OrderListViewDto> Orders { get; set; } = new List<OrderListViewDto>();
}