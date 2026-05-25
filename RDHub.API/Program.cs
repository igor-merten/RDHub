using MediatR;
using Microsoft.EntityFrameworkCore;
using RDHub.Application.Interfaces;
using RDHub.API.Middleware;
using RDHub.Domain.Repositories;
using RDHub.Infrastructure.BankAdapters;
using RDHub.Infrastructure.BankAdapters.Abstractions;
using RDHub.Infrastructure.BackgroundServices;
using RDHub.Infrastructure.Messaging;
using RDHub.Infrastructure.Persistence;
using RDHub.Infrastructure.Persistence.Repositories;
using RDHub.Application.Commands.CreateCob;

var builder = WebApplication.CreateBuilder(args);

// ====== CONTROLLERS ======
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("RecebDigital", client =>
    client.BaseAddress = new Uri(builder.Configuration["RecebaDigital:BaseUrl"]!));

// ====== MEDIATR ======
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateCobCommand).Assembly));

// ====== DATABASE ======
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ====== REPOSITORIES ======
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICredentialRepository, CredentialRepository>();
builder.Services.AddScoped<IPixKeyRepository, PixKeyRepository>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ====== BANK ADAPTERS ======
//adapter1
builder.Services.AddHttpClient<MockBankAdapter>(client =>
    client.BaseAddress = new Uri(builder.Configuration["BanksBaseUrl:MockServer"]!));
builder.Services.AddSingleton<IBankPixAdapter>(sp =>
{
    var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(MockBankAdapter));
    var bankId = builder.Configuration["BankAdapters:MockBankId"]!;
    return new MockBankAdapter(http, bankId);
});

//adapter2
builder.Services.AddHttpClient<MockBank2Adapter>(client =>
    client.BaseAddress = new Uri(builder.Configuration["BanksBaseUrl:MockServer"]!));
builder.Services.AddSingleton<IBankPixAdapter>(sp =>
{
    var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(MockBank2Adapter));
    var bankId = builder.Configuration["BankAdapters:MockBank2Id"]!;
    return new MockBank2Adapter(http, bankId);
});

builder.Services.AddSingleton<IBankAdapterFactory, BankAdapterFactory>();

// ====== MESSAGING ======
builder.Services.AddSingleton<IMessageQueue>(sp =>
    new RabbitMqPublisher(
        builder.Configuration["RabbitMQ:HostName"]!,
        builder.Configuration["RabbitMQ:UserName"]!,
        builder.Configuration["RabbitMQ:Password"]!));

// ====== BACKGROUND SERVICES ======
builder.Services.AddHostedService<PaymentConsumerService>();
builder.Services.AddHostedService<PaymentSchedulerService>();

var app = builder.Build();

// ====== MIDDLEWARE ======
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();