using CloudinaryDotNet;
using ContactBook.Data;
using ContactBook.DTOs;
using ContactBook.Models;
using ContactBook.Repository;
using ContactBook.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public class AppUserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;

        private readonly IRepository _repository;
        public AppUserController(IRepository repository, IConfiguration config, UserManager<AppUser> userManager)
        {
            _config = config;
            _repository = repository;
            _userManager = userManager;

            Account account = new Account
            {
                Cloud = _config.GetSection("CloudinarySettings:CloudName").Value,
                ApiKey = _config.GetSection("CloudinarySettings:Apikey").Value,
                ApiSecret = _config.GetSection("CloudinarySettings:ApiSecret").Value
            };
        }

        //
        [HttpGet("all-users")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll([FromQuery] PaginParameter usersParameter)
        {
            var data = _repository.GetAll(usersParameter);
            return Ok(data);
        }

        //
        [HttpGet("get-user/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(string Id)
        {
            var data = await _repository.GetByIdAsync(Id);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        //
        [HttpGet]
        [Route("email")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetByEmail([FromQuery] string Email)
        {
            var data = await _repository.GetByEmailAsync(Email);
            if (data == null)
            {
                return BadRequest();
            }
            return Ok(data);
        }


        //
        [HttpPost]
        [Route("add-new")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] AppUserDTO appUser)
        {
            var created = await _repository.CreateUserAsync(appUser);
            if (created)
            {
                return Ok("User created successfully");
            }
            return BadRequest("User already exists");
        }

        //
        [HttpDelete("delete/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string Id)
        {
            var deleted = await _repository.DeleteByIdAsync(Id);
            if (deleted)
            {
                return Ok("User successfully removed");
            }
            return BadRequest("User not found");
        }

        //
        [HttpPatch("photo/{Id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> AddUserPhotoAsync(string userId, [FromForm] PhotoToAddDTO model)
        {
            var success = await _repository.AddUserPhoto(userId, model);
            if (success)
                return Ok(new { Message = "User photo updated successfully." });
            else
                return BadRequest("Failed to update user photo.");
        }

        //
        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            if (string.IsNullOrEmpty(term))
                return BadRequest("Search term cannot be Empty");

            var appUserDTOs = await _repository.SearchUsersAsync(term);

            if (appUserDTOs.Count == 0)
            {
                return NotFound("Search Result Empty");
            }
            return Ok(appUserDTOs);
        }
    }
}
