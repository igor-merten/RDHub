using Microsoft.EntityFrameworkCore;
using RDHub.Domain.Aggregates;

namespace RDHub.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Secret> Secrets => Set<Secret>();
    public DbSet<Audit> Audits => Set<Audit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClientId).IsRequired();
            entity.Property(e => e.BankId).IsRequired();
            entity.Property(e => e.Agency).IsRequired();
            entity.Property(e => e.AccountNumber).IsRequired();
            entity.Property(e => e.Cnpj).IsRequired();
            entity.Property(e => e.PixKey).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Secret>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClientId).IsRequired();
            entity.Property(e => e.ClientSecret).IsRequired();
            entity.Property(e => e.Certificate).IsRequired();
            entity.Property(e => e.CertificatePassword).IsRequired();
        });

        modelBuilder.Entity<Audit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AccountId).IsRequired();
            entity.Property(e => e.BankId).IsRequired();
            entity.Property(e => e.Action).IsRequired();
            entity.Property(e => e.Detail).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.TxId);
            entity.Property(e => e.Amount);
            entity.Property(e => e.Currency);
            entity.Property(e => e.Status);
            entity.Property(e => e.PaidAmount);
            entity.Property(e => e.PaidAt);
        });
    }
}