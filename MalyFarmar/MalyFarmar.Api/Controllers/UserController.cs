using MalyFarmar.Api.DAL.Data;
using MalyFarmar.Api.DTOs.Input;
using MalyFarmar.Api.DTOs.Output;
using MalyFarmar.Api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MalyFarmar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly MalyFarmarDbContext _context;

    public UserController(MalyFarmarDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("{userId:int}")]
    public async Task<ActionResult<UserViewDto>> GetUser([FromRoute] int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user.MapToViewDto());
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<ActionResult<UsersListDto>> GetAllUsers()
    {
        var users = await _context.Users.ToListAsync();

        return Ok(new UsersListDto
        {
            Users = users.Select(u => u.MapToListViewDto()).ToList()
        });
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser([FromBody] UserCreateDto userDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userEntity = userDto.MapToEntity();
        
        _context.Users.Add(userEntity);
        await _context.SaveChangesAsync();
        
        var createdUserDto = userEntity.MapToViewDto();

        return CreatedAtAction(nameof(GetUser), new { id = userEntity.Id }, createdUserDto);
    }

    [HttpPost]
    [Route("{userId:int}/set-location")]
    public async Task<ActionResult> SetUserLocation([FromRoute] int userId, [FromBody] UserSetLocationDto userSetLocationDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        user.LocationLongitude = userSetLocationDto.UserLongitude;
        user.LocationLatitude = userSetLocationDto.UserLatitude;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok();
    }
}