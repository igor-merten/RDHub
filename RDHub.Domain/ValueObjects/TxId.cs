using RDHub.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Domain.ValueObjects;

public sealed record TxId
{
    public string Value { get; }
    private TxId(string value)
    {
        Value = value;
    }

    public static TxId From(string value)
    {
        if (String.IsNullOrEmpty(value))
            throw new InvalidTxIdException("TxId não pode ser vazio");

        if (value.Length > 35)
            throw new InvalidTxIdException("TxId não pode exceder 35 caracteres");

        if (!System.Text.RegularExpressions.Regex.IsMatch(value, "^[a-zA-Z0-9]+$"))
            throw new InvalidTxIdException("TxId deve conter apenas caracteres alfanuméricos");

        return new TxId(value);
    }

    public static TxId Generate()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = Guid.NewGuid().ToString("N")[..10].ToUpper();
        var value = $"RDH{timestamp}{random}"[..35];
        return new TxId(value);
    }

    public override string ToString() => Value;
    public static implicit operator string(TxId txId) => txId.Value;
}
