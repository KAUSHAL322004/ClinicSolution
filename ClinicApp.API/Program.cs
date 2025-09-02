//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Add JWT Authentication
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//.AddJwtBearer(options =>
//{
//options.TokenValidationParameters = new TokenValidationParameters
// {
//ValidateIssuer = true,
//ValidateAudience = true,
//ValidateLifetime = true,
//ValidateIssuerSigningKey = true,

//ValidIssuer = builder.Configuration["Jwt:Issuer"],
//ValidAudience = builder.Configuration["Jwt:Audience"],
//IssuerSigningKey = new SymmetricSecurityKey(
// Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
// };
// });

//builder.Services.AddAuthorization();

//var app = builder.Build();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();
//app.Run();





using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClinicApp.API;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/api/auth/Login", (LoginDto dto, IConfiguration cfg) =>
{
    if (dto.Username == "admin" && dto.Password == "admin")
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, dto.Username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: cfg["Jwt:Issuer"],
            audience: cfg["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(2), // short expiry for demo
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = Guid.NewGuid().ToString();

        return Results.Ok(new { accessToken = tokenString, refreshToken });
    }

    return Results.Unauthorized();
});

app.Run();
