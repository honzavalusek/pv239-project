using MalyFarmar.Clients;
using MalyFarmar.Models;

namespace MalyFarmar.Mappers;

public static class UserMapper
{
    public static ProfileDetailModel ToProfileDetailModel(this UserViewDto dto)
    {
        return new ProfileDetailModel()
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            UserLongitude = dto.UserLongitude,
            UserLatitude = dto.UserLatitude
        };
    }
}