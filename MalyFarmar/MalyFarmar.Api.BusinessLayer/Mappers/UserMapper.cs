using Common.Enums;
using MalyFarmar.Api.DAL.Models;
using MalyFarmar.Api.BusinessLayer.DTOs.Input;
using MalyFarmar.Api.BusinessLayer.DTOs.Output;

namespace MalyFarmar.Api.BusinessLayer.Mappers;

public static class UserMapper
{
    public static UserViewDto MapToViewDto(this User entity)
    {
        return new UserViewDto()
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            UserLongitude = entity.LocationLongitude,
            UserLatitude = entity.LocationLatitude,
            FullName = entity.FullName
        };
    }

    public static UserSummaryDto MapToSummaryDto(this User entity)
    {
        return new UserSummaryDto()
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            NumberOfOrders = entity.Orders.Count,
            NumberOfActiveReservations = entity.Products
                .SelectMany(product => product.OrderItems)
                .Select(orderItem => orderItem.Order)
                .Count(o => o.StatusId is OrderStatusEnum.Created or OrderStatusEnum.PickUpSet)
        };
    }

    public static User MapToEntity(this UserCreateDto dto)
    {
        return new User()
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            LocationLongitude = dto.UserLongitude,
            LocationLatitude = dto.UserLatitude,
        };
    }

    public static UserListViewDto MapToListViewDto(this User entity)
    {
        return new UserListViewDto()
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            FullName = entity.FullName
        };
    }
}