using MediatR;

namespace RDHub.Application.Commands.CreateCob;

public sealed record CreateCobCommand(
    Guid AccountId,
    string ChargeType,
    decimal Amount,
    string PixKey,
    DateTime DueDate,
    int ExpiresInSeconds,
    string PayerMessage) : IRequest<CreateCobResult>;

public sealed record CreateCobResult(
    string TxId,
    string Status,
    string Emv,
    string PixLink);