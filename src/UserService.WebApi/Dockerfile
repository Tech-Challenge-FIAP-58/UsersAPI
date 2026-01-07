# syntax=docker/dockerfile:1

# =========================
# Base (runtime)
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Se você NÃO tem APP_UID definido no compose/ambiente, isso quebra.
# Mantive como opcional: comente se não estiver usando.
# USER $APP_UID


# =========================
# Build (restore + build)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia apenas o necessário para restore (melhor cache)
COPY UserService.sln ./

COPY src/UserService.WebApi/UserService.WebApi.csproj src/UserService.WebApi/
COPY src/UserService.Application/UserService.Application.csproj src/UserService.Application/
COPY src/UserService.Core/UserService.Core.csproj src/UserService.Core/
COPY src/UserService.Infra/UserService.Infra.csproj src/UserService.Infra/

# Restore do projeto de entrada (WebApi)
RUN dotnet restore src/UserService.WebApi/UserService.WebApi.csproj

# Copia o restante do código
COPY . .

# Build
RUN dotnet build src/UserService.WebApi/UserService.WebApi.csproj -c $BUILD_CONFIGURATION -o /app/build


# =========================
# Publish
# =========================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish src/UserService.WebApi/UserService.WebApi.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# =========================
# Final (runtime)
# =========================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.WebApi.dll"]
