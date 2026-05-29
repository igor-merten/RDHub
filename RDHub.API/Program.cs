using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using RDHub.API.Middleware;
using RDHub.Application.Commands.CreateCob;
using RDHub.Infrastructure;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// ====== CONTROLLERS ======
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();

// ====== VERSIONING ======
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ====== SWAGGER ======
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

// ====== MEDIATR ======
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateCobCommand).Assembly));

// ====== INFRASTRUCTURE ======
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// ====== MIDDLEWARE ======
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"RDHub API {description.GroupName.ToUpperInvariant()}");
        }
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

// ====== SWAGGER CONFIG PER VERSION ======
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        => _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "RDHub API",
                Version = description.ApiVersion.ToString(),
                Description = description.IsDeprecated
                    ? "Esta versão está depreciada."
                    : string.Empty
            });
        }
    }
}
