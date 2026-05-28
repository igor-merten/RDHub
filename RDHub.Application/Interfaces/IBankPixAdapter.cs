using RDHub.Application.DTOs;
using RDHub.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.Interfaces;

// interface para comunicação com os bancos (MockServer)
public interface IBankPixAdapter
{
    string BankId { get; }
    Task<CobChargeResponseDto> CreateCob(CobChargeRequestDto request, Credential credential, CancellationToken ct = default);
    Task<CobvChargeResponseDto> CreateCobV(CobvChargeRequestDto request, Credential credential, CancellationToken ct = default);
    Task<BankChargeStatusResponseDto> GetChargeStatusAsync(string txId, Credential credential, CancellationToken ct = default);
}
