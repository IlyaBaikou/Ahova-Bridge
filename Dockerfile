FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src
COPY Directory.Build.props Directory.Packages.props Ahova.Bridge.slnx ./
COPY src/Ahova.Bridge/Ahova.Bridge.csproj src/Ahova.Bridge/
RUN dotnet restore src/Ahova.Bridge/Ahova.Bridge.csproj
COPY src/Ahova.Bridge src/Ahova.Bridge
RUN dotnet publish src/Ahova.Bridge/Ahova.Bridge.csproj -c Release --no-restore -o /app /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
LABEL org.opencontainers.image.source="https://github.com/IlyaBaikou/Ahova-Bridge" \
      org.opencontainers.image.description="Outbound-only Ahova private AI and family storage bridge" \
      org.opencontainers.image.licenses="MIT"
RUN addgroup -S ahova && adduser -S -G ahova -u 10001 ahova
WORKDIR /app
COPY --from=build /app ./
RUN mkdir -p /var/lib/ahova-bridge && chown -R ahova:ahova /var/lib/ahova-bridge /app
USER ahova
ENV ASPNETCORE_URLS=http://0.0.0.0:7433
ENV Bridge__StateDirectory=/var/lib/ahova-bridge
EXPOSE 7433
HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget -q --spider http://127.0.0.1:7433/health/live || exit 1
ENTRYPOINT ["dotnet", "Ahova.Bridge.dll"]
