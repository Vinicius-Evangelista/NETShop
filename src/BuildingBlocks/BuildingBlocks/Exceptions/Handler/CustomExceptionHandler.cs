using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Exceptions.Handler;

public class CustomExceptionHandler(
    ILogger<CustomExceptionHandler> logger
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        logger.LogError(
            exception: exception,
            message: "Error message: {exceptionMessage}, Time of occurrence: {time}",
            exception.Message,
            DateTime.UtcNow
        );

        (string detail, string title, int statusCode) details =
            exception switch
            {
                InternalServerException => (
                    exception.Message,
                    "Internal Server Error",
                    StatusCodes.Status500InternalServerError
                ),
                ValidationException => (
                    exception.Message,
                    "Validation Error",
                    StatusCodes.Status400BadRequest
                ),
                BadRequestException => (
                    exception.Message,
                    "Bad Request",
                    StatusCodes.Status400BadRequest
                ),
                NotFoundException => (
                    exception.Message,
                    "Not Found",
                    StatusCodes.Status404NotFound
                ),
                _ => (
                    exception.Message,
                    exception.GetType().Name,
                    StatusCodes.Status404NotFound
                ),
            };

        var problemDetails = new ProblemDetails
        {
            Title = details.title,
            Detail = details.detail,
            Status = details.statusCode,
            Instance = httpContext.Request.Path,
        };

        problemDetails.Extensions.Add(
            key: "traceId",
            value: httpContext.TraceIdentifier
        );

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions.Add(
                key: "errors",
                value: validationException.Errors
            );
        }

        await httpContext.Response.WriteAsJsonAsync(
            value: problemDetails,
            cancellationToken: cancellationToken
        );

        return true;
    }
}
