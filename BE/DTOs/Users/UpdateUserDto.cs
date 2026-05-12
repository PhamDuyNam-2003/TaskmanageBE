namespace BE.DTOs.Users
{
    public class UpdateUserDto
    {
        public string Username { get; set; } = string.Empty;

        public string? FcmToken { get; set; }
    }
}