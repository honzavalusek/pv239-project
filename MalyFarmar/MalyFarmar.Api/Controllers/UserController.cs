using MalyFarmar.Api.BusinessLayer.DTOs.Input;
using MalyFarmar.Api.BusinessLayer.DTOs.Output;
using MalyFarmar.Api.BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MalyFarmar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("{userId:int}")]
    public async Task<ActionResult<UserViewDto>> GetUser([FromRoute] int userId)
    {
        var userDto = await _userService.GetUser(userId);

        if (userDto == null)
        {
            return NotFound();
        }

        return Ok(userDto);
    }

    [HttpGet]
    [Route("get-all")]
    public async Task<ActionResult<UsersListDto>> GetAllUsers()
    {
        return await _userService.GetAllUsers();
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<UserViewDto>> CreateUser([FromBody] UserCreateDto userDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return await _userService.CreateUser(userDto);
    }

    [HttpPost]
    [Route("{userId:int}/set-location")]
    public async Task<ActionResult> SetUserLocation([FromRoute] int userId, [FromBody] UserSetLocationDto userSetLocationDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _userService.SetUserLocation(userId, userSetLocationDto);

        if (result == false)
        {
            return NotFound();
        }

        return Ok();
    }
}