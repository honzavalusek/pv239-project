namespace MalyFarmar.Api.BusinessLayer.DTOs.Output;

public class OrdersListDto
{
    public List<OrderListViewDto> Orders { get; set; } = new List<OrderListViewDto>();
}