using Ordering.API;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args: args);

builder
    .Services
    .AddApplicationServices()
    .AddInfrastructureServices(configuration: builder.Configuration)
    .AddApiServices(builder.Configuration);

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}

app.UseHealthChecks("/health");

app.UseApiServices();

app.Run();
