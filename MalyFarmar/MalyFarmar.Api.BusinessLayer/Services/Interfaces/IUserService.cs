using MalyFarmar.Api.BusinessLayer.DTOs.Input;
using MalyFarmar.Api.BusinessLayer.DTOs.Output;
using MalyFarmar.Api.DAL.Models;

namespace MalyFarmar.Api.BusinessLayer.Services.Interfaces;

public interface IUserService
{
    public Task<UserViewDto?> GetUser(int userId);
    public Task<UsersListDto> GetAllUsers();
    public Task<UserViewDto> CreateUser(UserCreateDto userDto);
    public Task<bool> SetUserLocation(int userId, UserSetLocationDto userSetLocationDto);
    public Task<UserSummaryDto?> GetUserSummary(int userId);
}