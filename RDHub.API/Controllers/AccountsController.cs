using MediatR;
using Microsoft.AspNetCore.Mvc;
using RDHub.Application.Commands.CreateAccount;
using RDHub.Application.Commands.UpdateAccount;
using RDHub.Application.Commands.DeleteAccount;
using RDHub.Application.Queries.GetAccountById;

namespace RDHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(
        [FromBody] CreateAccountCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetAccountById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAccountById(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAccountByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAccount(
        [FromRoute] Guid id,
        [FromBody] UpdateAccountRequest request,
        CancellationToken ct)
    {
        var command = new UpdateAccountCommand(id, request.CredentialId);
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAccount(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        await _mediator.Send(new DeleteAccountCommand(id), ct);
        return NoContent();
    }
}

// DTO local para evitar que o Id venha no body do PUT
public sealed record UpdateAccountRequest(Guid? CredentialId);