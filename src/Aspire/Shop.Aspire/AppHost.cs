var builder = DistributedApplication.CreateBuilder(args);

var apiGateway = builder.AddProject("api-gateway",
    "/src/ApiGateways/Yarp.ApiGateway/Yarp.ApiGateway.csproj");


var basketApi = builder.AddProject("basket-api",
    "../src/Services/Basket/Basket.Api/Basket.Api.csproj");
var catalogApi = builder.AddProject("catalog-api",
    "../src/Services/Catalog/Catalog.Api/Catalog.Api.csproj");


var discountGrpc = builder.AddProject("discount-grpc",
    "../src/Services/Discount/Discount.Grpc/Discount.Grpc.csproj");

var orderingApi = builder.AddProject("ordering-api",
    "/src/Services/Ordering/Ordering.API/Ordering.API.csproj");

var redis = builder.AddRedis("redis");

var postgres = builder.AddPostgres("postgres");

var rabbitmq = builder.AddRabbitMQ("rabbitmq");

basketApi.WithReference(redis);
basketApi.WithReference(discountGrpc);
basketApi.WithReference(rabbitmq);


orderingApi.WithReference(rabbitmq);

// Configurar API Gateway para rotear para os serviços
apiGateway.WithReference(basketApi);
apiGateway.WithReference(catalogApi);
apiGateway.WithReference(orderingApi);

builder.Build().Run();
