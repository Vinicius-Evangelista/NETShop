using BuildingBlocks.Behaviors;

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

builder.Services.AddScoped<IBasketRepository, BasketRepository>();

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

var app = builder.Build();

app.MapCarter();

app.Run();
