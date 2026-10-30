using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopApi.Data;
using ShopApi.Models;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// 1. БД
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Регистрируем ключ для JWT как Singleton, чтобы контроллеры могли его использовать
var key = new SymmetricSecurityKey(RandomNumberGenerator.GetBytes(32));
builder.Services.AddSingleton(key);

// 3. Настройка JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "ShopApi",
            ValidateAudience = true,
            ValidAudience = "ShopApiClient",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });
builder.Services.AddAuthorization();

// 4. Подключаем контроллеры (вместо MapGet/MapPost)
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // <-- вот это заменяет все MapGet/MapPost

// 5. Автосоздание менеджера
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
    if (!await db.Users.AnyAsync(u => u.Role == "Manager"))
    {
        var hasher = new PasswordHasher<User>();
        var manager = new User
        {
            Login = "manager",
            Phone = "88005553535",
            Address = "Офис",
            Role = "Manager"
        };
        manager.PasswordHash = hasher.HashPassword(manager, "admin123");
        db.Users.Add(manager);
        await db.SaveChangesAsync();
    }
}

app.Run();