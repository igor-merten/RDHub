using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RDHub.API.Contracts.Payments;
using RDHub.Application.Commands.ConfirmPayment;
using RDHub.Application.Commands.CreateCob;
using RDHub.Application.Commands.CreateCobv;
using RDHub.Application.Queries.GetChargeStatus;

namespace RDHub.API.Controllers.v1;

/// <summary>
/// API para gerenciamento de cobranças Pix.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IMediator mediator, ILogger<PaymentController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Confirma manualmente o pagamento de uma cobrança.
    /// </summary>
    [HttpPost("validate/{txId}")]
    [ProducesResponseType(typeof(ConfirmPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmPayment(
        [FromRoute] string txId,
        CancellationToken ct)
    {
        _logger.LogInformation("Confirmando pagamento: TxId={TxId}", txId);

        var result = await _mediator.Send(new ConfirmPaymentCommand(txId), ct);

        var response = new ConfirmPaymentResponse(
            TxId: result.TxId,
            IsPaid: result.isPaid,
            Status: result.Status,
            PaymentConfirmationTime: result.PaymentConfirmationTime);

        return Ok(response);
    }

    /// <summary>
    /// Consulta o status de uma cobrança.
    /// </summary>
    [HttpGet("status/{txId}")]
    [ProducesResponseType(typeof(GetChargeStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChargeStatus(
        [FromRoute] string txId,
        CancellationToken ct)
    {
        _logger.LogInformation("Consultando status: TxId={TxId}", txId);

        var result = await _mediator.Send(new GetChargeStatusQuery(txId), ct);

        var response = new GetChargeStatusResponse(
            TxId: result.TxId,
            Status: result.Status,
            Amount: result.Amount,
            PaymentConfirmationTime: result.PaymentConfirmationTime,
            PaymentId: result.PaymentId);

        return Ok(response);
    }

    /// <summary>
    /// Cria uma cobrança Pix imediata (cob).
    /// </summary>
    [HttpPost("charge/cob")]
    [ProducesResponseType(typeof(CreateCobResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> CreateCob(
        [FromBody] CreateCobRequest request,
        CancellationToken ct)
    {
        _logger.LogInformation(
            "Criando cobrança Cob: InvoiceId={InvoiceId}, Valor={Amount}",
            request.InvoiceId,
            request.Amount);

        var command = new CreateCobCommand(
            InvoiceId: request.InvoiceId,
            Amount: request.Amount,
            PixKey: request.PixKey,
            ExpireInSeconds: request.ExpireInSeconds,
            PayerMessage: request.PayerMessage);

        var result = await _mediator.Send(command, ct);

        var response = new CreateCobResponse(
            TxId: result.TxId,
            Status: result.Status,
            Emv: result.Emv,
            PixLink: result.PixLink);

        return CreatedAtAction(nameof(GetChargeStatus), new { txId = response.TxId }, response);
    }

    /// <summary>
    /// Cria uma cobrança Pix com vencimento (cobv).
    /// </summary>
    [HttpPost("charge/cobv")]
    [ProducesResponseType(typeof(CreateCobvResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> CreateCobv(
        [FromBody] CreateCobvRequest request,
        CancellationToken ct)
    {
        _logger.LogInformation(
            "Criando cobrança Cobv: InvoiceId={InvoiceId}, Valor={Amount}, Vencimento={DueDate}",
            request.InvoiceId,
            request.Amount,
            request.DueDate);

        var command = new CreateCobvCommand(
            InvoiceId: request.InvoiceId,
            Amount: request.Amount,
            PixKey: request.PixKey,
            DueDate: request.DueDate,
            PayerMessage: request.PayerMessage);

        var result = await _mediator.Send(command, ct);

        var response = new CreateCobvResponse(
            TxId: result.TxId,
            Status: result.Status,
            Emv: result.Emv,
            PixLink: result.PixLink);

        return CreatedAtAction(nameof(GetChargeStatus), new { txId = response.TxId }, response);
    }
}