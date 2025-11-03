FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Aptiverse.Api.sln", "./"]

COPY ["src/Aptiverse.Api.Web/Aptiverse.Api.Web.csproj", "src/Aptiverse.Api.Web/"]
COPY ["src/Aptiverse.Application/Aptiverse.Application.csproj", "src/Aptiverse.Application/"]
COPY ["src/Aptiverse.Core/Aptiverse.Core.csproj", "src/Aptiverse.Core/"]
COPY ["src/Aptiverse.Domain/Aptiverse.Domain.csproj", "src/Aptiverse.Domain/"]
COPY ["src/Aptiverse.Infrastructure/Aptiverse.Infrastructure.csproj", "src/Aptiverse.Infrastructure/"]
COPY ["src/Aptiverse.Benchmarks/Aptiverse.Benchmarks.csproj", "src/Aptiverse.Benchmarks/"]

RUN dotnet restore "Aptiverse.Api.sln"

COPY . .

WORKDIR "/src/src/Aptiverse.Api.Web"
RUN dotnet build "Aptiverse.Api.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Aptiverse.Api.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aptiverse.Api.Web.dll", "--urls", "http://0.0.0.0:5000"]