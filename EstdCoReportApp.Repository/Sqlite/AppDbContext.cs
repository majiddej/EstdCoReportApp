using EstdCoReportApp.Application.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstdCoReportApp.Repository.Sqlite
{
    public class AppDbContext : DbContext
    {
        public DbSet<CryptoRate> CryptoRates { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            //context.Database.EnsureCreated(); // Ensure that the database is created

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // You can remove this method if you are using the constructor with DbContextOptions
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=crypto.db");
            }
        }
    }
}
