using MalyFarmar.Api.DAL.Models;
using MalyFarmar.Api.DTOs.Input;
using MalyFarmar.Api.DTOs.Output;

namespace MalyFarmar.Api.Mappers;

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
}