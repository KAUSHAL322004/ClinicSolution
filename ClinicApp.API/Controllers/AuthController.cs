using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClinicApp.API.Models;

namespace ClinicApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new AppUser { UserName = username };
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded) return Unauthorized();

            // create JWT
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey12345"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

       
            private readonly JwtService _jwt;

            public AuthController(JwtService jwt)
            {
                _jwt = jwt;
            }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // ✅ Validate user (check username + password)
            if (model.Username == "admin" && model.Password == "123")
            {
                var accessToken = GenerateJwtToken("admin", "Admin"); // expires in ~15 min
                var refreshToken = Guid.NewGuid().ToString(); // generate random refresh token

                // Save refresh token in DB or in-memory (for demo, just return it)
                return Ok(new
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }

            return Unauthorized();
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            // Normally: validate refresh token from DB
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Unauthorized();

            // Issue new access token
            var newAccessToken = GenerateJwtToken("admin", "Admin");

            return Ok(new { AccessToken = newAccessToken });
        }

        public class RefreshRequest
        {
            public string RefreshToken { get; set; }
        }


        private object GenerateJwtToken(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        public record LoginRequest(string Username, string Password);

    }

    public class LoginModel
    {
        internal string Password;
        internal string Username;
    }
}
