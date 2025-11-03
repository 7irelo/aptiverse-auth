FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Aptiverse.Api.Web/Aptiverse.Api.Web.csproj", "Aptiverse.Api.Web/"]
COPY ["Aptiverse.Application/Aptiverse.Application.csproj", "Aptiverse.Application/"]
COPY ["Aptiverse.Core/Aptiverse.Core.csproj", "Aptiverse.Core/"]
COPY ["Aptiverse.Domain/Aptiverse.Domain.csproj", "Aptiverse.Domain/"]
COPY ["Aptiverse.Infrastructure/Aptiverse.Infrastructure.csproj", "Aptiverse.Infrastructure/"]
RUN dotnet restore "Aptiverse.Api.Web/Aptiverse.Api.Web.csproj"

COPY . .
WORKDIR "/src/Aptiverse.Api.Web"
RUN dotnet build "Aptiverse.Api.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Aptiverse.Api.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aptiverse.Api.Web.dll", "--urls", "http://0.0.0.0:5000"]