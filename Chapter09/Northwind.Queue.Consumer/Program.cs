using Microsoft.EntityFrameworkCore.Metadata;

using Northwind.Queue.Models; // To use ProductQueueMessage.

using RabbitMQ.Client; // To use ConnectionFactory.
using RabbitMQ.Client.Events; // To use EventingBasicConsumer.

using System.Text.Json; // To use JsonSerializer.

string queueName = "product";

ConnectionFactory factory = new() { HostName = "localhost", Port = 5672 };

using IConnection connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

WriteLine("Declaring queue...");
var response = await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

WriteLine($"Queue name: {response.QueueName}, Message count: {response.MessageCount}, Consumer count: {response.ConsumerCount}.");
WriteLine("Waiting for messages...");
AsyncEventingBasicConsumer consumer = new(channel);

consumer.ReceivedAsync += async (model, args) =>
{
    try
    {
        byte[] body = args.Body.ToArray();
        ProductQueueMessage? message = JsonSerializer.Deserialize<ProductQueueMessage>(body);

        if (message != null)
        {
            WriteLine($"Received product. Id: {message.Product.ProductId}, Name: {message.Product.ProductName}, Message: {message.Text}");
        }
        else
        {
            WriteLine("Received unknown message.");
        }
    }
    catch (Exception ex)
    {
        WriteLine($"Error processing message: {ex.Message}");
    }

    await Task.CompletedTask;
};

// Start consuming as messages arrive in the queue.
await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
WriteLine(">>> Press Enter to stop consuming and quit. <<<");
ReadLine();

