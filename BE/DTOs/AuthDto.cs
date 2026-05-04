namespace BE.DTOs
{
    public class AuthDto
    {
        public record RegisterDto(string Username, string Email, string Password);
        public record LoginDto(string Username, string Password);
        public record AuthResponseDto(bool Success, string Message, string? AccessToken = null, string? RefreshToken = null);
    }
}
