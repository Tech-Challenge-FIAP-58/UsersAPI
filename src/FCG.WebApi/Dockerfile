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
COPY FCG.Users.sln ./

COPY src/FCG.WebApi/FCG.WebApi.csproj src/FCG.WebApi/
COPY src/FCG.Application/FCG.Application.csproj src/FCG.Application/
COPY src/FCG.Core/FCG.Core.csproj src/FCG.Core/
COPY src/FCG.Infra/FCG.Infra.csproj src/FCG.Infra/

# Restore do projeto de entrada (WebApi)
RUN dotnet restore src/FCG.WebApi/FCG.WebApi.csproj

# Copia o restante do código
COPY . .

# Build
RUN dotnet build src/FCG.WebApi/FCG.WebApi.csproj -c $BUILD_CONFIGURATION -o /app/build


# =========================
# Publish
# =========================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish src/FCG.WebApi/FCG.WebApi.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# =========================
# Final (runtime)
# =========================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FCG.WebApi.dll"]
