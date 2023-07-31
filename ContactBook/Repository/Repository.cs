using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using ContactBook.Data;
using ContactBook.DTOs;
using ContactBook.Models;
using ContactBook.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ContactBook.Repository
{
    public class Repository : IRepository
    {
        private readonly ContactBookContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly Cloudinary _cloudinary;

        public Repository(ContactBookContext context, UserManager<AppUser> userManager, Cloudinary cloudinary)
        {
            _context = context;
            _userManager = userManager;
            _cloudinary = cloudinary;
        }
        public IEnumerable<AppUserDTO> GetAll(PaginParameter usersParameter)
        {
            var utility = new Utilities(_context);
            var data = new List<AppUserDTO>();
            foreach (var userdata in utility.GetAllUsers(usersParameter))
            {
                data.Add(new AppUserDTO
                {
                    FirstName = userdata.FirstName,
                    LastName = userdata.LastName,
                    Email = userdata.Email,
                    PhoneNumber = userdata.PhoneNumber,
                    ImageUrl = userdata.ImageUrl,
                    FacebookUrl = userdata.FacebookUrl,
                    TwitterUrl = userdata.TwitterUrl,
                    City = userdata.City,
                    State = userdata.State,
                    Country = userdata.Country
                });
            }
            return data;
        }


        public async Task<AppUser> GetByIdAsync(string Id)
        {
            var result = await _context.appUsers.FindAsync(Id);
            if (result == null)
            {
                return null;
            }

            var data = new AppUser
            {
                FirstName = result.FirstName,
                LastName = result.LastName,
                Email = result.Email,
                PhoneNumber = result.PhoneNumber,
                ImageUrl = result.ImageUrl,
                FacebookUrl = result.FacebookUrl,
                TwitterUrl = result.TwitterUrl,
                City = result.City,
                State = result.State,
                Country = result.Country
            };
            return data;
        }

        public async Task<AppUserDTO> GetByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            var data = new AppUserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ImageUrl = user.ImageUrl,
                FacebookUrl = user.FacebookUrl,
                TwitterUrl = user.TwitterUrl,
                City = user.City,
                State = user.State,
                Country = user.Country
            };
            return data;
        }

        public async Task<bool> CreateUserAsync(AppUserDTO appUser)
        {
            var user = await _userManager.FindByEmailAsync(appUser.Email);
            if (user == null)
            {
                var data = new AppUser()
                {
                    FirstName = appUser.FirstName,
                    LastName = appUser.LastName,
                    UserName = appUser.Email,
                    Email = appUser.Email,
                    ImageUrl = appUser.ImageUrl,
                    PhoneNumber = appUser.PhoneNumber,
                    FacebookUrl = appUser.FacebookUrl,
                    TwitterUrl = appUser.TwitterUrl,
                    City = appUser.City,
                    State = appUser.State,
                    Country = appUser.Country
                };

                var res = await _userManager.CreateAsync(data, appUser.Password);
                if (res.Succeeded)
                {
                    await _userManager.AddToRoleAsync(data, "Regular");
                    return true;
                }
            }
            return false;
        }


        public async Task<bool> DeleteByIdAsync(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            { return false; }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                // Handle deletion error if needed
                return false;
            }

            _context.SaveChanges();
            return true;
        }

        //
        public async Task<bool> AddUserPhoto(string appUser, PhotoToAddDTO model)
        {
            var user = await _userManager.FindByIdAsync(appUser);
            if (user == null)
                return false;

            var file = model.PhotoFile;
            if (file.Length <= 0)
                return false;

            var imageUploadResult = new ImageUploadResult();
            using (var fs = file.OpenReadStream())
            {
                var imageUploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name, fs),
                    Transformation = new Transformation().Width(300).Height(300).Crop("fill").Gravity("face")
                };
                imageUploadResult = _cloudinary.Upload(imageUploadParams);
            }

            var publicId = imageUploadResult.PublicId;
            var Url = imageUploadResult.Url;
            user.ImageUrl = Url.AbsolutePath;

            await _userManager.UpdateAsync(user);
            return true;
        }

        //
        public async Task<List<AppUserDTO>> SearchUsersAsync(string term)
        {
            if (string.IsNullOrEmpty(term))
                return new List<AppUserDTO>();

            var users = await _userManager.Users
                .Where(u => u.Email.Contains(term)
                    || u.FirstName.Contains(term)
                    || u.LastName.Contains(term)
                    || u.City.Contains(term)
                    || u.State.Contains(term)
                    || u.Country.Contains(term))
                .ToListAsync();

            var appUserDTOs = users.Select(item => new AppUserDTO
            {
                FirstName = item.FirstName,
                LastName = item.LastName,
                Username = item.UserName,
                Email = item.Email,
                ImageUrl = item.ImageUrl,
                PhoneNumber = item.PhoneNumber,
                FacebookUrl = item.FacebookUrl,
                TwitterUrl = item.TwitterUrl,
                City = item.City,
                State = item.State,
                Country = item.Country
            }).ToList();

            return appUserDTOs;
        }
    }
}
