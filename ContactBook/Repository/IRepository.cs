using ContactBook.DTOs;
using ContactBook.Models;
using ContactBook.Services;

namespace ContactBook.Repository
{
    public interface IRepository
    {
        IEnumerable<AppUserDTO> GetAll(PaginParameter usersParameter);
        Task<AppUser> GetByIdAsync(string Id);
        Task<AppUserDTO> GetByEmailAsync(string email);
        Task<bool> CreateUserAsync(AppUserDTO appUser);
        Task<bool> DeleteByIdAsync(string Id);
        Task<bool> AddUserPhoto(string UserId, PhotoToAddDTO model);
        Task<List<AppUserDTO>> SearchUsersAsync(string term);
    }
}
