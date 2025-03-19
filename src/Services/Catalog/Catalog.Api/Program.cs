using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using Catalog.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
var assembly = typeof(Program).Assembly;
var buildingBlockAssembly = typeof(ValidationBehavior<,>).Assembly;

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(
        assembly,
        buildingBlockAssembly
    );
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder
    .Services.AddMarten(opts =>
    {
        opts.Connection(
            builder.Configuration.GetConnectionString("Database")!
        );
    })
    .UseLightweightSessions();

if (builder.Environment.IsDevelopment())
{
    builder.Services.InitializeMartenWith<CatalogInitialData>();
}

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapCarter();

app.UseHealthChecks("/health");

app.UseExceptionHandler(options => { });

app.Run();

public partial class Program { }
