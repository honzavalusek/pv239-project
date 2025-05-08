using Common.Enums;
using MalyFarmar.Api.DAL.Data;
using MalyFarmar.Api.DTOs.Input;
using MalyFarmar.Api.DTOs.Output;
using MalyFarmar.Api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MalyFarmar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : Controller
{
    private readonly MalyFarmarDbContext _context;

    public OrderController(MalyFarmarDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("{orderId:int}")]
    public async Task<ActionResult<OrderDetailViewDto>> GetOrder([FromRoute] int orderId)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order.MapToDetailViewDto());
    }


    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<OrderDetailViewDto>> CreateOrder([FromBody] OrderCreateDto orderDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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

        return Ok(addResult.Entity.MapToDetailViewDto());
    }

    [HttpPost]
    [Route("{orderId:int}/set-pick-up-date-time")]
    public async Task<ActionResult> SetPickUpDateTime([FromRoute] int orderId, [FromBody] OrderSetPickUpDateDto pickUpDateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var order = await _context.Orders.FindAsync(orderId);

        if (order == null)
        {
            return NotFound();
        }

        order.PickUpAt = pickUpDateDto.PickUpAt;
        order.StatusId = OrderStatusEnum.PickUpSet;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [Route("{orderId:int}/set-order-completed")]
    public async Task<ActionResult> SetOrderCompleted([FromRoute] int orderId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var order = await _context.Orders.FindAsync(orderId);

        if (order == null)
        {
            return NotFound();
        }

        order.StatusId = OrderStatusEnum.Completed;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [Route("get-orders/{buyerId:int}")]
    public async Task<ActionResult<OrdersListDto>> GetOrders([FromRoute] int buyerId)
    {
        var orders = await _context.Orders
            .Where(o => o.BuyerId == buyerId)
            .ToListAsync();

        return Ok(new OrdersListDto
        {
            Orders = orders.Select(o => o.MapToListViewDto()).ToList()
        });
    }

    [HttpPost]
    [Route("ger-reservations/{sellerId:int}")]
    public async Task<ActionResult<OrdersListDto>> GetReservations([FromRoute] int sellerId)
    {
        var orders = await _context.Orders
            .Where(o => o.Items.Any(i => i.Product.SellerId == sellerId))
            .ToListAsync();

        return Ok(new OrdersListDto
        {
            Orders = orders.Select(o => o.MapToListViewDto()).ToList()
        });
    }
}