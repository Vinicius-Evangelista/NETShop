using Catalog.Api.Behaviors;
using FluentValidation;

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
    config.AddBehavior(
        typeof(IPipelineBehavior<,>),
        typeof(ValidationBehavior<,>)
    );
});

builder.Services.AddValidatorsFromAssembly(assembly);

builder
    .Services.AddMarten(opts =>
    {
        opts.Connection(
            builder.Configuration.GetConnectionString("Database")!
        );
    })
    .UseLightweightSessions();

var app = builder.Build();

app.MapCarter();
app.Run();

public partial class Program { }
