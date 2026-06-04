using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDHub.Application.Commands.ConfirmPayment;

// representa a intenção de confirmar um pagamento
public sealed record ConfirmPaymentCommand(string TxId, string Status, decimal PaidAmount) : IRequest<ConfirmPaymentResult>;

// resultado retornado após confirmação
public sealed record ConfirmPaymentResult(string TxId, bool isPaid, string Status, DateTime? PaymentConfirmationTime);
