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
    public async Task<IActionResult> GetOrder([FromRoute] int orderId)
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
    public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var order = orderDto.MapToEntity();
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [Route("{orderId:int}/set-pick-up-date-time")]
    public async Task<IActionResult> SetPickUpDateTime([FromRoute] int orderId, [FromBody] OrderSetPickUpDateDto pickUpDateDto)
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
    public async Task<IActionResult> SetOrderCompleted([FromRoute] int orderId)
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
    public async Task<IActionResult> GetOrders([FromRoute] int buyerId)
    {
        var orders = await _context.Orders
            .Where(o => o.BuyerId == buyerId)
            .ToListAsync();

        return Ok(new OrderListDto
        {
            Orders = orders.Select(o => o.MapToListViewDto()).ToList()
        });
    }

    [HttpPost]
    [Route("ger-reservations/{sellerId:int}")]
    public async Task<IActionResult> GetReservations([FromRoute] int sellerId)
    {
        var orders = await _context.Orders
            .Where(o => o.Items.Any(i => i.Product.SellerId == sellerId))
            .ToListAsync();

        return Ok(new OrderListDto
        {
            Orders = orders.Select(o => o.MapToListViewDto()).ToList()
        });
    }
}