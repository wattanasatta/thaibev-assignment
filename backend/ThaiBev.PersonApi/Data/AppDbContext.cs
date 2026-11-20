using Microsoft.EntityFrameworkCore;
using ThaiBev.PersonApi.Entities;

namespace ThaiBev.PersonApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Person> Persons => Set<Person>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("Person", "dbo");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasColumnName("Id").UseIdentityColumn();
            entity.Property(p => p.FirstName).HasColumnName("FirstName").HasMaxLength(100).IsRequired();
            entity.Property(p => p.LastName).HasColumnName("LastName").HasMaxLength(100).IsRequired();
            entity.Property(p => p.BirthDate).HasColumnName("BirthDate").IsRequired();
            entity.Property(p => p.Address).HasColumnName("Address").HasMaxLength(500).IsRequired();
            entity.Property(p => p.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2(0)").HasDefaultValueSql("SYSUTCDATETIME()").IsRequired();
        });
    }
}
