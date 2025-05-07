using Common.Enums;
using MalyFarmar.Api.DAL.Models;
using MalyFarmar.Api.DTOs.Input;
using MalyFarmar.Api.DTOs.Output;

namespace MalyFarmar.Api.Mappers;

public static class OrderMapper
{
    public static Order MapToEntity(this OrderCreateDto dto)
    {
        return new Order
        {
            BuyerId = dto.BuyerId,
            StatusId = OrderStatusEnum.Created,
            PickUpAt = dto.PickUpAt,
            Items = dto.Items.Select(i => i.MapToEntity()).ToList()
        };
    }

    public static OrderItem MapToEntity(this OrderItemCreateDto dto)
    {
        return new OrderItem
        {
            ProductId = dto.ProductId,
            Amount = dto.Amount,
            PricePerUnit = dto.PricePerUnit
        };
    }

    public static OrderDetailViewDto MapToDetailViewDto(this Order entity)
    {
        return new OrderDetailViewDto()
        {
            Id = entity.Id,
            PickUpAt = entity.PickUpAt,
            StatusId = entity.StatusId,
            Items = entity.Items.Select(i => i.MapToDetailViewDto()).ToList(),
            Seller = entity.Items.First().Product.Seller.MapToViewDto(),
            Buyer = entity.Buyer.MapToViewDto(),
            TotalPrice = entity.TotalPrice,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static OrderItemDetailViewDto MapToDetailViewDto(this OrderItem entity)
    {
        return new OrderItemDetailViewDto()
        {
            Id = entity.Id,
            Amount = entity.Amount,
            PricePerUnit = entity.PricePerUnit,
            Product = entity.Product.MapToDetailViewDto()
        };
    }

    public static OrderListViewDto MapToListViewDto(this Order entity)
    {
        return new OrderListViewDto()
        {
            Id = entity.Id,
            PickUpAt = entity.PickUpAt,
            StatusId = entity.StatusId,
            TotalPrice = entity.TotalPrice,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}