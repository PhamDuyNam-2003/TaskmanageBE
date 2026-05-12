using BE.DTOs.Users;

namespace BE.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();

        Task<UserDto?> GetByIdAsync(Guid id);

        Task<UserDto?> UpdateAsync(
            Guid id,
            UpdateUserDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}