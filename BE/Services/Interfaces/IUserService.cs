using BE.Models;

namespace BE.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(string id);
        Task<bool> UpdateFcmTokenAsync(string userId, string token);
        Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
    }
}
