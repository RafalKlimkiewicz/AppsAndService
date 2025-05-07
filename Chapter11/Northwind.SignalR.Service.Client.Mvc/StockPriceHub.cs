using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.SignalR;

using Northwind.Common;

namespace Northwind.SignalR.Service.Client.Mvc
{
    public class StockPriceHub : Hub
    {
        public async IAsyncEnumerable<StockPrice> GetStockPriceUpdates(string stock, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            double currentPrice = 267.10;

            for (int i = 0; i < 10; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                currentPrice += (Random.Shared.NextDouble() * 10.0) - 5.0;

                StockPrice stockPrice = new(stock, currentPrice);

                Console.WriteLine("[{0}] {1} at {2:C}", DateTime.UtcNow, stockPrice.Stock, stockPrice.Price);

                yield return stockPrice;

                await Task.Delay(4000, cancellationToken);

            }
        }

        public async Task UploadStocks(IAsyncEnumerable<string> stocks)
        {
            await foreach (string stock in stocks)
            {
                Console.WriteLine($"Receiving {stock} from client...");
            }
        }
    }
}
