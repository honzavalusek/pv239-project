using MalyFarmar.Api.BusinessLayer.DTOs.Input;
using MalyFarmar.Api.BusinessLayer.DTOs.Output;
using MalyFarmar.Api.BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MalyFarmar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [Route("{orderId:int}")]
    public async Task<ActionResult<OrderDetailViewDto>> GetOrder([FromRoute] int orderId)
    {
        var orderDto = await _orderService.GetOrder(orderId);

        if (orderDto == null)
        {
            return NotFound();
        }

        return Ok(orderDto);
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<OrderDetailViewDto>> CreateOrder([FromBody] OrderCreateDto orderDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdOrder = await _orderService.CreateOrder(orderDto);
        return Ok(createdOrder);
    }

    [HttpPost]
    [Route("{orderId:int}/set-pick-up-date-time")]
    public async Task<ActionResult> SetPickUpDateTime([FromRoute] int orderId, [FromBody] OrderSetPickUpDateDto pickUpDateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _orderService.SetPickUpDateTime(orderId, pickUpDateDto);

        if (!result)
        {
            return NotFound();
        }

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

        var result = await _orderService.SetOrderCompleted(orderId);

        if (!result)
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpPost]
    [Route("get-orders/{buyerId:int}")]
    public async Task<ActionResult<OrdersListDto>> GetOrders([FromRoute] int buyerId)
    {
        var ordersDto = await _orderService.GetOrdersByBuyer(buyerId);
        return Ok(ordersDto);
    }

    [HttpPost]
    [Route("ger-reservations/{sellerId:int}")]
    public async Task<ActionResult<OrdersListDto>> GetReservations([FromRoute] int sellerId)
    {
        var ordersDto = await _orderService.GetReservationsBySeller(sellerId);
        return Ok(ordersDto);
    }
}
