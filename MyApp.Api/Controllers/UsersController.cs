using KapadaModel.DBOs;
using KapadaModel.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyApp.Api.DataContext;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration; 
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDBO _registerUser)
        {
            var existingUser = await _context.RegsiterUser.FirstOrDefaultAsync(i => i.Username == _registerUser.Username);

            if (existingUser != null)
            {
                return BadRequest("User Already Exists");
            }

            if (_registerUser.Password != _registerUser.ConfirmPassword)
            {
                return BadRequest("Password and Confirm Password do not match.");
            }

            var hasher = new PasswordHasher<RegisterUser>();
            var newUser = new RegisterUser
            {
                Username = _registerUser.Username,
                Password = hasher.HashPassword(null, _registerUser.Password)  
            };

            _context.RegsiterUser.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("User Registered Successfully");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDBO _login)
        {
            var user = await _context.RegsiterUser.FirstOrDefaultAsync(u => u.Username == _login.Username);
            if (user == null)
                return Unauthorized("User not found.");

            var hasher = new PasswordHasher<string>();
            var result = hasher.VerifyHashedPassword(null, user.Password, _login.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Invalid password.");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiresInMinutes"]!)),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new 
            {
                Message = "Login successful",
                Token = tokenString 
            });

        }


    }
}
