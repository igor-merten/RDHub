using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RDHub.Infrastructure.Persistence;

namespace RDHub.Tests.Integration;

/// <summary>
/// Classe base para testes de integração com WebApplicationFactory e SQLite em memória.
/// </summary>
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected WebApplicationFactory<Program> _factory = null!;
    protected HttpClient _client = null!;

    public virtual async Task InitializeAsync()
    {
        // Criar o factory com configuração customizada
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("ASPNETCORE_ENVIRONMENT", "Testing");
                
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] = "Data Source=:memory:"
                    });
                });

                builder.ConfigureServices((context, services) =>
                {
                    // Remover EF Core PostgreSQL se estiver registrado
                    var npgsqlDescriptor = services.FirstOrDefault(d =>
                        d.ServiceType.FullName?.Contains("NpgsqlDataSourceFactory") == true);
                    if (npgsqlDescriptor != null)
                        services.Remove(npgsqlDescriptor);

                    // Remover todos os DbContextOptions registrados
                    var dbContextOptions = services
                        .Where(d => d.ServiceType.IsGenericType &&
                                   d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>))
                        .ToList();
                    
                    foreach (var descriptor in dbContextOptions)
                    {
                        services.Remove(descriptor);
                    }

                    // Remover BackgroundService
                    var backgroundServices = services
                        .Where(d => d.ServiceType == typeof(IHostedService))
                        .ToList();
                    
                    foreach (var descriptor in backgroundServices)
                    {
                        services.Remove(descriptor);
                    }

                    // Reregistrar AppDbContext com SQLite
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite("Data Source=:memory:;Cache=Shared;"));
                });
            });

        // Criar o cliente HTTP
        _client = _factory.CreateClient();

        // Criar schema
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.EnsureCreatedAsync();
        }
    }

    public virtual async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Fazer uma requisição POST e retornar como resposta desserializada.
    /// </summary>
    protected async Task<T?> PostAsync<T>(string uri, object? content)
    {
        var response = await _client.PostAsJsonAsync(uri, content);
        response.EnsureSuccessStatusCode();
        var jsonString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(jsonString);
    }

    /// <summary>
    /// Fazer uma requisição POST e retornar o HttpResponseMessage.
    /// </summary>
    protected async Task<HttpResponseMessage> PostAsync(string uri, object? content)
    {
        return await _client.PostAsJsonAsync(uri, content);
    }

    /// <summary>
    /// Fazer uma requisição GET e retornar como resposta desserializada.
    /// </summary>
    protected async Task<T?> GetAsync<T>(string uri)
    {
        var response = await _client.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        var jsonString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(jsonString);
    }

    /// <summary>
    /// Fazer uma requisição GET e retornar o HttpResponseMessage.
    /// </summary>
    protected async Task<HttpResponseMessage> GetAsync(string uri)
    {
        return await _client.GetAsync(uri);
    }

    /// <summary>
    /// Fazer uma requisição PUT e retornar como resposta desserializada.
    /// </summary>
    protected async Task<T?> PutAsync<T>(string uri, object? content)
    {
        var response = await _client.PutAsJsonAsync(uri, content);
        response.EnsureSuccessStatusCode();
        var jsonString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(jsonString);
    }

    /// <summary>
    /// Fazer uma requisição PUT e retornar o HttpResponseMessage.
    /// </summary>
    protected async Task<HttpResponseMessage> PutAsync(string uri, object? content)
    {
        return await _client.PutAsJsonAsync(uri, content);
    }

    /// <summary>
    /// Fazer uma requisição DELETE e retornar o HttpResponseMessage.
    /// </summary>
    protected async Task<HttpResponseMessage> DeleteAsync(string uri)
    {
        return await _client.DeleteAsync(uri);
    }

    /// <summary>
    /// Obter o DbContext para asserções diretas no banco de dados.
    /// </summary>
    protected AppDbContext GetDbContext()
    {
        var scope = _factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
}
