using System.Diagnostics;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;

using Northwind.EntityModels;
using Northwind.WebApi.Client.Mvc.Models;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace Northwind.WebApi.Client.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("home/products/{name?}")]
        public async Task<IActionResult> Products(string? name = "cha")
        {
            HomeProductsViewModel model = new();

            HttpClient client = _httpClientFactory.CreateClient(name: "Northwind.WebApi.Service");

            model.NameContains = name;
            model.BaseAddress = client.BaseAddress;

            HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: $"api/products/{name}");
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                model.Products = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();
            }
            else
            {
                model.Products = Enumerable.Empty<Product>();

                string content = await response.Content.ReadAsStringAsync();

                string exceptionMessage = content[..content.IndexOf("\r")];

                model.ErrorMessage = string.Format("{0}: {1}:", response.ReasonPhrase, exceptionMessage);
            }

            return View(model);
        }

        public IActionResult SendMessage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string? message, int? productId)
        {
            HomeSendMessageViewModel model = new();
            model.Message = new();

            if (message is null || productId is null)
            {
                model.Error = "Please enter a message and a product ID.";
                return View(model);
            }

            model.Message.Text = message;
            model.Message.Product = new() { ProductId = productId.Value };

            HttpClient client = _httpClientFactory.CreateClient(name: "Northwind.WebApi.Service");

            HttpRequestMessage request = new(method: HttpMethod.Get, requestUri: $"api/products/{productId}");
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Product? product = await response.Content.ReadFromJsonAsync<Product>();

                if (product is not null)
                {
                    model.Message.Product = product;
                }
            }

            // Create a RabbitMQ factory.
            ConnectionFactory factory = new() { HostName = "localhost", Port = 5672 };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            string queueNameAndRoutingKey = "product";

            // If the queue does not exist, it will be created.
            // If the Docker container is restarted, the queue will be lost
            // The queue can be shared with multiple consumers.
            // The queue will not be deleted when the last message is consumer.
            await channel.QueueDeclareAsync(queue: queueNameAndRoutingKey, durable: true, exclusive: false, autoDelete: false, arguments: null);

            byte[] body = JsonSerializer.SerializeToUtf8Bytes(model.Message);

            var properties = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                Persistent = true,
            };

            // The exchange is empty because we are using the default exchange.
            //await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueNameAndRoutingKey, basicProperties: null, body: body);
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueNameAndRoutingKey, mandatory: false,
                basicProperties: properties, body: body, cancellationToken: CancellationToken.None);

            model.Info = "Message sent to queue successfully.";

            return View(model);
        }
    }
}
