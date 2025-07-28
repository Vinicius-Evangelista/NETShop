using Discount.Grpc;

var assembly = typeof(Program).Assembly;
var builder = WebApplication.CreateBuilder(args: args);

builder.Services.AddCarter();
builder.Services.AddOpenApi();

builder.Services.AddMediatR(configuration: config =>
{
    config.RegisterServicesFromAssembly(assembly: assembly);

    config.AddOpenBehavior(
        openBehaviorType: typeof(ValidationBehavior<,>)
    );

    config.AddOpenBehavior(
        openBehaviorType: typeof(LoggingBehavior<,>)
    );
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddStackExchangeRedisCache(setupAction: options =>
{
    options.Configuration = builder.Configuration.GetConnectionString(
        name: "Redis"
    );
});

builder
    .Services.AddMarten(configure: opts =>
    {
        opts.Connection(
            connectionString: builder.Configuration.GetConnectionString(
                name: "Database"
            ) ?? throw new ArgumentNullException()
        );

        opts.Schema.For<ShoppingCart>()
            .Identity(member: x => x.UserName);
    })
    .UseLightweightSessions();

builder
    .Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(
        configureClient: options =>
        {
            options.Address = new Uri(
                uriString: builder.Configuration[
                    key: "GrpcSettings:DiscountUrl"
                ] ?? throw new ArgumentNullException()
            );
        }
    )
    .ConfigurePrimaryHttpMessageHandler(configureHandler: () =>
    {
        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
        };

        return handler;
    });

builder.Services.AddScoped<BasketRepository>();

builder.Services.AddScoped<IBasketRepository>(
    implementationFactory: provider =>
    {
        var basketRepository =
            provider.GetRequiredService<BasketRepository>();

        return new CachedBasketRepository(
            repository: basketRepository,
            cache: provider.GetRequiredService<IDistributedCache>()
        );
    }
);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler(configure: options => { });
app.MapCarter();
app.UseHealthChecks(path: "/health");

app.Run();
