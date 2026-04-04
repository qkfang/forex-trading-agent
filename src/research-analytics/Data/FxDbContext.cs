using FxWebPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace FxWebPortal.Data;

public class FxDbContext : DbContext
{
    public FxDbContext(DbContextOptions<FxDbContext> options) : base(options) { }

    public DbSet<ResearchArticle> ResearchArticles => Set<ResearchArticle>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerPortfolio> CustomerPortfolios => Set<CustomerPortfolio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResearchArticle>(entity =>
        {
            entity.ToTable("ResearchArticles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(500);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Author).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Tags).HasMaxLength(500);
            entity.Property(e => e.Sentiment).HasMaxLength(50);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(300);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Company).HasMaxLength(300);
            entity.HasMany(e => e.Portfolios)
                  .WithOne(p => p.Customer)
                  .HasForeignKey(p => p.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CustomerPortfolio>(entity =>
        {
            entity.ToTable("CustomerPortfolios");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CurrencyPair).HasMaxLength(20);
            entity.Property(e => e.Direction).HasMaxLength(10);
            entity.Property(e => e.Amount).HasPrecision(18, 4);
            entity.Property(e => e.EntryRate).HasPrecision(18, 6);
            entity.Property(e => e.Status).HasMaxLength(20);
        });
    }
}
