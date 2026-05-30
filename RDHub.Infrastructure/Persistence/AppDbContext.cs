using Microsoft.EntityFrameworkCore;
using RDHub.Domain.Aggregates;

namespace RDHub.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Credential> Credentials => Set<Credential>();
    public DbSet<PixKey> PixKeys => Set<PixKey>(); 
    public DbSet<Audit> Audits => Set<Audit>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Credential>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClientId).IsRequired();
            entity.Property(e => e.ClientSecret).IsRequired();
            entity.Property(e => e.Certificate);
            entity.Property(e => e.CertificatePassword);
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Document).IsRequired().HasMaxLength(20);
            entity.Property(e => e.BankId).IsRequired();
            entity.Property(e => e.Agency).IsRequired().HasMaxLength(10);
            entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.Active).IsRequired();

            entity.HasOne(e => e.Credential)
                  .WithMany()
                  .HasForeignKey(e => e.CredentialId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PixKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);

            entity.HasOne<Account>()
                  .WithMany(a => a.PixKeys)
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Audit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AccountId).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.TxId);
            entity.Property(e => e.Amount);
            entity.Property(e => e.Status);
            entity.Property(e => e.PaymentConfirmationTime);
            entity.Property(e => e.PaymentId);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AuditoryId).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Body).IsRequired();
        });
    }
}