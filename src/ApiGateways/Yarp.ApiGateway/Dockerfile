FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS publish
WORKDIR /src

COPY ["ApiGateways/Yarp.ApiGateway/Yarp.ApiGateway.csproj", "ApiGateways/Yarp.ApiGateway/"]

WORKDIR /src/ApiGateways/Yarp.ApiGateway
RUN dotnet restore "Yarp.ApiGateway.csproj" --runtime linux-musl-x64

WORKDIR /src
COPY ["ApiGateways/Yarp.ApiGateway/", "ApiGateways/Yarp.ApiGateway/"]

WORKDIR /src/ApiGateways/Yarp.ApiGateway
RUN dotnet publish "Yarp.ApiGateway.csproj" -c Release -o /app/publish \
  --no-restore \
  --runtime linux-musl-x64 \
  --self-contained true \
  /p:PublishSingleFile=true \
  /p:PublishTrimmed=true \
  /p:TrimMode=link 

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine AS final

RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app

RUN apk upgrade musl

USER dotnetuser
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["./Yarp.ApiGateway"]
