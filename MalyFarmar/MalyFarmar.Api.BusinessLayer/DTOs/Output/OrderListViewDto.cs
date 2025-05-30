using MalyFarmar.Api.DAL.Enums;

namespace MalyFarmar.Api.BusinessLayer.DTOs.Output;

public class OrderListViewDto
{
    public int Id { get; set; }
    public DateTime? PickUpAt { get; set; }
    public OrderStatusEnum StatusId { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}