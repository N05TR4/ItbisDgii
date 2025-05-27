# ItbisDgii.API/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivos de proyecto
COPY ["ItbisDgii.API/ItbisDgii.API.csproj", "ItbisDgii.API/"]
COPY ["ItbisDgii.Application/ItbisDgii.Application.csproj", "ItbisDgii.Application/"]
COPY ["ItbisDgii.Domain/ItbisDgii.Domain.csproj", "ItbisDgii.Domain/"]
COPY ["ItbisDgii.Infraestructure/ItbisDgii.Infraestructure.csproj", "ItbisDgii.Infraestructure/"]

# Restaurar dependencias
RUN dotnet restore "ItbisDgii.API/ItbisDgii.API.csproj"

# Copiar código fuente
COPY . .

# Construir aplicación
WORKDIR "/src/ItbisDgii.API"
RUN dotnet build "ItbisDgii.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ItbisDgii.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ItbisDgii.API.dll"]