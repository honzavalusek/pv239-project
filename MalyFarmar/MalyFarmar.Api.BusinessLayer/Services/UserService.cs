using MalyFarmar.Api.BusinessLayer.DTOs.Input;
using MalyFarmar.Api.BusinessLayer.DTOs.Output;
using MalyFarmar.Api.BusinessLayer.Mappers;
using MalyFarmar.Api.BusinessLayer.Services.Interfaces;
using MalyFarmar.Api.DAL.Data;
using MalyFarmar.Api.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MalyFarmar.Api.BusinessLayer.Services;

public class UserService : IUserService
{
    private readonly MalyFarmarDbContext _context;

    public UserService(MalyFarmarDbContext context)
    {
        _context = context;
    }

    public async Task<UserViewDto?> GetUser(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        return user?.MapToViewDto();
    }

    public async Task<UsersListDto> GetAllUsers()
    {
        var users = await _context.Users.ToListAsync();

        return new UsersListDto
        {
            Users = users.Select(u => u.MapToListViewDto()).ToList()
        };
    }

    public async Task<UserViewDto> CreateUser(UserCreateDto userDto)
    {
        var addResult = await _context.Users.AddAsync(userDto.MapToEntity());
        await _context.SaveChangesAsync();

        return addResult.Entity.MapToViewDto();
    }

    public async Task<bool> SetUserLocation(int userId, UserSetLocationDto userSetLocationDto)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return false;
        }

        user.LocationLongitude = userSetLocationDto.UserLongitude;
        user.LocationLatitude = userSetLocationDto.UserLatitude;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<UserSummaryDto?> GetUserSummary(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        return user?.MapToSummaryDto();
    }
}