using Hangfire;
using Northwind.Background.Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

SqlConnectionStringBuilder connection = new();
connection.InitialCatalog = "Northwind.HangfireDb";
connection.MultipleActiveResultSets = true;
connection.Encrypt = true;
connection.TrustServerCertificate = true;
connection.ConnectTimeout = 5; // Default is 30 seconds.
connection.DataSource = "."; // To use local SQL Server.
                             // To use Windows Integrated authentication.
connection.IntegratedSecurity = true;

var builder = WebApplication.CreateBuilder(args);

//builder.UserID = "sa";
//builder.Password = "xdR!ty01%2#";
//builder.PersistSecurityInfo = false;

builder.Services.AddHangfire(config => config
  .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
  .UseSimpleAssemblyNameTypeSerializer()
  .UseRecommendedSerializerSettings()
  .UseSqlServerStorage(connection.ConnectionString));


builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseHangfireDashboard();

app.MapGet("/", () => "Navigate to /hangfire to see the Hangfire Dashboard.");

app.MapPost("/schedulejob", ([FromBody] WriteMessageJobDetail job) =>
{
    BackgroundJob.Schedule(methodCall: () => JobHandlers.WriteMessage(job.Message), enqueueAt: DateTimeOffset.UtcNow + TimeSpan.FromSeconds(job.Seconds));
});

app.MapHangfireDashboard();

app.Run();
