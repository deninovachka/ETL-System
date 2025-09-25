using ETLSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ETLSystem.Data
{
    public class ApplicationDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=.;Database=ETLSystemDB;User Id=sa;Password=14082003;TrustServerCertificate=True;");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>(e =>
            {
                e.ToTable("Transactions");
                e.HasKey(x => x.TransactionId);
                e.Property(x => x.TransactionId).ValueGeneratedNever();
                e.Property(x => x.CustomerName).HasMaxLength(200).IsRequired();
                e.Property(x => x.Amount);
                e.Property(x => x.TransactionDate).HasColumnType("datetime2");
            });
        }

        public DbSet<Transaction> Transactions => Set<Transaction>();
    }
}
