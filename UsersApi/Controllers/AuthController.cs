using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UsersApi.Data;
using UsersApi.DTOs;
using UsersApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using UsersApi.Validators;
using System.Security.Cryptography;
using Microsoft.AspNetCore.RateLimiting;

namespace UsersApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("AuthPolicy")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    public AuthController(AppDbContext context,IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDTO request)
    {
        var validators = new RegisterValidator();
        var result = validators.Validate(request);
        if(!result.IsValid)
        {
            var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(errors);
        }
        if (_context.Users.Any(u => u.Email == request.Email))
        {
            return BadRequest("Email Already Exist");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash,
            Age = request.Age,
            Role = request.Role,
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok("User Already registered");
    }

    //Post api/auth/Login
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO request)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
        if (user == null)
        {
            return Unauthorized("Invalid Email or Password");
        }
        // Verify password
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid) return Unauthorized("Invalid email or password");
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        //Save refresh token to databse 
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        _context.SaveChanges();
        return Ok(new { accessToken,refreshToken });
    }

    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshTokenDTO request)
    {
        // Find user with this refresh token
        var user = _context.Users
            .FirstOrDefault(u => u.RefreshToken == request.RefreshToken);

        if (user == null)
            return Unauthorized("Invalid refresh token");

        // Check if refresh token is expired
        if (user.RefreshTokenExpiry < DateTime.UtcNow)
            return Unauthorized("Refresh token expired");

        // Generate new access token
        var newAccessToken = GenerateAccessToken(user);

        // Generate new refresh token
        var newRefreshToken = GenerateRefreshToken();

        // Update refresh token in database
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        _context.SaveChanges();

        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = newRefreshToken
        });
    }
    
    [HttpPost("Revoke")]
    public IActionResult Revoke([FromBody] RefreshTokenDTO request)
    {
        var user = _context.Users.FirstOrDefault(u=>u.RefreshToken == request.RefreshToken);

        if(user==null)
        {
            return BadRequest("Invalid refresh token");
        }
        //remove refresh token -> logout
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        _context.SaveChanges();
        return Ok("Logged out successfully");
    }
    private string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));
        var credentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Email , user.Email),
            new Claim(ClaimTypes.Name , user.Name),
            new Claim(ClaimTypes.Role,user.Role),
        };
        var token = new JwtSecurityToken(
            issuer : _configuration["Jwt:Issuer"],
            audience : _configuration["Jwt:Audience"],
            claims : claims,
            expires : DateTime.Now.AddMinutes(15),
            signingCredentials : credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Email,user.Email ),
            new Claim(ClaimTypes.Name,user.Name),
            new Claim(ClaimTypes.Role,user.Role),
        };

        var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddHours(24),
        signingCredentials: credentials
    );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}