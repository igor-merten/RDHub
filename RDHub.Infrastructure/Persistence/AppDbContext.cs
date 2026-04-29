using Microsoft.EntityFrameworkCore;
using RDHub.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Infrastructure.Persistence;

// contexto principal do banco de dados (mapeia entidades para tabelas)
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<PixCharge> PixCharges => Set<PixCharge>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Bank> Banks => Set<Bank>();
    public DbSet<Secret> Secrets => Set<Secret>();
    public DbSet<Audit> Audits => Set<Audit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BankId).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>();
            entity.OwnsOne(e => e.Amount, money =>
            {
                money.Property(m => m.Value).HasColumnName("Amount");
                money.Property(m => m.Currency).HasColumnName("Currency");
            });
            entity.OwnsOne(e => e.TxId, txId =>
            {
                txId.Property(t => t.Value).HasColumnName("TxId");
            });
        });

        modelBuilder.Entity<PixCharge>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BankId).IsRequired();
            entity.Property(e => e.Emv).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>();
            entity.OwnsOne(e => e.TxId, txId =>
            {
                txId.Property(t => t.Value).HasColumnName("TxId");
            });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Cnpj).IsRequired();
            entity.Property(e => e.PixKey).IsRequired();
            entity.Property(e => e.BankId).IsRequired();
        });

        modelBuilder.Entity<Bank>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });

        modelBuilder.Entity<Secret>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClientId).IsRequired();
            entity.Property(e => e.ClientSecret).IsRequired();
        });

        modelBuilder.Entity<Audit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired();
            entity.Property(e => e.Details).IsRequired();
        });
    }
}
