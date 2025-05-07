using System.Threading.Channels;

using Microsoft.AspNetCore.SignalR.Client;

using Northwind.Common;

Write("Enter a stock (press Enter for MSFT): ");
string? stock = ReadLine();

if (string.IsNullOrEmpty(stock))
{
    stock = "MSFT";
}

HubConnection hubConnection = new HubConnectionBuilder()
  .WithUrl("https://localhost:5111/stockprice")
  .Build();

await hubConnection.StartAsync();

try
{
    CancellationTokenSource cts = new();
    
    IAsyncEnumerable<StockPrice> stockPrices = hubConnection.StreamAsync<StockPrice>("GetStockPriceUpdates", stock, cts.Token);

    await foreach (StockPrice sp in stockPrices)
    {
        WriteLine($"{sp.Stock} is now {sp.Price:C}.");

        Write("Do you want to cancel (y/n)? ");
        ConsoleKey key = ReadKey().Key;

        if (key == ConsoleKey.Y)
        {
            cts.Cancel();
            break;
        }

        WriteLine();
    }
}
catch (Exception ex)
{
    WriteLine($"{ex.GetType()} says {ex.Message}");
}

WriteLine();
WriteLine("Streaming download completed.");

await hubConnection.SendAsync("UploadStocks", GetStocksAsync());

WriteLine("Uploading stocks to service... (press ENTER to stop.)");
ReadLine();
WriteLine("Ending console app.");
