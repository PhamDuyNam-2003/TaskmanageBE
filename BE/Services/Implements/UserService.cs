using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using BE.Data;
using BE.DTOs.Users;
using BE.Models;
using BE.Services.Interfaces;

namespace BE.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        private readonly IMapper _mapper;


        public UserService(
            AppDbContext context,
            IMapper mapper)
        {
            _context = context;

            _mapper = mapper;
        }


        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return await _context.Users
                .ProjectTo<UserDto>(
                    _mapper.ConfigurationProvider)
                .ToListAsync();
        }


        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Where(x => x.Id == id)
                .ProjectTo<UserDto>(
                    _mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }


        public async Task<UserDto?> UpdateAsync(
            Guid id,
            UpdateUserDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return null;
            }


            user.Username = dto.Username;

            user.FcmToken = dto.FcmToken;


            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return false;
            }


            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}