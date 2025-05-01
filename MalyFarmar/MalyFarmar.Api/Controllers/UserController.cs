using MalyFarmar.Api.DAL.Data;
using MalyFarmar.Api.DTOs.Input;
using MalyFarmar.Api.Mappers;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetUser([FromRoute] int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user.MapToViewDto());
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Users.Add(userDto.MapToEntity());
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [Route("{userId:int}/set-location")]
    public async Task<IActionResult> SetUserLocation([FromRoute] int userId, [FromBody] UserSetLocationDto userSetLocationDto)
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