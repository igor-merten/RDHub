using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.Exceptions;

// exceção específica para TxId inválido
public class InvalidTxIdException : DomainException
{
    public InvalidTxIdException(string message) : base(message)
    {
    }
}
