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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Credential>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClientId).IsRequired();
            entity.Property(e => e.ClientSecret).IsRequired();
            // Certificado e senha podem ser opcionais dependendo do banco
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

            // Configuração da FK para Credential (1 Account -> 1 Credential)
            entity.HasOne(e => e.Credential)
                  .WithMany() // Ou .WithOne() se for uma relação 1:1 estrita
                  .HasForeignKey(e => e.CredentialId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PixKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);

            // Configuração da FK para Account (1 Account -> N PixKeys)
            entity.HasOne<Account>()
                  .WithMany(a => a.PixKeys)
                  .HasForeignKey(e => e.AccountId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Audit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AccountId).IsRequired();
            entity.Property(e => e.Payloads).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.TxId);
            entity.Property(e => e.Amount);
            entity.Property(e => e.Status);
            entity.Property(e => e.PaymentConfirmationTime);
        });
    }
}