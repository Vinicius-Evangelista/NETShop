var builder = DistributedApplication.CreateBuilder(args);

// Adiciona os contêineres de banco de dados, cache e message broker
var catalogDb = builder.AddPostgres("catalog-db")
    .WithEnvironment("POSTGRES_USER", "example")
    .WithEnvironment("POSTGRES_PASSWORD", "example")
    .WithEnvironment("POSTGRES_DB", "CatalogDb")
    .WithHostPort(5432);

var basketDb = builder.AddPostgres("basket-db")
        .WithDataBindMount("/var/lib/postgresql/data/")
    .WithEnvironment("POSTGRES_USER", "example")
    .WithEnvironment("POSTGRES_PASSWORD", "example")
    .WithEnvironment("POSTGRES_DB", "BasketDb")
    .WithHostPort(5432);

var orderDb = builder.AddSqlServer("order-db")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("SA_PASSWORD", "SwN12345678")
    .WithHostPort(1433);

var redis = builder.AddRedis("distributed-cache")
    .WithHostPort(6379);

var rabbitmq = builder.AddRabbitMQ("ecommerce-mq")
    .WithEnvironment("RABBITMQ_DEFAULT_USER", "guest")
    .WithEnvironment("RABBITMQ_DEFAULT_PASS", "guest")
    .WithManagementPlugin(5672);

var apiGateway = builder.AddProject("api-gateway", "../../ApiGateways/Yarp.ApiGateway/Yarp.ApiGateway.csproj")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_HTTP_PORTS", "8080")
    .WithEnvironment("ASPNETCORE_HTTPS_PORTS", "8081")
    .WithEndpoint(6004)
    .WithEndpoint(6064, scheme: "https", name: "https")
    .WithVolume("${APPDATA}/Microsoft/UserSecrets", "/home/app/.microsoft/usersecrets:ro")
    .WithVolume("//c/Users/vinie/.aspnet/https", "/https:ro");

var basketApi = builder.AddProject("basket-api", "../../Services/Basket/Basket.Api/Basket.Api.csproj")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_HTTP_PORTS", "8080")
    .WithEnvironment("ASPNETCORE_HTTPS_PORTS", "8081")
    .WithEnvironment("ConnectionStrings__Database", "Server={basket-db.bindings.postgres};Database=BasketDb;Port=5432;User Id=example;Password=example;Include Error Detail=true;")
    .WithEnvironment("ConnectionStrings__Redis", "{distributed-cache.bindings.tcp}")
    .WithEnvironment("GrpcSettings__DiscountUrl", "https://{discount-grpc.bindings.https}")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", "123senha")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", "/https/aspnetapp.pfx")
    .WithEnvironment("MessageBroker__Host", "amqp://{message-broker.bindings.rabbitmq}")
    .WithEnvironment("MessageBroker__UserName", "guest")
    .WithEnvironment("MessageBroker__Password", "guest")
    .WithEndpoint(hostPort: 6001, scheme: "http", name: "http")
    .WithEndpoint(hostPort: 6061, scheme: "https", name: "https")
    .WithVolume("${APPDATA}/Microsoft/UserSecrets", "/home/app/.microsoft/usersecrets:ro")
    .WithVolume("//c/Users/vinie/.aspnet/https", "/https:ro");

var catalogApi = builder.AddProject("catalog-api", "../../Services/Catalog/Catalog.Api/Catalog.Api.csproj")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_HTTP_PORTS", "8080")
    .WithEnvironment("ASPNETCORE_HTTPS_PORTS", "8081")
    .WithEnvironment("ConnectionStrings__Database", "Server={catalog-db.bindings.postgres};Database=CatalogDb;Port=5432;User Id=example;Password=example;Include Error Detail=true;")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", "123senha")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", "/https/aspnetapp.pfx")
    .WithEndpoint(hostPort: 6000, scheme: "http", name: "http")
    .WithEndpoint(hostPort: 6060, scheme: "https", name: "https")
    .WithVolume("${APPDATA}/Microsoft/UserSecrets", "/home/app/.microsoft/usersecrets:ro")
    .WithVolume("//c/Users/vinie/.aspnet/https", "/https:ro");

var discountGrpc = builder.AddProject("discount-grpc", "../../Services/Discount/Discount.Grpc/Discount.Grpc.csproj")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_HTTP_PORTS", "8080")
    .WithEnvironment("ASPNETCORE_HTTPS_PORTS", "8081")
    .WithEnvironment("ConnectionStrings__DiscountDb", "Data Source=Discount.db")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", "123senha")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", "/https/aspnetapp.pfx")
    .WithEndpoint(hostPort: 6002, scheme: "http", name: "http")
    .WithEndpoint(hostPort: 6062, scheme: "https", name: "https")
    .WithVolume("${APPDATA}/Microsoft/UserSecrets", "/home/app/.microsoft/usersecrets:ro")
    .WithVolume("//c/Users/vinie/.aspnet/https", "/https:ro")
    .WithVolumeMount("discount_volume", "/app/data");

var orderingApi = builder.AddProject("ordering-api", "../../Services/Ordering/Ordering.API/Ordering.API.csproj")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("ASPNETCORE_HTTP_PORTS", "8080")
    .WithEnvironment("ASPNETCORE_HTTPS_PORTS", "8081")
    .WithEnvironment("ConnectionStrings__Database", "Server={order-db.bindings.tcp};Database=OrderDb;User Id=sa;Password=SwN12345678;Encrypt=False;TrustServerCertificate=True")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", "123senha")
    .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", "/https/aspnetapp.pfx")
    .WithEnvironment("MessageBroker__Host", "amqp://{message-broker.bindings.rabbitmq}")
    .WithEnvironment("MessageBroker__UserName", "guest")
    .WithEnvironment("MessageBroker__Password", "guest")
    .WithEndpoint(hostPort: 6003, scheme: "http", name: "http")
    .WithEndpoint(hostPort: 6063, scheme: "https", name: "https")
    .WithVolume("${APPDATA}/Microsoft/UserSecrets", "/home/app/.microsoft/usersecrets:ro")
    .WithVolume("//c/Users/vinie/.aspnet/https", "/https:ro");

// Configurando dependências entre os serviços
basketApi.WithReference(basketDb);
basketApi.WithReference(redis);
basketApi.WithReference(discountGrpc);
basketApi.WithReference(rabbitmq);

catalogApi.WithReference(catalogDb);

orderingApi.WithReference(orderDb);
orderingApi.WithReference(rabbitmq);

apiGateway.WithReference(basketApi);
apiGateway.WithReference(catalogApi);
apiGateway.WithReference(orderingApi);

builder.Build().Run();
