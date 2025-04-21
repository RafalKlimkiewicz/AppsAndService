using System.Text.Json; // To use JsonSerializer.

using Northwind.Queue.Models; // To use ProductQueueMessage.

using RabbitMQ.Client; // To use ConnectionFactory.
using RabbitMQ.Client.Events; // To use EventingBasicConsumer.

namespace Northwind.Background.Workers;

public class QueueWorker : BackgroundService, IAsyncDisposable
{
    private readonly ILogger<QueueWorker> _logger;

    // RabbitMQ objects.
    private const string queueNameAndRoutingKey = "product";
    private ConnectionFactory? _factory;
    private IConnection? _connection;
    private IChannel? _channel;
    private AsyncEventingBasicConsumer? _consumer;
    private bool _disposed;

    public QueueWorker(ILogger<QueueWorker> logger)
    {
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(queue: queueNameAndRoutingKey, durable: false, exclusive: false, autoDelete: false, arguments: null);

        _consumer = new AsyncEventingBasicConsumer(_channel);
        _consumer.ReceivedAsync += async (model, args) =>
        {
            var body = args.Body.ToArray();
            var message = JsonSerializer.Deserialize<ProductQueueMessage>(body);

            if (message != null)
            {
                _logger.LogInformation($"Received product. Id: {message.Product.ProductId}, Name: {message.Product.ProductName}, Message: {message.Text}");
            }
            else
            {
                _logger.LogInformation("Received unknown message.");
            }

            await Task.CompletedTask;
        };

        await _channel.BasicConsumeAsync(queue: queueNameAndRoutingKey, autoAck: true, consumer: _consumer, cancellationToken: cancellationToken);

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            await Task.Delay(3000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _channel!.CloseAsync(cancellationToken: cancellationToken);

        await _connection!.CloseAsync();

        await base.StopAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        if (_channel != null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }

        _disposed = true;
        base.Dispose();
    }
}
