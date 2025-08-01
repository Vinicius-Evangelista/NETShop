FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS publish
WORKDIR /src

COPY ["Directory.Packages.props", "./"]

COPY ["Services/Ordering/Ordering.API/Ordering.API.csproj", "Services/Ordering/Ordering.API/"]
COPY ["BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/BuildingBlocks/"]
COPY ["Services/Ordering/Ordering.Application/Ordering.Application.csproj", "Services/Ordering/Ordering.Application/"]
COPY ["Services/Ordering/Ordering.Infrastructure/Ordering.Infrastructure.csproj", "Services/Ordering/Ordering.Infrastructure/"]
COPY ["Services/Ordering/Ordering.Domain/Ordering.Domain.csproj", "Services/Ordering/Ordering.Domain/"]

WORKDIR /src/Services/Ordering/Ordering.API
RUN dotnet restore "Ordering.API.csproj" --runtime linux-musl-x64

WORKDIR /src
COPY ["Services/Ordering/Ordering.API/", "Services/Ordering/Ordering.API/"]
COPY ["BuildingBlocks/BuildingBlocks/", "BuildingBlocks/BuildingBlocks/"]
COPY ["Services/Ordering/Ordering.Application/", "Services/Ordering/Ordering.Application/"]
COPY ["Services/Ordering/Ordering.Infrastructure/", "Services/Ordering/Ordering.Infrastructure/"]
COPY ["Services/Ordering/Ordering.Domain/", "Services/Ordering/Ordering.Domain/"]

WORKDIR /src/Services/Ordering/Ordering.API
RUN dotnet publish "Ordering.API.csproj" -c Release -o /app/publish \
  --no-restore \
  --runtime linux-musl-x64 \
  --self-contained true \
  /p:PublishTrimmed=true \
  /p:TrimMode=link 

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine AS final

RUN apk add --no-cache icu-libs

RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV LC_ALL=en_US.UTF-8
ENV LANG=en_US.UTF-8

RUN apk upgrade musl

USER dotnetuser
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["./Ordering.API"]
