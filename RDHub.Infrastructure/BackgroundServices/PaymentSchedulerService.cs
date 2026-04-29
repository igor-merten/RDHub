using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RDHub.Application.Commands.ConfirmPayment;
using MediatR;
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
        var pixChargeRepository = scope.ServiceProvider.GetRequiredService<IPixChargeRepository>();

        // Busca todas as cobranças ativas
        var activeCharges = await pixChargeRepository.GetAllActiveAsync(ct);

        foreach (var charge in activeCharges)
        {
            _logger.LogInformation("Verificando cobrança: TxId={TxId}", charge.TxId.Value);
            await mediator.Send(new ConfirmPaymentCommand(charge.TxId.Value), ct);
        }
    }
}