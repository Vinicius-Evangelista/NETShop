namespace BuildingBlocks.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            message: "[START] Handle request={Request} - Response={Response} - RequestData={RequestData}",
            request,
            typeof(TRequest).Name,
            typeof(TResponse).Name
        );

        var timer = new Stopwatch();

        timer.Start();
        var response = await next();
        timer.Stop();

        var timeTaken = timer.Elapsed;

        if (timeTaken.Seconds > 3)
        {
            logger.LogWarning(
                message: "[PERFORMANCE] Request={Request} took {TimeTaken}",
                typeof(TRequest).Name,
                timeTaken.Seconds
            );
        }

        logger.LogInformation(
            message: "[END] Handle request={Request} - Response={Response} - TimeTaken={TimeTaken}",
            request,
            response,
            timeTaken
        );

        return response;
    }
}
