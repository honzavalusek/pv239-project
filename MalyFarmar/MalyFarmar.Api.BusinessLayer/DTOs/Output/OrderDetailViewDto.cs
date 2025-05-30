using MalyFarmar.Api.DAL.Enums;

namespace MalyFarmar.Api.BusinessLayer.DTOs.Output;

public class OrderDetailViewDto
{
    public int Id { get; set; }
    public DateTime? PickUpAt { get; set; }
    public OrderStatusEnum StatusId { get; set; }
    public List<OrderItemDetailViewDto> Items { get; set; }
    public UserViewDto Seller { get; set; }
    public UserViewDto Buyer { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}