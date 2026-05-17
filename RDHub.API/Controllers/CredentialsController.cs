using MediatR;
using Microsoft.AspNetCore.Mvc;
using RDHub.Application.Commands.CreateCredential;
using RDHub.Application.Commands.UpdateCredential;
using RDHub.Application.Commands.DeleteCredential;
using RDHub.Application.Queries.GetCredentialById;

namespace RDHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CredentialsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CredentialsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCredential(
        [FromBody] CreateCredentialCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetCredentialById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCredentialById(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCredentialByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCredential(
        [FromRoute] Guid id,
        [FromBody] UpdateCredentialRequest request,
        CancellationToken ct)
    {
        var command = new UpdateCredentialCommand(id, request.ClientSecret, request.Certificate, request.CertificatePassword);
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCredential(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        await _mediator.Send(new DeleteCredentialCommand(id), ct);
        return NoContent();
    }
}

public sealed record UpdateCredentialRequest(string ClientSecret, string Certificate, string CertificatePassword);