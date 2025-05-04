namespace MalyFarmar.Api.DTOs.Output;

public class OrdersListDto
{
    public List<OrderListViewDto> Orders { get; set; } = new List<OrderListViewDto>();
}