using System.Net.Http.Headers;

using Polly; // To use AddTransientHttpErrorPolicy method.
using Polly.Contrib.WaitAndRetry; // To use Backoff.
using Polly.Extensions.Http; // To use HttpPolicyExtensions.
using Polly.Retry; // To use AsyncRetryPolicy<T>

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

IEnumerable<TimeSpan> delays = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);

WriteLine("Jittered delays for Polly retries:");

foreach (TimeSpan item in delays)
{
    WriteLine($"  {item.TotalSeconds:N2} seconds.");
}

// Handle network failures, 408 and 5xx status codes.
AsyncRetryPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(delays);

builder.Services.AddHttpClient(name: "Northwind.WebApi.Service",
  configureClient: options =>
  {
      options.BaseAddress = new("https://localhost:5091/");
      options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1.0));
  }).AddPolicyHandler(retryPolicy);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
