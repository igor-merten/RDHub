using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using RDHub.Application.Interfaces;
using RDHub.Domain.Repositories;
using RDHub.Infrastructure.BackgroundServices;
using RDHub.Infrastructure.BankAdapters;
using RDHub.Infrastructure.BankAdapters.Abstractions;
using RDHub.Infrastructure.BankAdapters.MockBank;
using RDHub.Infrastructure.Messaging;
using RDHub.Infrastructure.Persistence;
using RDHub.Infrastructure.Persistence.Repositories;

namespace RDHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ====== DATABASE ======
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // ====== REPOSITORIES ======
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ICredentialRepository, CredentialRepository>();
        services.AddScoped<IPixKeyRepository, PixKeyRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ====== BANK ADAPTERS ======
        services.AddSingleton<IMockBankTokenProvider, MockTokenProvider>();
        services.AddHttpClient(nameof(MockBankAdapter), client =>
        {
            client.BaseAddress = new Uri(configuration["BanksBaseUrl:MockServer"]!);
        });
        services.AddSingleton<IBankPixAdapter>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var http = factory.CreateClient(nameof(MockBankAdapter));
            var tokenProvider = sp.GetRequiredService<IMockBankTokenProvider>();
            var bankId = configuration["BankAdapters:MockBankId"]!;
            return new MockBankAdapter(http, tokenProvider, bankId);
        });
        services.AddSingleton<IBankAdapterFactory, BankAdapterFactory>();

        // ====== MESSAGING ======
        services.AddSingleton<IMessageQueue>(sp =>
            new RabbitMqPublisher(
                configuration["RabbitMQ:HostName"]!,
                configuration["RabbitMQ:UserName"]!,
                configuration["RabbitMQ:Password"]!));

        // ====== BACKGROUND SERVICES ======
        services.AddHostedService<PaymentConsumerService>();
        services.AddHostedService<PaymentSchedulerService>();

        return services;
    }
}