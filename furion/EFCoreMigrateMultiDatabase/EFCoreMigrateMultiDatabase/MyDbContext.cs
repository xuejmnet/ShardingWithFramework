using Microsoft.EntityFrameworkCore;

namespace EFCoreMigrateMultiDatabase;

public class MyDbContext:DbContext
{
    public DbSet<User> Users { get; set; }
    public MyDbContext(DbContextOptions<MyDbContext> options):base(options)
    {
        
    }
}