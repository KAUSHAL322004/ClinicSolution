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


using ClinicApp.API.Data;
using ClinicApp.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Swagger (optional but helpful)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Read JWT section from configuration
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = jwtSection["Key"];
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];

var keyBytes = Encoding.UTF8.GetBytes(key);



// ✅ Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ClockSkew = TimeSpan.Zero
        };
    });
var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
    });

var app = builder.Build();

// ✅ Enable middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Inside your /api/auth/login endpoint
if (dto.Username == "admin" && dto.Password == "admin")
{
    var claims = new[]
    {
        new Claim(ClaimTypes.Name, dto.Username),
        new Claim(ClaimTypes.Role, "Admin")   // ✅ role added here
    };

    // rest of token code...
}

internal class dto
{
    public static string Password { get; internal set; }
    public static string Username { get; internal set; }
}

app.MapPost("/api/auth/login", (LoginDto dto, IConfiguration cfg) =>
{
    if (dto.Username == "admin" && dto.Password == "admin")
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, dto.Username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: cfg["Jwt:Issuer"],
            audience: cfg["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(2),  // short expiry for demo
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Generate refresh token (random string)
        var refreshToken = Guid.NewGuid().ToString();

        return Results.Ok(new { accessToken = tokenString, refreshToken });
    }

    return Results.Unauthorized();
});
// Protect endpoints

app.MapPost("/api/auth/refresh", (RefreshRequest req, IConfiguration cfg) =>
{
    // Here you would normally validate refreshToken from DB
    if (string.IsNullOrEmpty(req.RefreshToken))
        return Results.Unauthorized();

    // Re-issue access token
    var claims = new[]
    {
        new Claim(ClaimTypes.Name, "admin"),
        new Claim(ClaimTypes.Role, "Admin")
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: cfg["Jwt:Issuer"],
        audience: cfg["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(2), // short expiry for demo
        signingCredentials: creds);

    var newAccessToken = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new { accessToken = newAccessToken });
});

record RefreshRequest(string RefreshToken);

app.MapPost("/api/auth/login", async (LoginDto dto, AppDbContext db, IConfiguration cfg) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username && u.Password == dto.Password);

    if (user == null)
        return Results.Unauthorized();

    var claims = new[]
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: cfg["Jwt:Issuer"],
        audience: cfg["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(2), // short expiry
        signingCredentials: creds);

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    // ✅ create refresh token and save
    var refreshToken = Guid.NewGuid().ToString();
    user.RefreshToken = refreshToken;
    user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

    await db.SaveChangesAsync();

    return Results.Ok(new { accessToken = tokenString, refreshToken });
});


app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/api/admin/secret", () => "Only Admins can see this")
   .RequireAuthorization(policy => policy.RequireRole("Admin"));

app.MapControllers();

app.Run();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Users.Any())
    {
        db.Users.Add(new User { Username = "admin", Password = "admin", Role = "Admin" });
        db.SaveChanges();
    }
}


record LoginDto(string Username, string Password);
