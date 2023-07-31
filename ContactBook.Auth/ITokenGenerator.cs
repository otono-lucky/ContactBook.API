using ContactBook.Models;
using Microsoft.Extensions.Configuration;

namespace ContactBook.Auth
{
    public interface ITokenGenerator
    {
        Task<string> GenerateTokenAsync(AppUser user);
        object GenerateToken(string username, string id, string email, IConfiguration config, string[] roles);
    }
}
