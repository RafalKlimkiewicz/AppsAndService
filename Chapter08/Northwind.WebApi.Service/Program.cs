using DataContext;

using Northwind.WebApi.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(defaultScheme: "Bearer")
  .AddJwtBearer();

builder.Services.AddCustomRateLimiting(builder.Configuration);

builder.Services.AddCustomCors();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddNorthwindContext();

builder.Services.AddCustomHttpLogging();

var app = builder.Build();

app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHttpLogging();

await app.UseCustomClientRateLimiting();

// app.UseCors(policyName: "Northwind.Mvc.Policy");

// Without a named policy the middleware is added but not active.
app.UseCors();

app.MapGets(); // Default pageSize: 10.
app.MapPosts();
app.MapPuts();
app.MapDeletes();

app.Run();
