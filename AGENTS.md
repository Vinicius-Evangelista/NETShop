# Repository Guidelines

## Project Structure & Module Organization

NETShop is a .NET 9 microservices solution rooted at `src/NETShop.sln`.
Service code lives under `src/Services`: `Catalog/Catalog.Api`,
`Basket/Basket.Api`, `Discount/Discount.Grpc`, and the layered
`Ordering` service (`Ordering.Domain`, `Ordering.Application`,
`Ordering.Infrastructure`, `Ordering.API`). Shared abstractions and messaging
contracts are in `src/BuildingBlocks`. The YARP gateway is in
`src/ApiGateways/Yarp.ApiGateway`, and the Aspire app host is in
`src/Aspire/Shop.Aspire`. Docker Compose files and centralized package
versions are in `src/docker-compose*.yml` and `src/Directory.Packages.props`.

## Build, Test, and Development Commands

Run commands from the repository root unless noted.

- `dotnet restore src/NETShop.sln`: restores all solution packages.
- `dotnet build src/NETShop.sln`: compiles every project.
- `dotnet run --project src/Aspire/Shop.Aspire/Shop.Aspire.csproj`: starts the
  local distributed application with databases, Redis, RabbitMQ, services, and
  gateway wiring.
- `cd src && docker compose up --build`: builds and runs the Compose stack.
- `dotnet format src/NETShop.sln`: applies formatting and analyzer fixes where
  available.

## Coding Style & Naming Conventions

Follow `src/.editorconfig`: UTF-8, CRLF line endings for C# files, four-space
indentation, final newlines, and trimmed trailing whitespace. C# nullable
reference types and implicit usings are enabled across projects. Use PascalCase
for public types and members, prefix interfaces with `I`, and keep command,
query, handler, endpoint, and DTO names aligned with their feature folders
such as `CreateOrderCommand`, `CreateOrderHandler`, and `CreateOrderEndpoint`.
Manage shared dependency versions in `Directory.Packages.props`.

## Testing Guidelines

No dedicated test projects are currently present in the solution. When adding
tests, place them near the related service, for example
`src/Services/Basket/Basket.Api.Tests`, and add the project to
`src/NETShop.sln`. Prefer focused unit tests for handlers, validators, domain
objects, and repository behavior; add integration tests for API, persistence,
messaging, and gRPC boundaries. Run all tests with
`dotnet test src/NETShop.sln`.

## Commit & Pull Request Guidelines

Recent history mostly uses Conventional Commit-style prefixes such as
`feat: add gRPC tracing interceptor`, with one generic `add` commit. Prefer
clear messages like `feat: add basket checkout tracing` or
`fix: handle missing discount coupon`. Pull requests should describe the
change, list validation commands run, link related issues, and include
screenshots or request/response examples when API behavior changes.

## Security & Configuration Tips

Do not commit real secrets. Local sample credentials appear in Aspire and
Compose configuration only for development. Keep environment-specific values in
`appsettings.Development.json`, user secrets, or deployment configuration, and
avoid changing ports or service names without updating gateway and host wiring.
