using Aspire.Hosting.Yarp.Transforms;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("env");

var catalogDb = builder.AddPostgres("catalog-db",
        builder.AddParameter("catalog-username", "example"),
        builder.AddParameter("catalog-password", "example"))
    .WithEnvironment("POSTGRES_USER", "example")
    .WithEnvironment("POSTGRES_PASSWORD", "example")
    .WithEnvironment("POSTGRES_DB", "CatalogDb")
    .WithHostPort(5432);

var basketDb = builder.AddPostgres("basket-db",
        builder.AddParameter("basket-username", "example"),
        builder.AddParameter("basket-password", "example"))
    .WithEnvironment("POSTGRES_DB", "BasketDb")
    .WithHostPort(5433);


var orderDb = builder.AddSqlServer("order-db",
        builder.AddParameter("order-password", "SwN12345678"))
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithHostPort(1433);

var redis = builder.AddRedis("distributed-cache", 6379)
    .WithPassword(
        builder.AddParameter("redis-password", "SwN12345678"))
    .WithHostPort(6379);

var rabbitmq = builder.AddRabbitMQ("ecommerce-mq",
        builder.AddParameter("rabbitmq-username", "guest"),
        builder.AddParameter("rabbitmq-password", "guest"), 5672)
    .WithManagementPlugin(15672);

var basketApiLocal = builder.AddProject("basket-api",
    "../../Services/Basket/Basket.Api/Basket.Api.csproj");

var catalogApiLocal = builder.AddProject("catalog-api",
    "../../Services/Catalog/Catalog.Api/Catalog.Api.csproj");

var discountGrpcLocal = builder.AddProject("discount-service",
    "../../Services/Discount/Discount.Grpc/Discount.Grpc.csproj");

var orderingApiLocal = builder.AddProject("ordering-api",
    "../../Services/Ordering/Ordering.API/Ordering.API.csproj");

var gateway = builder.AddYarp("gateway")
    .WithHostPort(8080)
    .WithConfiguration(yarp =>
    {
        yarp.AddRoute("/basket-api/{**catch-all}",
                basketApiLocal)
            .WithTransformPathRemovePrefix("/basket-api")
            .WithTransformRequestHeader("X-Forwarded-Host",
                "shop.gateway.com")
            .WithTransformResponseHeader("X-Powered-By", "YARP");

        yarp.AddRoute("order-api/{**catch-all}",
                orderingApiLocal)
            .WithTransformPathRemovePrefix("/order-api")
            .WithTransformRequestHeader("X-Forwarded-Host",
                "shop.gateway.com")
            .WithTransformResponseHeader("X-Powered-By", "YARP");

        yarp.AddRoute("/catalog-api/{**catch-all}",
                catalogApiLocal)
            .WithTransformPathRemovePrefix("/catalog-api")
            .WithTransformRequestHeader("X-Forwarded-Host",
                "shop.gateway.com")
            .WithTransformResponseHeader("X-Powered-By", "YARP");
    });

basketApiLocal.WithReference(basketDb)
    .WaitFor(basketDb);
basketApiLocal.WithReference(redis).WaitFor(redis);
basketApiLocal.WithReference(discountGrpcLocal)
    .WaitFor(discountGrpcLocal);
basketApiLocal.WithReference(rabbitmq)
    .WaitFor(rabbitmq);

catalogApiLocal.WithReference(catalogDb)
    .WaitFor(catalogDb);
orderingApiLocal.WithReference(orderDb)
    .WaitFor(orderDb);
orderingApiLocal.WithReference(rabbitmq)
    .WaitFor(rabbitmq);

gateway.WithReference(basketApiLocal)
    .WithReference(orderingApiLocal)
    .WithReference(catalogApiLocal)
    .WaitFor(basketApiLocal)
    .WaitFor(orderingApiLocal)
    .WaitFor(catalogApiLocal);

builder.Build().Run();
