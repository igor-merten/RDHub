using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RDHub.Infrastructure.BackgroundServices;

// Consumer que lê mensagens da fila e avisa a Receba Digital
public class PaymentConsumerService : BackgroundService
{
    private readonly ILogger<PaymentConsumerService> _logger;
    private readonly HttpClient _http;
    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;
    private readonly string _recebDigitalUrl;

    public PaymentConsumerService(
        ILogger<PaymentConsumerService> logger,
        HttpClient http,
        string hostName,
        string userName,
        string password,
        string recebDigitalUrl)
    {
        _logger = logger;
        _http = http;
        _hostName = hostName;
        _userName = userName;
        _password = password;
        _recebDigitalUrl = recebDigitalUrl;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var factory = new ConnectionFactory
        {
            HostName = _hostName,
            UserName = _userName,
            Password = _password
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
            _logger.LogInformation("Mensagem recebida: {Body}", body);

            try
            {
                // Avisa a Receba Digital
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                await _http.PostAsync($"{_recebDigitalUrl}/payments/confirmed", content, ct);

                // Confirma que a mensagem foi processada
                await channel.BasicAckAsync(ea.DeliveryTag, false, ct);
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

        // Mantém o serviço rodando
        await Task.Delay(Timeout.Infinite, ct);
    }
}