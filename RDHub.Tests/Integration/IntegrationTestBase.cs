using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RDHub.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace RDHub.Tests.Integration;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("rdhub_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected WebApplicationFactory<Program> _factory = null!;
    protected HttpClient _client = null!;

    public virtual async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("ASPNETCORE_ENVIRONMENT", "Testing");

                builder.ConfigureServices(services =>
                {
                    // Remove o DbContext registrado com Npgsql de produção
                    services.Where(d =>
                        d.ServiceType == typeof(AppDbContext) ||
                        d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                        d.ServiceType == typeof(DbContextOptions))
                        .ToList()
                        .ForEach(d => services.Remove(d));

                    // Remove BackgroundServices
                    services.Where(d => d.ServiceType == typeof(IHostedService))
                        .ToList()
                        .ForEach(d => services.Remove(d));

                    // Reregistra apontando pro container
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(_postgres.GetConnectionString()));
                });
            });

        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public virtual async Task DisposeAsync()
    {
        _client?.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    protected async Task<HttpResponseMessage> PostAsync(string uri, object? content)
        => await _client.PostAsJsonAsync(uri, content);

    protected async Task<T?> PostAsync<T>(string uri, object? content)
    {
        var response = await _client.PostAsJsonAsync(uri, content);
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync());
    }

    protected async Task<HttpResponseMessage> GetAsync(string uri)
        => await _client.GetAsync(uri);

    protected async Task<T?> GetAsync<T>(string uri)
    {
        var response = await _client.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync());
    }

    protected async Task<HttpResponseMessage> PutAsync(string uri, object? content)
        => await _client.PutAsJsonAsync(uri, content);

    protected async Task<T?> PutAsync<T>(string uri, object? content)
    {
        var response = await _client.PutAsJsonAsync(uri, content);
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync());
    }

    protected async Task<HttpResponseMessage> DeleteAsync(string uri)
        => await _client.DeleteAsync(uri);

    protected AppDbContext GetDbContext()
    {
        var scope = _factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
}