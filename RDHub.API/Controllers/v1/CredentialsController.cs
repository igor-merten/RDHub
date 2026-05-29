using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RDHub.API.Contracts.Credentials;
using RDHub.Application.Commands.CreateCredential;
using RDHub.Application.Commands.DeleteCredential;
using RDHub.Application.Commands.UpdateCredential;
using RDHub.Application.Queries.GetCredentialById;

namespace RDHub.API.Controllers.v1;

/// <summary>
/// API para gerenciamento de credenciais.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/credentials")]
[Produces("application/json")]
public class CredentialsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CredentialsController> _logger;

    public CredentialsController(IMediator mediator, ILogger<CredentialsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Cria uma nova credencial.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateCredentialResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCredential(
        [FromBody] CreateCredentialRequest request,
        CancellationToken ct)
    {
        _logger.LogInformation(
            "Criando credencial: ClientId={ClientId}",
            request.ClientId);

        var command = new CreateCredentialCommand(
            ClientId: request.ClientId,
            ClientSecret: request.ClientSecret,
            Certificate: request.Certificate,
            CertificatePassword: request.CertificatePassword);

        var result = await _mediator.Send(command, ct);

        var response = new CreateCredentialResponse(
            Id: result.Id,
            ClientId: result.ClientId);

        return CreatedAtAction(nameof(GetCredentialById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Consulta uma credencial pelo Id.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetCredentialByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCredentialById(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        _logger.LogInformation("Consultando credencial: Id={Id}", id);

        var result = await _mediator.Send(new GetCredentialByIdQuery(id), ct);

        var response = new GetCredentialByIdResponse(
            Id: result.Id,
            ClientId: result.ClientId);

        return Ok(response);
    }

    /// <summary>
    /// Atualiza uma credencial existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UpdateCredentialResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCredential(
        [FromRoute] Guid id,
        [FromBody] UpdateCredentialRequest request,
        CancellationToken ct)
    {
        _logger.LogInformation("Atualizando credencial: Id={Id}", id);

        var command = new UpdateCredentialCommand(
            Id: id,
            ClientSecret: request.ClientSecret,
            Certificate: request.Certificate,
            CertificatePassword: request.CertificatePassword);

        var result = await _mediator.Send(command, ct);

        var response = new UpdateCredentialResponse(
            Id: result.Id,
            ClientId: result.ClientId);

        return Ok(response);
    }

    /// <summary>
    /// Remove uma credencial.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCredential(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        _logger.LogInformation("Removendo credencial: Id={Id}", id);

        await _mediator.Send(new DeleteCredentialCommand(id), ct);

        return NoContent();
    }
}