﻿using Northwind.EntityModels; // To use Product.

using System.Net.Http.Json; // To use ReadFromJsonAsync<T> method.
Write("Enter a client name or press Enter: ");
string? clientName = ReadLine();


if (string.IsNullOrEmpty(clientName))
{
    clientName = $"console-client-{Guid.NewGuid()}";
}

WriteLine($"X-Client-Id will be: {clientName}");

HttpClient client = new();
client.BaseAddress = new("https://localhost:5081");
client.DefaultRequestHeaders.Accept.Add(new("application/json"));
// Specify the rate limiting client id for this console app.
client.DefaultRequestHeaders.Add("X-Client-Id", clientName);

while (true)
{
    WriteInColor(string.Format("{0:hh:mm:ss}: ", DateTime.UtcNow), ConsoleColor.DarkGreen);
    int waitFor = 1; // Second.
    try
    {
        HttpResponseMessage response = await client.GetAsync("api/products");
        if (response.IsSuccessStatusCode)
        {
            Product[]? products = await response.Content.ReadFromJsonAsync<Product[]>();
            if (products != null)
            {
                foreach (Product product in products)
                {
                    Write(product.ProductName);
                    Write(", ");
                }
                WriteLine();
            }
        }
        else
        {
            string retryAfter = response.Headers.GetValues("Retry-After").ToArray()[0];

            if (int.TryParse(retryAfter, out waitFor))
            {
                retryAfter = string.Format("I will retry after {0} seconds.", waitFor);
            }

            WriteInColor(string.Format("{0}: {1} {2}", (int) response.StatusCode, await response.Content.ReadAsStringAsync(), 1), ConsoleColor.DarkRed);
            WriteLine();
        }
    }
    catch (Exception ex)
    {
        WriteLine(ex.Message);
    }
    await Task.Delay(TimeSpan.FromSeconds(waitFor));
}
