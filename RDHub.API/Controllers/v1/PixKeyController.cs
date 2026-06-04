using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RDHub.API.Contracts.PixKeys;
using RDHub.Application.Commands.CreatePixKey;
using RDHub.Application.Commands.DeletePixKey;
using RDHub.Application.Queries.GetPixKeyById;

namespace RDHub.API.Controllers.v1;

/// <summary>
/// API para gerenciamento de chaves Pix.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/pix-keys")]
[Produces("application/json")]
public class PixKeyController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PixKeyController> _logger;

    public PixKeyController(IMediator mediator, ILogger<PixKeyController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Cria uma nova chave Pix.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreatePixKeyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePixKey(
        [FromBody] CreatePixKeyRequest request,
        CancellationToken ct)
    {
        _logger.LogInformation(
            "Criando chave Pix: Key={Key}, AccountId={AccountId}",
            request.Key,
            request.AccountId);

        var command = new CreatePixKeyCommand(
            Key: request.Key,
            AccountId: request.AccountId);

        var result = await _mediator.Send(command, ct);

        var response = new CreatePixKeyResponse(
            Id: result.Id,
            Key: result.Key,
            AccountId: result.AccountId);

        return CreatedAtAction(nameof(GetPixKeyById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Consulta uma chave Pix pelo Id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetPixKeyByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPixKeyById(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        _logger.LogInformation("Consultando chave Pix: Id={Id}", id);

        var result = await _mediator.Send(new GetPixKeyByIdQuery(id), ct);

        var response = new GetPixKeyByIdResponse(
            Id: result.Id,
            Key: result.Key,
            AccountId: result.AccountId);

        return Ok(response);
    }

    /// <summary>
    /// Remove uma chave Pix.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePixKey(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        _logger.LogInformation("Removendo chave Pix: Id={Id}", id);

        await _mediator.Send(new DeletePixKeyCommand(id), ct);

        return NoContent();
    }
}