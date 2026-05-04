using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace RDHub.Application.Commands.CreateInvoice;

// Representa a intenção de criar uma nova fatura
public sealed record CreateInvoiceCommand(
    Guid AccountId,
    decimal Amount) : IRequest<CreateInvoiceResult>;

// Resultado retornado após a fatura ser criada
public sealed record CreateInvoiceResult(
    Guid InvoiceId,
    string TxId,
    string Status,
    string Emv);