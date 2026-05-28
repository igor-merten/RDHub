using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.DTOs;

public sealed record CobvChargeRequestDto(
    string TxId,
    decimal Amount,
    string PixKey,
    DateOnly? DueDate = null,
    string? PayerMessage = null
    );