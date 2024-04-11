using Microsoft.EntityFrameworkCore;
using CurdAssesment.Model;

namespace CurdAssesment.Db
{
    public class ContactDbContext : DbContext
    {
        public ContactDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
    }
}
