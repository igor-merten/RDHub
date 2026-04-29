using MediatR;
using Microsoft.EntityFrameworkCore;
using RDHub.Application.Commands.CreateInvoice;
using RDHub.Application.Interfaces;
using RDHub.API.Middleware;
using RDHub.Domain.Repositories;
using RDHub.Infrastructure.BankAdapters;
using RDHub.Infrastructure.BankAdapters.Abstractions;
using RDHub.Infrastructure.BackgroundServices;
using RDHub.Infrastructure.Messaging;
using RDHub.Infrastructure.Persistence;
using RDHub.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ====== CONTROLLERS ======
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ====== MEDIATR ======
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateInvoiceCommand).Assembly));

// ====== DATABASE ======
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ====== REPOSITORIES ======
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPixChargeRepository, PixChargeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBankRepository, BankRepository>();
builder.Services.AddScoped<ISecretRepository, SecretRepository>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ====== BANK ADAPTERS ======
builder.Services.AddHttpClient<MockBankAdapter>(client =>
    client.BaseAddress = new Uri(builder.Configuration["MockServer:BaseUrl"]!));
builder.Services.AddSingleton<IBankPixAdapter>(sp =>
{
    var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(MockBankAdapter));
    return new MockBankAdapter(http, "MOCK1");
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