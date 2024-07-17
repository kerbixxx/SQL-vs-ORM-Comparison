using Microsoft.EntityFrameworkCore;
using SQL.Entities;
using System.Collections.Generic;

namespace SQL.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> employees { get; set; }
        public DbSet<Position> positions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=5051;Database=sqltesting");
        }
    }
}
