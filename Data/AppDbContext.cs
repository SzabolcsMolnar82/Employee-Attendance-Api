using Microsoft.EntityFrameworkCore;
using Employee_Attendance_Api.Models;

namespace Employee_Attendance_Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Dolgozo> Dolgozok { get; set; }
        public DbSet<Munkaora> Munkaorak { get; set; }
        public DbSet<HaviMunka> HaviMunka { get; set; }
    }

}