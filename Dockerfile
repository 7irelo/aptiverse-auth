FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Aptiverse.Auth.slnx", "./"]
COPY ["src/Aptiverse.Auth/Aptiverse.Auth.csproj", "src/Aptiverse.Auth/"]
COPY ["src/Aptiverse.Application/Aptiverse.Application.csproj", "src/Aptiverse.Application/"]
COPY ["src/Aptiverse.Core/Aptiverse.Core.csproj", "src/Aptiverse.Core/"]
COPY ["src/Aptiverse.Domain/Aptiverse.Domain.csproj", "src/Aptiverse.Domain/"]
COPY ["src/Aptiverse.Infrastructure/Aptiverse.Infrastructure.csproj", "src/Aptiverse.Infrastructure/"]

RUN dotnet restore "Aptiverse.Auth.slnx"

COPY . .
WORKDIR "/src/src/Aptiverse.Auth"
RUN dotnet build "Aptiverse.Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Aptiverse.Auth.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 5000

RUN adduser --disabled-password --home /app --gecos '' appuser && chown -R appuser:appuser /app
USER appuser

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aptiverse.Auth.dll", "--urls", "http://0.0.0.0:5000"]