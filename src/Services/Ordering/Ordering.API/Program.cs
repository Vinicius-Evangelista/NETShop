using Ordering.API;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args: args);

builder
    .Services.AddApiServices(builder.Configuration)
    .AddApplicationServices()
    .AddInfrastructureServices(configuration: builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}


app.Run();
