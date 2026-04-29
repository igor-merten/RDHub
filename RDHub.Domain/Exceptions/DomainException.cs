using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.Exceptions;

// exceção base para erros de regra de negócio
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
