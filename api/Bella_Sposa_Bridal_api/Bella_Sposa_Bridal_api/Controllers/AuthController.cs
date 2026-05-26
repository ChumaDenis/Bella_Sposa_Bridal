using BellaSposaBridal.Application.DTOs.Auth;
using BellaSposaBridal.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Bella_Sposa_Bridal_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _ctx;

    public AuthController(IConfiguration config, AppDbContext ctx)
    {
        _config = config;
        _ctx = ctx;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _ctx.AdminUsers.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null || user.PasswordHash != HashPassword(dto.Password))
            return Unauthorized(new { message = "Invalid credentials" });

        var token = _config["AdminToken"] ?? "bella-sposa-admin-2024";
        return Ok(new { token });
    }

    private static string HashPassword(string password)
    {
        var salt = Encoding.UTF8.GetBytes("BellaSposaBridal2024");
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
        return Convert.ToBase64String(pbkdf2.GetBytes(32));
    }
}
