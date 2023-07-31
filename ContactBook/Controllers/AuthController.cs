using ContactBook.Auth;
using ContactBook.DTOs;
using ContactBook.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactBook.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        private readonly ITokenGenerator _tokenGenerator;
        public AuthController(UserManager<AppUser> userManager, IConfiguration config, ITokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _config = config;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var Password = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!Password)
            {
                return BadRequest("Invalid credentials");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var UserRoles = roles.ToArray();

            //var token = _tokenGenerator.GenerateTokenAsync(user);

            var token = _tokenGenerator.GenerateToken(model.Email, user.Id, model.Password, _config, UserRoles);
            return Ok(token);
        }
    }
}
