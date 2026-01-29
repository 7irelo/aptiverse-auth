FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Aptiverse.Api.slnx", "./"]
COPY ["src/Aptiverse.Api/Aptiverse.Api.csproj", "src/Aptiverse.Api/"]
COPY ["src/Aptiverse.Api.Application/Aptiverse.Api.Application.csproj", "src/Aptiverse.Api.Application/"]
COPY ["src/Aptiverse.Api.Core/Aptiverse.Api.Core.csproj", "src/Aptiverse.Api.Core/"]
COPY ["src/Aptiverse.Api.Domain/Aptiverse.Api.Domain.csproj", "src/Aptiverse.Api.Domain/"]
COPY ["src/Aptiverse.Api.Infrastructure/Aptiverse.Api.Infrastructure.csproj", "src/Aptiverse.Api.Infrastructure/"]

RUN dotnet restore "Aptiverse.Api.slnx"

COPY . .
WORKDIR "/src/src/Aptiverse.Api"
RUN dotnet build "Aptiverse.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Aptiverse.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 5000

RUN useradd --create-home --home-dir /app --shell /bin/bash appuser && chown -R appuser:appuser /app
USER appuser

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aptiverse.Api.dll", "--urls", "http://0.0.0.0:5196"]