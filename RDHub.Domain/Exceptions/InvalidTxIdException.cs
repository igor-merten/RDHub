using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.Exceptions;

internal class InvalidTxIdException : DomainException
{
    public InvalidTxIdException(string message) : base(message)
    {
    }
}
