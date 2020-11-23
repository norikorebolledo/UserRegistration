using Microsoft.EntityFrameworkCore;
using UserRegistration.Entities;

namespace UserRegistration.Data.Sql
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
           : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<EmailVerification> EmailVerifications { get; set; }

    }
}
