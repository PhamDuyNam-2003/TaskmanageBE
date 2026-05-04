using BE.Models;
using BE.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static BE.DTOs.AuthDto;

namespace BE.Services.Implements;

public class AuthService : IAuthService
{
    private readonly IMongoCollection<User> _users;
    private readonly IConfiguration _config;

    public AuthService(IMongoDatabase database, IConfiguration config)
    {
        _users = database.GetCollection<User>("Users");
        _config = config;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _users.Find(u => u.Username == dto.Username).FirstOrDefaultAsync();
        if (existingUser != null)
        {
            return new AuthResponseDto(false, "User already exists");
        }

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRole.Member
        };

        await _users.InsertOneAsync(user);
        return new AuthResponseDto(true, "Registration successful");
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _users.Find(u => u.Username == dto.Username).FirstOrDefaultAsync();

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return new AuthResponseDto(false, "Invalid username or password");
        }

        var accessToken = GenerateJwtToken(user, 60); 

        var refreshToken = Guid.NewGuid().ToString();

        var update = Builders<User>.Update
            .Set(u => u.RefreshToken, refreshToken)
            .Set(u => u.RefreshTokenExpiryTime, DateTime.UtcNow.AddDays(7));

        await _users.UpdateOneAsync(u => u.Id == user.Id, update);

        return new AuthResponseDto(true, "Login successful", accessToken, refreshToken);
    }

    private string GenerateJwtToken(User user, int minutes)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!));

        var claims = new[] {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}