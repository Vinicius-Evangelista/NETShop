using BuildingBlocks.Behaviors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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

app.UseExceptionHandler(exceptionHandler =>
{
    exceptionHandler.Run(async context =>
    {
        var exception = context
            .Features.Get<IExceptionHandlerFeature>()
            ?.Error;

        if (exception is null)
        {
            return;
        }

        var problemsDetails = new ProblemDetails()
        {
            Title = exception.Message,
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.StackTrace,
        };

        var logger = context.RequestServices.GetRequiredService<
            ILogger<Program>
        >();

        logger.LogError(exception, exception.Message);

        context.Response.StatusCode =
            StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemsDetails);
    });
});

app.Run();

public partial class Program { }
