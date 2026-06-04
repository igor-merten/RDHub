using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RDHub.Application.Commands.ConfirmPayment;
using RDHub.Application.Queries.GetChargeStatus;
using RDHub.Domain.Repositories;

namespace RDHub.Infrastructure.BackgroundServices;

// Serviço que verifica pagamentos pendentes automaticamente de tempos em tempos
public class PaymentSchedulerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PaymentSchedulerService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

    public PaymentSchedulerService(
        IServiceProvider serviceProvider,
        ILogger<PaymentSchedulerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            _logger.LogInformation("Verificando pagamentos pendentes...");

            try
            {
                await CheckPendingPaymentsAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar pagamentos pendentes");
            }

            await Task.Delay(_interval, ct);
        }
    }

    private async Task CheckPendingPaymentsAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var auditRepository = scope.ServiceProvider.GetRequiredService<IAuditRepository>();

        // Busca todas as auditorias com status Open
        var openCharges = await auditRepository.GetAllOpenAsync(ct);

        foreach (var audit in openCharges)
        {
            _logger.LogInformation("Verificando cobrança: TxId={TxId}", audit.TxId);
            var result = await mediator.Send(new GetChargeStatusQuery(audit.TxId!), ct);
            
            _logger.LogInformation("Status da cobrança {TxId}: {Status}", audit.TxId, result.Status);

            // Se o branco responder que foi pago, dispara o command para dar baixa no sistema
            if (result.Status == "Paid")
            {
                await mediator.Send(new ConfirmPaymentCommand(audit.TxId!, audit.Status, audit.Amount), ct);
                _logger.LogInformation("Cobrança {TxId} confirmada.", audit.TxId);
            }
        }
    }
}
