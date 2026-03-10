# =============================================================
# ARGs globais — disponíveis em todos os stages
# =============================================================
ARG DOTNET_VERSION=8.0
ARG BUILD_CONFIGURATION=Development
ARG APP_VERSION=1.0.0

# =============================================================
# STAGE 1: Base runtime
# Definido primeiro para ser reutilizado no stage final
# =============================================================
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS base

# Usuário não-root — compatível com Alpine e Debian (imagem aspnet usa Debian)
RUN groupadd -r appgroup && useradd -r -g appgroup appuser

WORKDIR /app

# Permissões antes de copiar qualquer coisa
RUN chown -R appuser:appgroup /app

EXPOSE 8080

# =============================================================
# STAGE 2: Restore de dependências (layer altamente cacheável)
# Layer separado: só re-executa se algum .csproj/.sln mudar
# =============================================================
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS restore

WORKDIR /src

COPY FCG.Users.sln ./

COPY src/FCG.WebApi/FCG.WebApi.csproj src/FCG.WebApi/
COPY src/FCG.Application/FCG.Application.csproj src/FCG.Application/
COPY src/FCG.Core/FCG.Core.csproj src/FCG.Core/
COPY src/FCG.Infra/FCG.Infra.csproj src/FCG.Infra/

RUN dotnet restore src/FCG.WebApi/FCG.WebApi.csproj

# =============================================================
# STAGE 3: Build + Publish
# --no-restore: aproveita o cache do stage anterior
# =============================================================
FROM restore AS publish

ARG BUILD_CONFIGURATION

# Copia o restante do código fonte
COPY . .

RUN dotnet publish src/FCG.WebApi/FCG.WebApi.csproj \
    -c ${BUILD_CONFIGURATION} \
    --no-restore \
    -o /app/publish \
    /p:UseAppHost=false

# =============================================================
# STAGE 4: Runtime final (imagem mínima)
# =============================================================
FROM base AS final

# Labels OCI — rastreabilidade no DockerHub, Kubernetes e pipelines CI/CD
# ARGs precisam ser redeclarados após cada FROM para ficarem visíveis
ARG APP_VERSION
LABEL org.opencontainers.image.title="FCG.UserService" \
      org.opencontainers.image.description="Microsserviço de usuários - FCG Pós Tech" \
      org.opencontainers.image.version="${APP_VERSION}" \
      org.opencontainers.image.vendor="Thiago Olszewski" \
      org.opencontainers.image.source="https://github.com/Tech-Challenge-FIAP-58/UsersAPI"

WORKDIR /app

# Copia artefatos publicados com ownership já correto
COPY --from=publish --chown=appuser:appgroup /app/publish .

# Variáveis de ambiente de runtime do .NET
# DOTNET_RUNNING_IN_CONTAINER: ativa otimizações internas do runtime para containers
# DOTNET_USE_POLLING_FILE_WATCHER: FALSE em produção (só útil em dev com hot reload)
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_USE_POLLING_FILE_WATCHER=false

# Aplica usuário não-root
USER appuser

# Health check via wget (disponível na imagem aspnet — curl NÃO está)
# start-period: tempo para o app inicializar antes de começar a checar
HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
    CMD wget -qO- http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "FCG.WebApi.dll"]