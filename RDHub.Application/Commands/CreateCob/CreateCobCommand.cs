using MediatR;

namespace RDHub.Application.Commands.CreateCob;

public sealed record CreateCobCommand(
    Guid InvoiceId,
    string ChargeType,
    decimal Amount,
    string PixKey,
    DateOnly DueDate,
    int ExpiresInSeconds,
    string PayerMessage) : IRequest<CreateCobResult>;

public sealed record CreateCobResult(
    string TxId,
    string Status,
    string Emv,
    string PixLink);