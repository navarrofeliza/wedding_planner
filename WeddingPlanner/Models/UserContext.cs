using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Models
{

    // the Context class is representing a session with MySql. it's allowing us to query for our save data.
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }

        // Make sure to add your models below here! Without it, you'll get errors
        public DbSet<User> Users { get; set; }
        public DbSet<Wedding> Weddings { get; set; }
        public DbSet<RSVP> RSVPs { get; set; }
    }
}