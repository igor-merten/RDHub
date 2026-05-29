using MediatR;
using Microsoft.AspNetCore.Builder;
using RDHub.API.Middleware;
using RDHub.Application.Commands.CreateCob;
using RDHub.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ====== CONTROLLERS ======
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddSwaggerGen();

// ====== MEDIATR ======
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateCobCommand).Assembly));

// ====== DATABASE ======
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateCobCommand).Assembly));

// ====== INFRASTRUCTURE ======
builder.Services.AddInfrastructure(builder.Configuration);

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