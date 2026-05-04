using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RDHub.Infrastructure.BackgroundServices;

// Consumer que lê mensagens da fila e avisa a Receba Digital
public class PaymentConsumerService : BackgroundService
{
    private readonly ILogger<PaymentConsumerService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public PaymentConsumerService(
        ILogger<PaymentConsumerService> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"]!,
            UserName = _configuration["RabbitMQ:UserName"]!,
            Password = _configuration["RabbitMQ:Password"]!
        };

        var connection = await factory.CreateConnectionAsync(ct);
        var channel = await connection.CreateChannelAsync(cancellationToken: ct);

        await channel.QueueDeclareAsync(
            queue: "payment-confirmed",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: ct);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            var body = Encoding.UTF8.GetString(ea.Body.ToArray());

            _logger.LogInformation("Mensagem lida da fila!");
            _logger.LogInformation("Conteúdo: {Body}", body);

            try
            {
                // TODO: Chamar a Receba Digital quando estiver disponível
                _logger.LogInformation("Mensagem processada com sucesso!");
                await channel.BasicAckAsync(ea.DeliveryTag, false, ct);
                _logger.LogInformation("Mensagem confirmada na fila!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem");
                await channel.BasicNackAsync(ea.DeliveryTag, false, true, ct);
            }
        };

        await channel.BasicConsumeAsync(
            queue: "payment-confirmed",
            autoAck: false,
            consumer: consumer,
            cancellationToken: ct);

        await Task.Delay(Timeout.Infinite, ct);
    }
}