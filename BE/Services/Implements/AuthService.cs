using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using BCrypt.Net;

using BE.Data;
using BE.DTOs.Auth;
using BE.Models;
using BE.Services.Interfaces;

namespace BE.Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        private readonly IConfiguration _config;

        private readonly IMapper _mapper;


        public AuthService(
            AppDbContext context,
            IConfiguration config,
            IMapper mapper)
        {
            _context = context;

            _config = config;

            _mapper = mapper;
        }


        public async Task<AuthResponseDto> RegisterAsync(
            RegisterRequestDto dto)
        {
            dto.Email =
                dto.Email.Trim().ToLower();

            var exists =
                await _context.Users.AnyAsync(x =>
                    x.Email == dto.Email ||
                    x.Username == dto.Username);

            if (exists)
            {
                throw new Exception(
                    "Username hoặc Email đã tồn tại");
            }


            var user = new User
            {
                Username = dto.Username,

                Email = dto.Email,

                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        dto.Password)
            };


            _context.Users.Add(user);

            await _context.SaveChangesAsync();


            var accessToken =
                GenerateAccessToken(user);

            var refreshToken =
                GenerateRefreshToken();


            user.RefreshToken =
                refreshToken;

            user.RefreshTokenExpiryTime =
                DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();


            return new AuthResponseDto
            {
                AccessToken = accessToken,

                RefreshToken = refreshToken,

                ExpiredAt =
                    DateTime.UtcNow.AddHours(1)
            };
        }


        public async Task<AuthResponseDto> LoginAsync(
            LoginRequestDto dto)
        {
            dto.Login =
                dto.Login.Trim().ToLower();

            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Email.ToLower() == dto.Login ||
                    x.Username.ToLower() == dto.Login);

            if (user == null)
            {
                throw new Exception(
                    "Tài khoản không tồn tại");
            }


            var validPassword =
                BCrypt.Net.BCrypt.Verify(
                    dto.Password,
                    user.PasswordHash);

            if (!validPassword)
            {
                throw new Exception(
                    "Sai mật khẩu");
            }


            var accessToken =
                GenerateAccessToken(user);

            var refreshToken =
                GenerateRefreshToken();


            user.RefreshToken =
                refreshToken;

            user.RefreshTokenExpiryTime =
                DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,

                RefreshToken = refreshToken,

                Id = user.Id,

                Username = user.Username,

                Email = user.Email,

                Role = user.Role.ToString(),

                ExpiredAt = DateTime.UtcNow.AddHours(1)
            };
        }


        public async Task<AuthResponseDto>
            RefreshTokenAsync(
                string refreshToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.RefreshToken == refreshToken);

            if (user == null)
            {
                throw new Exception(
                    "Refresh token không hợp lệ");
            }

            if (user.RefreshTokenExpiryTime
                < DateTime.UtcNow)
            {
                throw new Exception(
                    "Refresh token hết hạn");
            }


            var accessToken =
                GenerateAccessToken(user);

            var newRefreshToken =
                GenerateRefreshToken();


            user.RefreshToken =
                newRefreshToken;

            user.RefreshTokenExpiryTime =
                DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();


            return new AuthResponseDto
            {
                AccessToken = accessToken,

                RefreshToken = newRefreshToken,

                ExpiredAt =
                    DateTime.UtcNow.AddHours(1)
            };
        }


        public string GenerateAccessToken(
            User user)
        {
            var jwtSettings =
                _config.GetSection("JwtSettings");

            var secret =
                jwtSettings["Secret"]
                ?? throw new Exception(
                    "JWT Secret missing");


            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secret));


            var creds =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);


            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.Username),

                new Claim(
                    ClaimTypes.Email,
                    user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.Role.ToString())
            };


            var token =
                new JwtSecurityToken(
                    issuer:
                        jwtSettings["Issuer"],

                    audience:
                        jwtSettings["Audience"],

                    claims: claims,

                    expires:
                        DateTime.UtcNow.AddHours(1),

                    signingCredentials: creds
                );


            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }


        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];

            using var rng =
                RandomNumberGenerator.Create();

            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(
                randomBytes);
        }


        public async Task LogoutAsync(
            Guid userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == userId);

            if (user == null)
            {
                throw new Exception(
                    "User không tồn tại");
            }


            user.RefreshToken = null;

            user.RefreshTokenExpiryTime = null;

            await _context.SaveChangesAsync();
        }
    }
}