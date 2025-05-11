using MalyFarmar.Api.BusinessLayer.DTOs.Input;
using MalyFarmar.Api.BusinessLayer.DTOs.Output;

namespace MalyFarmar.Api.BusinessLayer.Services.Interfaces;

public interface IOrderService
{
    public Task<OrderDetailViewDto?> GetOrder(int orderId);
    public Task<OrderDetailViewDto> CreateOrder(OrderCreateDto orderDto);
    public Task<bool> SetPickUpDateTime(int orderId, OrderSetPickUpDateDto pickUpDateDto);
    public Task<bool> SetOrderCompleted(int orderId);
    public Task<OrdersListDto> GetOrdersByBuyer(int buyerId);
    public Task<OrdersListDto> GetReservationsBySeller(int sellerId);
}
