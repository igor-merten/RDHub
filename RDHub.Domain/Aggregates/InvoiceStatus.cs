using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.Aggregates;

public enum InvoiceStatus
{
    Open,
    Paid,
    Canceled
}
