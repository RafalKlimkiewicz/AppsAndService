using System.Text;

using Humanizer;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Northwind.AzureFunctions.Service
{
    public class NumbersToWordsFunction
    {
        private readonly ILogger<NumbersToWordsFunction> _logger;

        public NumbersToWordsFunction(ILogger<NumbersToWordsFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(NumbersToWordsFunction))]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req)
        {
            try
            {
                _logger.LogInformation("C# HTTP trigger function processed a request.");

                string? amount = req.Query["amount"];

                HttpResponseData response;

                if (long.TryParse(amount, out long number))
                {
                    response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                    await response.WriteStringAsync("NUBMERS:" + number.ToWords(), Encoding.UTF8);
                }
                else
                {
                    response = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    await response.WriteStringAsync($"Failed to parse: {amount}", Encoding.UTF8);
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
