using MediatR;
using Microsoft.AspNetCore.Mvc;
using RDHub.Application.Commands.ConfirmPayment;
using RDHub.Application.Commands.CreateInvoice;
using RDHub.Application.Queries.GetChargeStatus;
using RDHub.Application.Commands.CreateCob;
using RDHub.Application.Commands.CreateCobv;

namespace RDHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // cria uma nova cobrança pix para um usuário
    [HttpPost("charge")]
    public async Task<IActionResult> CreateCharge(
        [FromBody] CreateInvoiceCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetChargeStatus), new { txId = result.TxId }, result);
    }

    // confirma manualmente o pagamento de uma cobrança
    [HttpPost("{txId}/confirm")]
    public async Task<IActionResult> ConfirmPayment(
        [FromRoute] string txId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ConfirmPaymentCommand(txId), ct);
        return Ok(result);
    }

    // consulta o status de uma cobrança
    [HttpGet("{txId}/status")]
    public async Task<IActionResult> GetChargeStatus(
        [FromRoute] string txId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetChargeStatusQuery(txId), ct);
        return Ok(result);
    }

    // cria cobrança pix imediata (cob) 
    [HttpPost("charge/v1/cob")]
    public async Task<IActionResult> CreateCob(
        [FromBody] CreateCobCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetChargeStatus), new { txId = result.TxId }, result);
    }

    // cria cobrança pix com vencimento (cobv)
    [HttpPost("charge/v1/cobv")]
    public async Task<IActionResult> CreateCobv(
        [FromBody] CreateCobvCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetChargeStatus), new { txId = result.TxId }, result);
    }
}