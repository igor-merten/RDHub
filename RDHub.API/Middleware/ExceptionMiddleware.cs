using System.Net;
using System.Text.Json;
using RDHub.Domain.Exceptions;

namespace RDHub.API.Middleware;

// captura exceções não tratadas e retorna respostas padronizadas
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning("Erro de domínio: {Message}", ex.Message);
            await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Recurso não encontrado: {Message}", ex.Message);
            await HandleExceptionAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado");
            await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "Erro interno do servidor");
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new { error = message, statusCode = (int)statusCode };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}