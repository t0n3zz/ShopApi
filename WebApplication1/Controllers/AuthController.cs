using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopApi.Data;
using ShopApi.DTO;
using ShopApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ShopDbContext _db;
    private readonly SymmetricSecurityKey _key;

    public AuthController(ShopDbContext db, SymmetricSecurityKey key)
    {
        _db = db;
        _key = key;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Login == request.Login))
            return BadRequest(new { message = "Логин уже занят" });

        var hasher = new PasswordHasher<User>();
        var user = new User
        {
            Login = request.Login,
            Phone = request.Phone,
            Address = request.Address,
            Role = "User"
        };
        user.PasswordHash = hasher.HashPassword(user, request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Пользователь зарегистрирован" });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == request.Login);
        if (user == null) return Unauthorized();

        var hasher = new PasswordHasher<User>();
        if (hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password)
            == PasswordVerificationResult.Failed)
            return Unauthorized();

        return Ok(new
        {
            access_token = CreateToken(user),
            token_type = "Bearer"
        });
    }

    private string CreateToken(User user)
    {
        var claims = new[]
        {
            new Claim("sub", user.Id.ToString()),
            new Claim("name", user.Login),
            new Claim("role", user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: "ShopApi",
            audience: "ShopApiClient",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}