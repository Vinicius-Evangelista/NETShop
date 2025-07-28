using Ordering.API;
using Ordering.Application;
using Ordering.Infrastructure;

var builder = WebApplication.CreateBuilder(args: args);

builder
    .Services.AddApiServices()
    .AddApplicationServices()
    .AddInfrastructureServices(configuration: builder.Configuration);

var app = builder.Build();

app.Run();
