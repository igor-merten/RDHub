using MediatR;

namespace RDHub.Application.Commands.CreateCobv;

public sealed record CreateCobvCommand(
    Guid InvoiceId,
    decimal Amount,
    string PixKey,
    DateOnly DueDate,
    string? PayerMessage) : IRequest<CreateCobvResult>;

public sealed record CreateCobvResult(
    string TxId,
    string Status,
    string Emv,
    string PixLink);