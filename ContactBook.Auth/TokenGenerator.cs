using ContactBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ContactBook.Auth
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        public TokenGenerator(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> GenerateTokenAsync(AppUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };



            if (!string.IsNullOrWhiteSpace(user.FirstName))
                authClaims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));



            if (!string.IsNullOrWhiteSpace(user.LastName))
                authClaims.Add(new Claim(ClaimTypes.Surname, user.LastName));



            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
                authClaims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));



            //Gets the roles of the logged in user and adds it to Claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }



            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:JWTSigningkey"]));
            int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out var tokenValidityInMinutes);


            // Specifying JWTSecurityToken Parameters
            var token = new JwtSecurityToken
            (audience: _configuration["JWT:Audience"],
             issuer: _configuration["JWT:Issuer"],
             claims: authClaims,
             expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
             signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public object GenerateToken(string username, string id, string email, IConfiguration config, string[] roles)
        {
            //Create and Setup claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, id),
            };

            //create and setup roles
            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //create security token descriptor
            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JWT:JWTSigningkey").Value)),
                SecurityAlgorithms.HmacSha256Signature)
            };

            //create a token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenCreated = tokenHandler.CreateToken(securityTokenDescriptor);

            return new
            {
                token = tokenHandler.WriteToken(tokenCreated).ToString(),
                Id = id
            };
        }
    }


}
