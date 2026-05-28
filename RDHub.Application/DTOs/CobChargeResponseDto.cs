using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.DTOs;

public sealed record CobChargeResponseDto(
    string TxId,
    string Status,
    string Emv);
