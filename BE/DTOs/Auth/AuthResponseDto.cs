namespace BE.DTOs.Auth
{
    public class AuthResponseDto
    {
        public Guid Id { get; set; }

        public string AccessToken { get; set; }
            = string.Empty;

        public string RefreshToken { get; set; }
            = string.Empty;

        public string Username { get; set; }
            = string.Empty;

        public string Email { get; set; }
            = string.Empty;

        public string Role { get; set; }
            = string.Empty;

        public DateTime ExpiredAt { get; set; }
    }
}