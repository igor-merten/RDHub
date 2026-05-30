using MediatR;

namespace RDHub.Application.Commands.CreateCob;

public sealed record CreateCobCommand(
    Guid InvoiceId,
    decimal Amount,
    string PixKey,
    int ExpireInSeconds,
    string? PayerMessage) : IRequest<CreateCobResult>;

public sealed record CreateCobResult(
    string TxId,
    string Status,
    string Emv,
    string PixLink);