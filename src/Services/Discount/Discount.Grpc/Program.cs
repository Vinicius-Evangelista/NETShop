using Discount.Grpc.Data;
using Discount.Grpc.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args: args);

builder.Services.AddGrpc();
builder.Services.AddDbContext<DiscountDbContext>(
    optionsAction: options =>
        options.UseSqlite(
            connectionString: builder.Configuration.GetConnectionString(
                name: "DiscountDb"
            )
        )
);

var app = builder.Build();

app.MapGrpcService<DiscountService>();
app.MapGet(
    pattern: "/",
    handler: () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
);

app.UseMigration();

app.Run();
