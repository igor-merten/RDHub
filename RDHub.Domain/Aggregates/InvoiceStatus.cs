using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.Aggregates;

// Estados de uma fatura
public enum InvoiceStatus
{
    Open,
    Paid,
    Canceled
}
