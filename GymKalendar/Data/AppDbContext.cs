using GymKalendar.Controllers;
using GymKalendar.Models;
using Microsoft.EntityFrameworkCore;


namespace GymKalendar.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}

  
