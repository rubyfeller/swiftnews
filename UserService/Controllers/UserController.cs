using Microsoft.AspNetCore.Mvc;
using UserService.AsyncDataServices;
using UserService.Auth0;
using UserService.Data;
using UserService.Dtos;
using UserService.Helpers;
using UserService.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/user")]
public class UsersController : ControllerBase
{
    private readonly IMessageBusClient _messageBusClient;
    private readonly IUserRepository _userRepository;
    private readonly IAuth0Service _auth0Service;

    public UsersController(IMessageBusClient messageBusClient, IUserRepository userRepository, IAuth0Service auth0Service)
    {
        _messageBusClient = messageBusClient;
        _userRepository = userRepository;
        _auth0Service = auth0Service;
    }

    [HttpPost("initialize")]
    public async Task<IActionResult> InitializeUser()
    {
        var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(jwtToken))
        {
            return BadRequest("JWT token is missing in the Authorization header.");
        }

        var userId = JwtHelper.GetUserId(jwtToken);
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            var userDetails = await _auth0Service.GetUserDetails(jwtToken);
            user = new User { Id = userId, Username = userDetails.Username };
            await _userRepository.AddUserAsync(user);
        }

        return Ok(user);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUsername(UpdateUserDto updateUserDto)
    {
        var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var userId = JwtHelper.GetUserId(jwtToken);
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        user.Username = updateUserDto.Username;
        await _userRepository.UpdateUserAsync(user);

        return Ok(user);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser()
    {
        var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var userId = JwtHelper.GetUserId(jwtToken);

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("JWT token is missing or invalid.");
        }

        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        await _userRepository.DeleteUserAsync(user);

        _messageBusClient.RemoveUser(user.Id);

        return Ok("User has been deleted.");
    }
}