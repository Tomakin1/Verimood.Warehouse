FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Core/Verimood.Warehouse.Domain/Verimood.Warehouse.Domain.csproj", "Core/Verimood.Warehouse.Domain/"]
COPY ["Core/Verimood.Warehouse.Application/Verimood.Warehouse.Application.csproj", "Core/Verimood.Warehouse.Application/"]
COPY ["Infrastructure/Verimood.Warehouse.Persistence/Verimood.Warehouse.Persistence.csproj", "Infrastructure/Verimood.Warehouse.Persistence/"]
COPY ["Infrastructure/Verimood.Warehouse.Infrastructure/Verimood.Warehouse.Infrastructure.csproj", "Infrastructure/Verimood.Warehouse.Infrastructure/"]
COPY ["Presentation/Verimood.Warehouse.Api/Verimood.Warehouse.Api.csproj", "Presentation/Verimood.Warehouse.Api/"]

RUN dotnet restore "Presentation/Verimood.Warehouse.Api/Verimood.Warehouse.Api.csproj"

COPY . .
WORKDIR "/src/Presentation/Verimood.Warehouse.Api"
RUN dotnet build "Verimood.Warehouse.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Verimood.Warehouse.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Verimood.Warehouse.Api.dll"]