using MediatR;

namespace RDHub.Application.Commands.CreateCobv;

public sealed record CreateCobvCommand(
    Guid InvoiceId,
    string ChargeType,
    decimal Amount,
    string PixKey,
    DateTime DueDate,
    int ExpiresInSeconds,
    string PayerMessage) : IRequest<CreateCobvResult>;

public sealed record CreateCobvResult(
    string TxId,
    string Status,
    string Emv,
    string PixLink);
