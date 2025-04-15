using DataContext;

using Northwind.WebApi.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddNorthwindContext();
builder.Services.AddCustomHttpLogging();
builder.Services.AddCustomCors();
builder.Services.AddCustomRateLimiting(builder.Configuration);

var app = builder.Build();

app.MapGets()
    .MapPosts()
    .MapPuts()
    .MapDeletes();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.UseCustomClientRateLimiting();
app.UseHttpsRedirection();
app.UseHttpLogging();
//app.UseCors(policyName: "Northwind.Mvc.Policy");
app.UseCors();

app.Run();
