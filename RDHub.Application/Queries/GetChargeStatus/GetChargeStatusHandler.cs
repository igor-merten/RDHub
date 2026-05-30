using MediatR;
using RDHub.Domain.Repositories;
using RDHub.Domain.ValueObjects;
using System.Text.Json;

namespace RDHub.Application.Queries.GetChargeStatus;

// Consulta o status de uma cobrança buscando na auditoria
public sealed class GetChargeStatusHandler
    : IRequestHandler<GetChargeStatusQuery, GetChargeStatusResult>
{
    private readonly IAuditRepository _auditRepository;
    private readonly IMessageRepository _messageRepository;

    public GetChargeStatusHandler(IAuditRepository auditRepository, IMessageRepository messageRepository)
    {
        _auditRepository = auditRepository;
        _messageRepository = messageRepository;
    }

    public async Task<GetChargeStatusResult> Handle(
        GetChargeStatusQuery query,
        CancellationToken ct)
    {

        var txId = TxId.From(query.TxId);
        var audit = await _auditRepository.GetByTxIdAsync(txId, ct)
            ?? throw new Exception("Auditoria não encontrada");

        var messages = await _messageRepository.GetAllByAuditoryIdAsync(audit.Id, ct);

        var raw = messages.Select(m => new
        {
            Type = m.Type,
            Body = m.Body
        });

        return new GetChargeStatusResult(
            TxId: query.TxId,
            Status: audit.Status ?? string.Empty,
            Amount: audit.Amount,
            PaidAt: audit.PaymentConfirmationTime,
            PaymentId: audit.PaymentId,
            Raw: raw);
    }
}