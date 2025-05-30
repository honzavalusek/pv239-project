using MalyFarmar.Api.BusinessLayer.DTOs.Input;
using MalyFarmar.Api.BusinessLayer.DTOs.Output;
using MalyFarmar.Api.BusinessLayer.Mappers;
using MalyFarmar.Api.BusinessLayer.Services.Interfaces;
using MalyFarmar.Api.DAL.Data;
using MalyFarmar.Api.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace MalyFarmar.Api.BusinessLayer.Services;

public class OrderService : IOrderService
{
    private readonly MalyFarmarDbContext _context;

    public OrderService(MalyFarmarDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDetailViewDto?> GetOrder(int orderId)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId);

        return order?.MapToDetailViewDto();
    }

    public async Task<OrderDetailViewDto> CreateOrder(OrderCreateDto orderDto)
    {
        var order = orderDto.MapToEntity();

        var addResult = await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        await addResult.Reference(o => o.Buyer).LoadAsync();
        await addResult.Collection(o => o.Items).LoadAsync();

        foreach (var item in addResult.Entity.Items)
        {
            await _context.Entry(item).Reference(i => i.Product).LoadAsync();

            await _context.Entry(item.Product).Reference(p => p.Seller).LoadAsync();
        }

        return addResult.Entity.MapToDetailViewDto();
    }

    public async Task<bool> SetPickUpDateTime(int orderId, OrderSetPickUpDateDto pickUpDateDto)
    {
        var order = await _context.Orders.FindAsync(orderId);

        if (order == null)
        {
            return false;
        }

        order.PickUpAt = pickUpDateDto.PickUpAt;
        order.StatusId = OrderStatusEnum.PickUpSet;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SetOrderCompleted(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);

        if (order == null)
        {
            return false;
        }

        order.StatusId = OrderStatusEnum.Completed;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<OrdersListDto> GetOrdersByBuyer(int buyerId)
    {
        var orders = await _context.Orders
            .Where(o => o.BuyerId == buyerId)
            .ToListAsync();

        return new OrdersListDto
        {
            Orders = orders.Select(o => o.MapToListViewDto()).ToList()
        };
    }

    public async Task<OrdersListDto> GetReservationsBySeller(int sellerId)
    {
        var orders = await _context.Orders
            .Where(o => o.Items.Any(i => i.Product.SellerId == sellerId))
            .ToListAsync();

        return new OrdersListDto
        {
            Orders = orders.Select(o => o.MapToListViewDto()).ToList()
        };
    }
}
