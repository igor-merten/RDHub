using RDHub.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.Interfaces;

// interface para comunicação com os bancos (MockServer)
public interface IBankPixAdapter
{
    string BankId { get; }
    Task<BankChargeResponse> CreateCob(BankChargeRequest request, CancellationToken ct = default);
    Task<BankChargeResponse> CreateCobV(BankChargeRequest request, CancellationToken ct = default);
    Task<BankChargeStatusResponse> GetChargeStatusAsync(string txId, CancellationToken ct = default);
}
