# ---------- Estagio 1: build do Angular ----------
FROM node:20-alpine AS client-build
WORKDIR /src/GestaoFacil.Client

# copia so os manifests primeiro para aproveitar o cache do npm ci
COPY GestaoFacil.Client/package.json GestaoFacil.Client/package-lock.json ./
RUN npm ci

COPY GestaoFacil.Client/ ./
RUN npm run build -- --configuration production

# ---------- Estagio 2: build/publish do .NET ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS server-build
WORKDIR /src

# restore em camada separada (so o csproj muda pouco)
COPY GestaoFacil.Server/GestaoFacil.Server.csproj GestaoFacil.Server/
RUN dotnet restore GestaoFacil.Server/GestaoFacil.Server.csproj

COPY GestaoFacil.Server/ GestaoFacil.Server/
RUN dotnet publish GestaoFacil.Server/GestaoFacil.Server.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# ---------- Estagio 3: runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=server-build /app/publish ./
# o builder "application" do Angular 19 emite em dist/<projeto>/browser
COPY --from=client-build /src/GestaoFacil.Client/dist/gestao-facil.client/browser/ ./wwwroot/

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# usuario nao-root ja existe na imagem aspnet:8.0
USER $APP_UID

ENTRYPOINT ["dotnet", "GestaoFacil.Server.dll"]
