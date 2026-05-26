using BellaSposaBridal.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Bella_Sposa_Bridal_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        if (dto.Username == "admin" && dto.Password == "adminBella")
        {
            var token = _config["AdminToken"] ?? "bella-sposa-admin-2024";
            return Ok(new { token });
        }

        return Unauthorized(new { message = "Invalid credentials" });
    }
}
