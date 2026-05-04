using BE.Models;
using BE.Services.Interfaces;
using MongoDB.Driver;

namespace BE.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
        }

        public async Task<User?> GetByIdAsync(string id) =>
            await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

        public async Task<bool> UpdateFcmTokenAsync(string userId, string token)
        {
            var update = Builders<User>.Update.Set(u => u.FcmToken, token);
            var result = await _users.UpdateOneAsync(u => u.Id == userId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null || !BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                return false;

            var newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _users.UpdateOneAsync(u => u.Id == userId,
                Builders<User>.Update.Set(u => u.PasswordHash, newHash));
            return true;
        }
    }
}
