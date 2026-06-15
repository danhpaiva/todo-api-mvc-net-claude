using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services.Interfaces;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] Login login)
    {
        var token = _authService.GenerateToken(login);

        if (token is null)
            return Unauthorized("Credenciais inválidas.");

        return Ok(new { Token = token });
    }
}
