using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ContactBook.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactBook.Data
{
    public class ContactBookContext : IdentityDbContext
    {

        public DbSet<AppUser> appUsers { get; set; }
        public ContactBookContext(DbContextOptions<ContactBookContext> options):
            base(options){ }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
