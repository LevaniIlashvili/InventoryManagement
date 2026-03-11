FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["InventoryManagement.Api/InventoryManagement.Api.csproj", "InventoryManagement.Api/"]
COPY ["InventoryManagement.Application/InventoryManagement.Application.csproj", "InventoryManagement.Application/"]
COPY ["InventoryManagement.Domain/InventoryManagement.Domain.csproj", "InventoryManagement.Domain/"]
COPY ["InventoryManagement.Infrastructure/InventoryManagement.Infrastructure.csproj", "InventoryManagement.Infrastructure/"]

RUN dotnet restore "InventoryManagement.Api/InventoryManagement.Api.csproj"

COPY . .

WORKDIR "/src/InventoryManagement.Api"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "InventoryManagement.Api.dll"]