using Microsoft.Data.SqlClient; // SqlConnectionStringBuilder
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Northwind.EntityModels; // DbContext

namespace DataContext;

public partial class NorthwindContext
{
    private readonly string connectionString = null!;

    public NorthwindContext(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("Northwind") ?? "";
    }

    public NorthwindContext(DbContextOptions<NorthwindContext> options, IConfiguration configuration) : base(options)
    {
        connectionString = configuration.GetConnectionString("Northwind") ?? "";
    }

    private static readonly SetLastRefreshedInterceptor SetLastRefreshedInterceptor = new();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
            else
            {
                SqlConnectionStringBuilder builder = new();

                builder.DataSource = ".";
                builder.InitialCatalog = "Northwind";
                builder.TrustServerCertificate = true;
                builder.MultipleActiveResultSets = true;

                // Because we want to fail fast. Default is 15 seconds.
                builder.ConnectTimeout = 3;

                // If using Windows Integrated authentication.
                builder.IntegratedSecurity = true;

                // If using SQL Server authentication.
                // builder.UserID = Environment.GetEnvironmentVariable("MY_SQL_USR");
                // builder.Password = Environment.GetEnvironmentVariable("MY_SQL_PWD");

                optionsBuilder.UseSqlServer(builder.ConnectionString);
            }
        }

        optionsBuilder.AddInterceptors(SetLastRefreshedInterceptor);
    }
}
