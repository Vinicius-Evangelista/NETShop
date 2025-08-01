using System.Reflection;

namespace BuildingBlocks.Messaging.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddMessageBroker(
        this IServiceCollection services,
        IConfiguration configuration, Assembly? assembly = null)
    {
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            if (assembly != null)
            {
                busConfigurator.AddConsumers(assembly);
            }

            busConfigurator.UsingRabbitMq((context, configurator) =>
            {
                var rabbitMqSettings = configuration.GetSection("RabbitMQ");
                configurator.Host(
                    rabbitMqSettings["Host"],
                    rabbitMqSettings["VirtualHost"],
                    hostConfigurator =>
                    {
                        hostConfigurator.Username(rabbitMqSettings["Username"]);
                        hostConfigurator.Password(rabbitMqSettings["Password"]);
                    });

                configurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
