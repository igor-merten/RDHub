using MediatR;
using Microsoft.AspNetCore.Mvc;
using RDHub.Application.Commands.CreatePixKey;
using RDHub.Application.Commands.DeletePixKey;
using RDHub.Application.Commands.UpdatePixKey;
using RDHub.Application.Queries.GetPixKeyById;

namespace RDHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PixKeyController : ControllerBase
{
    private readonly IMediator _mediator;

    public PixKeyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePixKey(
        [FromBody] CreatePixKeyCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetPixKeyById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPixKeyById(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPixKeyByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> UpdatePixKey(
        [FromRoute] Guid id,
        [FromBody] UpdatePixKeyRequest request,
        CancellationToken ct)
    {
        var command = new UpdatePixKeyCommand(id, request.Key, request.AccountId);
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> DeletePixKey(
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        await _mediator.Send(new DeletePixKeyCommand(id), ct);
        return NoContent();
    }
}

public sealed record UpdatePixKeyRequest(string Key, Guid AccountId);