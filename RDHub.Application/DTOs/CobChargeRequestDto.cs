using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.DTOs;

public sealed record CobChargeRequestDto(
    string TxId,
    decimal Amount,
    string PixKey,
    int? ExpiresInSeconds = null,
    string? PayerMessage = null
    );
