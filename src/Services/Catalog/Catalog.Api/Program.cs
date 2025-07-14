using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using Catalog.Api.Data;

var builder = WebApplication.CreateBuilder(args: args);

builder.Services.AddCarter();
var assembly = typeof(Program).Assembly;
var buildingBlockAssembly = typeof(ValidationBehavior<,>).Assembly;

builder.Services.AddMediatR(configuration: config =>
{
    config.RegisterServicesFromAssemblies(
        assembly,
        buildingBlockAssembly
    );
    config.AddOpenBehavior(
        openBehaviorType: typeof(ValidationBehavior<,>)
    );
    config.AddOpenBehavior(
        openBehaviorType: typeof(LoggingBehavior<,>)
    );
});

builder.Services.AddValidatorsFromAssembly(assembly: assembly);

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder
    .Services.AddMarten(configure: opts =>
    {
        opts.Connection(
            connectionString: builder.Configuration.GetConnectionString(
                name: "Database"
            )!
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

app.UseHealthChecks(path: "/health");

app.UseExceptionHandler(configure: options => { });

app.Run();

public partial class Program { }
