using Serilog;
using Serilog.Core;

using Logger log = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

Log.Logger = log;
Log.Information("The global logger has been configured.");

Log.Warning("Warning serilog");
Log.Error("This is an error!");
Log.Fatal("Fatal problem!");

ProductPageView pageView = new()
{
    PageTitle = "Chai",
    SiteSection = "Beverages",
    ProductId = 1
};

Log.Information("{@PageView} occured at {Viewed}", pageView, DateTimeOffset.UtcNow);

Log.CloseAndFlush();
